using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace Senjyouhara.Common.Log
{
    /// <summary>
    /// 支持多进程并发写日志的LogWriter版本
    /// </summary>
    internal class LogWriter : IDisposable
    {
        #region 字段属性

        private string _basePath;

        private int _fileSize = 2 * 1024 * 1024; //日志分隔文件大小

        private LogStream _currentStream = new LogStream();

        private string _logFormat = "{datetime}"; //日志文件名格式化
        private string _dateFormat = "yyyyMMdd"; //日志文件名日期格式化

        private string _rootFolder = "Log"; //日志文件夹名称

        private string _historyArchiveFolder = "archives"; //历史日志文件夹名称，对30天以前的文件进行归档存放
#if (DEBUG)
        private static string _stringFormat = "[{datetime}] {logType} [{line} Line] [{fullName}] {msg} \r\n";
#else
        private static string _stringFormat = "[{datetime}] {logType} {msg} \r\n";
#endif

        private Mutex _mutex;

        private DateTime _lastCheckFileExistsTime = DateTime.Now;

        private DateTime _lastReadFileSizeTime = DateTime.Now;

        private readonly ConcurrentQueue<string> logMsgQueue = new();

        private readonly CancellationTokenSource cts = null;

        #endregion

        #region LogWriter

        public LogWriter()
        {
            _mutex = new Mutex(false, "Global\\Mutex.LogWriter." + "7693FFAD38004F6B8FD31F6A8B4CE2BD");
            cts = new CancellationTokenSource();
            Init();
            ListenSaveLogAsync(cts.Token);
        }

        #endregion

        #region Init

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            //初始化 _basePath
            InitBasePath();

            //更新日志写入流
            UpdateCurrentStream();

            InitArchiveStore();
        }

        #endregion

        #region 初始化 _basePath

        /// <summary>
        /// 初始化 _basePath
        /// </summary>
        private void InitBasePath()
        {
            _basePath = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory).Replace("\\", "/");
        }

        #endregion

        #region 对历史归档文件进行创建文件夹存放，避免文件展示太多

        private void InitArchiveStore()
        {
            if (!Directory.Exists(PathCombine(_basePath, _rootFolder, _historyArchiveFolder)))
            {
                Directory.CreateDirectory(PathCombine(_basePath, _rootFolder, _historyArchiveFolder));
            }

            var files = Directory.GetFiles(PathCombine(_basePath, _rootFolder))
                .Where(v => !v.Contains(_historyArchiveFolder)).Select(v => PathCombine(v)).ToList();
            var Year = DateTime.Now.Year;
            var Month = (DateTime.Now.Month + "").PadLeft(2, '0');
            var indexOf = _logFormat.IndexOf("{datetime}");
            var nowDate = DateTime.Now;

            foreach (var file in files)
            {
                if (indexOf >= 0)
                {
                    var fileName = file.Substring(file.LastIndexOf("/") + 1);
                    var year = fileName.Substring(indexOf, 4);
                    var month = fileName.Substring(indexOf + 4, 2);
                    var date = new DateTime(nowDate.Year, nowDate.Month, 1);
                    var fileDate = new DateTime(int.Parse(year), int.Parse(month), 1);
                    int totalMonth = date.Year * 12 + date.Month - fileDate.Year * 12 - fileDate.Month;
                    if (totalMonth > 0)
                    {
                        var dirPath = PathCombine(_basePath, _rootFolder, _historyArchiveFolder, year + "-" + month);
                        if (!Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }

                        Directory.Move(file, PathCombine(dirPath, fileName));
                    }
                }
            }
        }

        #endregion

        #region 初始化 _currentArchiveIndex

        /// <summary>
        /// 初始化 _currentArchiveIndex
        /// </summary>
        private void InitCurrentArchiveIndex()
        {
            _currentStream.CurrentArchiveIndex = -1;
            Regex regex = new Regex(_currentStream.CurrentLogFileName + "_*(\\d*).txt");
            string[] fileArr =
                Directory.GetFiles(_currentStream.CurrentLogFileDir, _currentStream.CurrentLogFileName + "*");
            foreach (string file in fileArr)
            {
                Match match = regex.Match(file);
                if (match.Success)
                {
                    string str = match.Groups[1].Value;
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        int temp = Convert.ToInt32(str);
                        if (temp > _currentStream.CurrentArchiveIndex)
                        {
                            _currentStream.CurrentArchiveIndex = temp;
                        }
                    }
                }
            }
        }

        #endregion

        #region 初始化 _currentFileSize

        /// <summary>
        /// 初始化 _currentFileSize
        /// </summary>
        private void InitCurrentFileSize()
        {
            FileInfo fileInfo = new FileInfo(_currentStream.CurrentLogFilePath);
            _currentStream.CurrentFileSize = fileInfo.Length;
        }

        #endregion

        #region CreateLogDir()

        /// <summary>
        /// 创建日志目录
        /// </summary>
        private void CreateLogDir()
        {
            string logDir = PathCombine(_basePath, _rootFolder);
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
        }

        #endregion

        #region CreateStream

        /// <summary>
        /// 创建日志写入流
        /// </summary>
        private void CreateStream()
        {
            _currentStream.CurrentFileStream = new FileStream(
                _currentStream.CurrentLogFilePath,
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite,
                FileShare.ReadWrite | FileShare.Delete);
        }

        #endregion

        #region CloseStream

        /// <summary>
        /// 关闭日志写入流
        /// </summary>
        private void CloseStream()
        {
            if (_currentStream.CurrentFileStream != null)
            {
                _currentStream.CurrentFileStream.Close();
            }
        }

        #endregion

        #region Dispose 释放资源

        public void Dispose()
        {
            cts.Cancel();
            var logMsgList = logMsgQueue.ToList();
            WriteFile(logMsgList);
            CloseStream();

            _currentStream.CurrentFileStream = null;
            _currentStream = null;
        }

        #endregion

        #region 拼接日志内容

        /// <summary>
        /// 拼接日志内容
        /// </summary>
        private static string CreateLogString(LogType logType, string log,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
        )
        {
            var str = "";
            //取得当前代码的命名空间    
            //父方法名
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
            // System.Reflection.MethodBase mb = st.GetFrame(6).GetMethod();
            var stackFrames = st.GetFrames();
            var findStake = stackFrames[4];
            var line = findStake.GetFileLineNumber();
            for (var i = 4; i < stackFrames.Length; i++)
            {
                if (!string.IsNullOrEmpty(stackFrames[i].GetFileName()))
                {
                    findStake = stackFrames[i];
                    var fileName = findStake.GetFileName();
                    line = findStake.GetFileLineNumber();
                    if (fileName.Equals(sourceFilePath) && line.Equals(sourceLineNumber))
                    {
                        break;
                    }
                }
            }

            var mb = findStake.GetMethod();
            var dt = mb.DeclaringType;
            //////取得父方法命名空间    
            //str += mb.DeclaringType.Namespace + "\n";
            ////取得父方法类名    
            //str += mb.DeclaringType.Name + "\n";
            //////取得父方法类全名    
            //str += mb.DeclaringType.FullName + "\n";
            //////取得父方法名    
            //str += mb.Name + "\n";

            var tmp = dt;
            while (tmp.DeclaringType != null)
            {
                if (tmp.DeclaringType != null)
                {
                    tmp = tmp.DeclaringType;
                }
            }

            var FullName = tmp.FullName;
            str = FullName + "." + memberName;

            // str = dt.FullName.Replace("+<>c", "");
            // str += "." + Regex.Replace(mb.Name, @"<(.+)>.+", "$1");
            var result = _stringFormat
                    .Replace("{datetime}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                    .Replace("{logType}", ("[" + logType.ToString() + "]").PadRight(7, ' '))
                    .Replace("{line}", line.ToString())
                    .Replace("{fullName}", str)
                    .Replace("{msg}", log)
                ;
            return result;
        }

        #endregion

        #region 写文件

        /// <summary>
        /// 写文件
        /// </summary>
        private void WriteFile(List<string> logs)
        {
            try
            {
                //判断是否更新Stream
                string dateStr = DateTime.Now.ToString(_dateFormat);
                var logFileName = _logFormat.Replace("{datetime}", dateStr);
                if (_currentStream.CurrentLogFileName != logFileName)
                {
                    _currentStream.CurrentLogFileName = logFileName;
                    UpdateCurrentStream();
                }

                //判断文件是否存在
                if (DateTime.Now.Subtract(_lastCheckFileExistsTime).TotalMilliseconds > 500)
                {
                    _lastCheckFileExistsTime = DateTime.Now;
                    if (!File.Exists(_currentStream.CurrentLogFilePath))
                    {
                        UpdateCurrentStream();
                    }
                }

                try
                {
                    _mutex.WaitOne();
                }
                catch (AbandonedMutexException ex)
                {
                    Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
                }

                //读取文件大小
                if (DateTime.Now.Subtract(_lastReadFileSizeTime).TotalMilliseconds > 100)
                {
                    _lastReadFileSizeTime = DateTime.Now;
                    if (File.Exists(_currentStream.CurrentLogFilePath))
                    {
                        _currentStream.CurrentFileSize = new FileInfo(_currentStream.CurrentLogFilePath).Length;
                    }
                }

                //判断是否创建Archive
                byte[] bArr = Encoding.UTF8.GetBytes(string.Join("", logs.ToArray()));
                int byteCount = bArr.Length;

                _currentStream.CurrentFileSize += byteCount;

                if (_currentStream.CurrentFileSize >= _fileSize)
                {
                    _currentStream.CurrentFileSize = byteCount;
                    CreateArchive();
                }

                //日志内容写入文件
                _currentStream.CurrentFileStream.Seek(0, SeekOrigin.End);
                _currentStream.CurrentFileStream.Write(bArr, 0, bArr.Length);
                _currentStream.CurrentFileStream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        #endregion

        #region CreateArchive

        /// <summary>
        /// 创建日志存档
        /// </summary>
        private void CreateArchive()
        {
            try
            {
                CloseStream(); //关闭日志写入流

                string fileName = Path.GetFileNameWithoutExtension(_currentStream.CurrentLogFilePath);
                string newFilePath = PathCombine(_currentStream.CurrentLogFileDir,
                    fileName + "_" + (++_currentStream.CurrentArchiveIndex) + ".txt");

                if (!File.Exists(newFilePath))
                {
                    File.Copy(_currentStream.CurrentLogFilePath, newFilePath); //存档

                    //清空
                    _currentStream.CurrentFileStream = new FileStream(_currentStream.CurrentLogFilePath, FileMode.Open,
                        FileAccess.Write, FileShare.ReadWrite);
                    _currentStream.CurrentFileStream.SetLength(0);
                    _currentStream.CurrentFileStream.Close();
                }
                else
                {
                    //初始化 _currentFileSize
                    InitCurrentFileSize();
                }

                CreateStream(); //创建日志写入流
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        #endregion

        #region UpdateCurrentStream

        /// <summary>
        /// 更新日志写入流
        /// </summary>
        private void UpdateCurrentStream()
        {
            try
            {
                try
                {
                    _mutex.WaitOne();
                }
                catch (AbandonedMutexException ex)
                {
                    Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
                }

                //创建目录
                CreateLogDir();

                //关闭日志写入流
                CloseStream();

                var logFile = _logFormat.Replace("{datetime}", DateTime.Now.ToString(_dateFormat));
                //创建新的日志路径
                _currentStream.CurrentLogFileName = logFile;
                _currentStream.CurrentLogFileDir = PathCombine(_basePath, _rootFolder);
                _currentStream.CurrentLogFilePath = PathCombine(_currentStream.CurrentLogFileDir,
                    _currentStream.CurrentLogFileName + ".txt");

                //创建日志写入流
                CreateStream();

                //初始化 _currentArchiveIndex
                InitCurrentArchiveIndex();

                //初始化 _currentFileSize
                InitCurrentFileSize();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        #endregion

        #region 写日志

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="log">日志内容</param>
        public void WriteLog(LogType type, string log, [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                log = CreateLogString(type, log, memberName, sourceFilePath, sourceLineNumber);
#if (DEBUG)
                Debug.Write(log);
                Console.Write(log);
#else
                Console.Write(log);
#endif
                if (LogConfig.IsWriteFile)
                {
                    logMsgQueue.Enqueue(log);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        #endregion

        #region 定时写入日志

        private void ListenSaveLogAsync(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() =>
            {
                DateTime lastSaveLogTime = DateTime.Now;
                while (!cancellationToken.IsCancellationRequested) //如果没有取消线程，则一直监听执行写LOG
                {
                    //如是待写日志消息累计>=10条或上一次距离现在写日志时间超过30s则需要批量提交日志
                    if (logMsgQueue.Count >= 10 ||
                        (logMsgQueue.Count > 0 && (DateTime.Now - lastSaveLogTime).TotalSeconds > 30))
                    {
                        List<string> logMsgList = new List<string>();
                        string logMsgItems = null;

                        while (logMsgList.Count < 10 && logMsgQueue.TryDequeue(out logMsgItems))
                        {
                            logMsgList.Add(logMsgItems);
                        }

                        WriteFile(logMsgList);

                        lastSaveLogTime = DateTime.Now;
                    }
                    else
                    {
                        SpinWait.SpinUntil(() => logMsgQueue.Count >= 10, 5000); //自旋等待直到日志队列有>=10的记录或超时5S后再进入下一轮的判断
                    }
                }
            }, cancellationToken);
        }

        #endregion

        #region PathCombine

        /// <summary>
        /// Path.Combine反斜杠替换为正斜杠
        /// </summary>
        private String PathCombine(params string[] paths)
        {
            return Path.Combine(paths).Replace("\\", "/");
        }

        #endregion
    }
}