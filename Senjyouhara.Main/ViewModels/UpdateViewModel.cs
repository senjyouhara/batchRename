using Caliburn.Micro;
using PropertyChanged;
using Senjyouhara.Common.Models;
using Senjyouhara.Common.Utils;
using Senjyouhara.Main.Config;
using Senjyouhara.Main.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Shell;
using System.Windows.Threading;

namespace Senjyouhara.Main.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    internal class UpdateViewModel : Screen
    {
        public string Status { get; set; } = "check";
        public double Percent { get; set; } = 0;
        public double FileTotalSize { get; set; } = 0;
        public double DownloadFileSize { get; set; } = 0;
        public int FileTotal { get; set; } = 1;
        public string DownloadFileName { get; set; } = "update.7z";
        public int DownloadFileNumber { get; set; } = 1;
        public string Tips { get; set; } = "文件下载失败！";

        public IDelegateCommand CloseCommand { get; set; }

        public Visibility CancelVisibility { get; set; } = UpdateConfig.IsForceUpdate ? Visibility.Collapsed : Visibility.Visible;

        private UpdateConfig.UpdateDataEntity _updateInfo;

        private TaskbarItemInfo taskBarInfo;

        public UpdateViewModel()
        {
            taskBarInfo =  ((ShellView) Application.Current.MainWindow).TaskBarInfo;
            Init();
        }


        private void Init()
        {
            Tips = "正在检查更新中，请稍后……";
            Application.Current.Dispatcher.Invoke(async () =>
            {

                _updateInfo = UpdateConfig.UpdateInfo;
                if (UpdateConfig.UpdateInfo == null)
                {
                    _updateInfo = await UpdateConfig.GetUpdateData();
                }
                if (_updateInfo == null)
                {
                    Tips = "检查更新失败！";
                }
                else
                {
                    Status = "tipsUpdate";
                    Tips = $"当前版本为{AppConfig.Version}, 最新版为{_updateInfo.Version}，需要进行更新！";
                }
            });
        }

        public async void StartUpdate()
        {

            Status = "downloading";
            // 则需要更新
            if (_updateInfo.Version != AppConfig.Version)
            {
                if (!Directory.Exists(UpdateConfig.UpdateFilePath))
                {
                    Directory.CreateDirectory(UpdateConfig.UpdateFilePath);
                }
                
                //var result = await DownloadFile("https://patchwiki.biligame.com/images/re0/3/3f/h8yxnkq94sis8p5vn1i8x4lbir2fwo1.png", UpdateConfig.UpdateFilePath + @"/update.7z");
                var result = await DownloadFile(_updateInfo.Path, UpdateConfig.UpdateFilePath + @"/update.7z");
                if (result)
                {
                    Status = "unzip";
                    RunUpdateExe();
                }
                else
                {
                    Status = "downloadingError";
                    Tips = "下载更新文件失败！";
                }

            }
        }


        private void TabbarProcess()
        {
            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(0.5);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
           taskBarInfo.ProgressValue = Percent / 100;
        }

        public void CloseModal()
        {
            CloseCommand?.Execute();
            TryCloseAsync();
        }

        public async Task<bool> DownloadFile(string url, string localpath)
        {
            try
            {
                WebResponse response = null;
                //获取远程文件
                WebRequest request = WebRequest.Create(url);
                response = request.GetResponse();
                if (response == null) return false;


                double size = response.ContentLength;
                FileTotalSize = Math.Round(double.Parse(response.ContentLength.ToString()) / 1024 / 1024, 2);
                //读远程文件的大小
                //下载远程文件
                return await Task.Run(() =>
                {
                    Stream netStream = response.GetResponseStream();
                    Stream fileStream = new FileStream(localpath, FileMode.Create);
                    byte[] read = new byte[1024];
                    double progressBarValue = 0;
                    int realReadLen = netStream.Read(read, 0, read.Length);
              
                    Application.Current.Dispatcher.BeginInvoke(() => {
                        taskBarInfo.ProgressState = TaskbarItemProgressState.Normal;
                        TabbarProcess();
                    });
                    while (realReadLen > 0)
                    {
                        fileStream.Write(read, 0, realReadLen);
                        progressBarValue += realReadLen;
                        double percent = (Math.Round(progressBarValue / size, 6) * 100);
                        if (Application.Current == null)
                        {
                            CloseModal();
                            break;
                        } else
                        {
                            Application.Current.Dispatcher.BeginInvoke(() =>
                            {
                                Percent = percent;
                                DownloadFileSize = Math.Round(progressBarValue / 1024 / 1024, 2);
                            });
                        }

                        Thread.Sleep(10);
                        realReadLen = netStream.Read(read, 0, read.Length);
                    }


                    Application.Current.Dispatcher.Invoke(() => {
                        taskBarInfo.ProgressState = TaskbarItemProgressState.None;
                        FlashWindow.Flash(Application.Current.MainWindow, 0);
                    });

                    netStream.Close();
                    fileStream.Close();

                    return true;
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("NetError on DownloadFile");
            }

            return false;
        }


        public void RunUpdateExe()
        {
            Debug.WriteLine(AppConfig.Name);
            Debug.WriteLine(AppConfig.Version);
            Process.Start(Directory.GetCurrentDirectory() + @$"\update.exe --name={AppConfig.Name} --version ={AppConfig.Version}");
        }

    }
}
