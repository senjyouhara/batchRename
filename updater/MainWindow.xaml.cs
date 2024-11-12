using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace updater
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        public static string updatepath = Directory.GetCurrentDirectory() + @"/update"; //下载的更新文件路径

        public static string localpath = Directory.GetCurrentDirectory(); //文件路径

        public Dictionary<string, string> Args = new(); 

        public MainWindow()
        {
            var args = Environment.GetCommandLineArgs().Skip(1).ToList();

            if(args.Count > 0 )
            {
                args.ForEach(v =>
                {
                    if(Regex.IsMatch(v, @"--[a-zA-Z]+=.+"))                    
                    {
                        var key = Regex.Match(v, @"(?<=--)[a-zA-Z]+(?==)");
                        var value = Regex.Match(v, @"(?<=--[a-zA-Z]+=).+");
                        Args.Add(key.Value, value.Value);
                    }
                });
            } else
            {
                this.Close();
            }

            if(Args.Count == 0)
            {
                this.Close();
            }

            if (string.IsNullOrEmpty(Args["name"]))
            {
                Debug.WriteLine("name字段不在args入参");
                this.Close();
            }

            Init();
        }

        private void Init()
        {
            System.Diagnostics.Process[] pro = Process.GetProcesses();//获取已开启的所有进程
            //遍历所有查找到的进程
            for (int i = 0; i < pro.Length; i++)
            {
                //判断此进程是否是要查找的进程
                if (pro[i].ProcessName.ToString() == Args["name"]+".exe")
                {
                    pro[i].Kill();//结束进程
                    Thread.Sleep(100);
                }
            }

            if (File.Exists(updatepath + @"\update.7z"))
            {
                if (!Directory.Exists(@$"{localpath}/bak"))
                {
                    Directory.CreateDirectory(@$"{localpath}/bak");
                }
                DirectoryInfo d = new DirectoryInfo(localpath);
                FileSystemInfo[] fsinfos = d.GetFileSystemInfos();
                foreach (FileSystemInfo fsinfo in fsinfos)
                {
                    if (!fsinfo.Name.Equals("updater.exe") &&
                        (!Directory.Exists(fsinfo.FullName)))
                    {
                        File.Copy(fsinfo.FullName, @$"{localpath}/bak/{fsinfo.Name}", true);
                    }
                }
                try
                {
                    var err = RunSyncProcess(localpath, @"7z.exe -y x " + "\"" + updatepath + @"\update.7z" + "\"" + @" -o" + "\"" + localpath + "\"");
                }
                catch (Exception ex)
                {

                    if (ex.Message.Contains("7z"))
                    {
                        MessageBox.Show("文件更新失败，目录下7z.exe或7z.dll不存在", "更新失败");
                    }
                    else
                    {
                        MessageBox.Show("文件更新失败，请检查是否对应文件被占用", "更新失败");
                    }

                    DirectoryInfo bak = new DirectoryInfo($@"{localpath}/bak");
                    FileSystemInfo[] bakInfos = bak.GetFileSystemInfos();
                    foreach (FileSystemInfo info in bakInfos)
                    {
                        try
                        {
                            File.Copy(info.FullName, @$"{localpath}/{info.Name}", true);
                        }
                        catch
                        {
                        }
                    }
                    Directory.Delete($@"{localpath}/bak", true);
                    Directory.Delete(updatepath, true);
                    Close();
                    return;
                }

                Directory.Delete(updatepath, true);
            }

            Thread.Sleep(100);
            Process.Start(Directory.GetCurrentDirectory() + @$"\{Args["name"]}.exe");
            Close();
        }

        public static string RunSyncProcess(string clipath, string common)
        {
            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo("cmd")
                {
                    WorkingDirectory = clipath,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };

            proc.Start();
            proc.StandardInput.WriteLine(common);
            proc.StandardInput.WriteLine("exit");
            proc.WaitForExit();

            string outputstr = proc.StandardOutput.ReadToEnd();
            string error = proc.StandardError.ReadToEnd();
            if(!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }
            return outputstr;
        }

        /// <summary>
        /// 获取目录下所有文件
        /// </summary>
        /// <param name = "path" > 指定目录 </ param >
        /// < returns ></ returns >
        public static string[] GetAllFileInFolder(string path)
        {
            ArrayList array = new ArrayList();
            DirectoryInfo d = new DirectoryInfo(path);
            FileSystemInfo[] fsinfos = d.GetFileSystemInfos();
            foreach (FileSystemInfo fsinfo in fsinfos)
            {
                if (fsinfo is DirectoryInfo)
                {
                    string[] _temp = GetAllFileInFolder(fsinfo.FullName);
                    foreach (var item in _temp)
                    {
                        array.Add(item);
                    }
                }
                else
                {
                    array.Add(fsinfo.FullName);
                }
            }
            return (string[])array.ToArray(typeof(string));
        }
    }
}
