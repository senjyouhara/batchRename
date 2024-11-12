using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Senjyouhara.Common.Log
{
    public class Log
    {
        static Log()
        {

        }
        
        // 需要在App.xaml.cs 定义一些异常捕获事件
        public static void Init()
        {
#if (DEBUG)
            LogConfig.IsWriteFile = true;
#else
            LogConfig.IsWriteFile = false;
#endif
            
            var values = LogType.GetValues(typeof(LogType));
            var level = LogConfig.LogLevel;

            var index = 0;
            var flag = false;

            foreach (var value in values)
            {
                if (value.Equals(level))
                {
                    index = (int)value;
                    flag = true;
                }
                if (flag)
                {
                    LogLevelList.Add((LogType) value);
                }
            }
            
            Application.Current.Exit += App_Exit;
        }

        private static void App_Exit(object sender, ExitEventArgs e)
        {
            Dispose();
        }


        #region 写操作日志

        /// <summary>
        /// 写操作日志
        /// </summary>
        public static void Info(string log, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Write(LogType.Info, log, null, memberName, sourceFilePath, sourceLineNumber);
        }

        #endregion

        #region 写调试日志

        /// <summary>
        /// 写调试日志
        /// </summary>
        public static void Debug(string log,[CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Write(LogType.Debug, log, memberName, sourceFilePath, sourceLineNumber);
        }

        #endregion

        #region 写警告日志

        /// <summary>
        /// 写调试日志
        /// </summary>
        public static void Warn(string log)
        {
            Write(LogType.Warn, log);
        }

        #endregion

        #region Dispose

        public static void Dispose()
        {
            _logWriter?.Dispose();
        }

        #endregion


        #region 字段

        private static LogWriter _logWriter = new LogWriter();
        private static List<LogType> LogLevelList = new List<LogType>();

        #endregion

        #region 写错误日志

        /// <summary>
        /// 写错误日志
        /// </summary>
        public static void Error(string log, Exception ex, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Write(LogType.Error,string.IsNullOrEmpty(log) ? ex.ToString() : ex != null ? log + "：" + ex.ToString(): log, null, memberName, sourceFilePath, sourceLineNumber);
        }

        /// <summary>
        /// 写错误日志
        /// </summary>
        public static void Error(string log, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Write(LogType.Error, log, null, memberName, sourceFilePath, sourceLineNumber);
        }

        private static bool FindList<T>(List<T> list, string t)
        {
            foreach (var item in list)
            {
                if (item.ToString().Equals(t))
                {
                    return true;
                }
            }

            return false;
        }

        private static void Write(LogType type, string log, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            Write(type, log, null, memberName, sourceFilePath, sourceLineNumber);
        }

        private static void Write(LogType type, string log, Exception ex, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {

            var find = FindList(LogLevelList, type.ToString());
            if(!find)
            {
                return;
            }

            _logWriter.WriteLog(type, string.IsNullOrEmpty(log) ? ex?.ToString() : ex != null ? log + "：" + ex?.ToString() : log, memberName, sourceFilePath, sourceLineNumber);
        }

        #endregion
    }
}
