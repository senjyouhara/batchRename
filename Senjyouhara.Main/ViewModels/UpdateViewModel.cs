using Caliburn.Micro;
using PropertyChanged;
using Senjyouhara.Common.Models;
using Senjyouhara.Common.Utils;
using Senjyouhara.Main.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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
        public bool IsForceUpdate { get; set; } = UpdateConfig.IsForceUpdate;

        private UpdateConfig.UpdateDataEntity _updateInfo;

        public UpdateViewModel()
        {
            Init();
        }

        private async void Init()
        {
            Tips = "正在检查更新中，请稍后……";
            _updateInfo = await UpdateConfig.GetUpdateData();
            if (_updateInfo == null)
            {
                Tips = "检查更新失败！";
            } else
            {
                Status = "tipsUpdate";
                Tips = $"当前版本为{AppConfig.Version}, 最新版为{_updateInfo.Version}，需要进行更新！";
            }
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
                var result = await DownloadFile("https://test-1253492636.cos.ap-guangzhou.myqcloud.com/%2Fcaselib/upload/2023/4/7/455219546417081906/336.sdpc?q-sign-algorithm=sha1&q-ak=AKIDmSxjKlV4wc9V6adyscDdruvoW2Nfb073&q-sign-time=1682229839%3B1682237039&q-key-time=1682229839%3B1682237039&q-header-list=host&q-url-param-list=&q-signature=dfeb3b4ee56840c976974dadd68febf57d77003d", UpdateConfig.UpdateFilePath + @"/update.7z");
                //await DownloadFile(updateInfo.Path, UpdateConfig.UpdateFilePath);
                if (result)
                {
                    Status = "unzip";
                    //RunUpdateExe();
                }
                else
                {
                    Status = "downloadingError";
                    Tips = "检查更新失败！";
                }

            }
        }

        public void CloseModal()
        {
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
                    while (realReadLen > 0)
                    {
                        fileStream.Write(read, 0, realReadLen);
                        progressBarValue += realReadLen;
                        double percent = (Math.Round(progressBarValue / size, 6) * 100);
                        if (Application.Current == null)
                        {
                            TryCloseAsync();
                            break;
                        } else
                        {
                            Application.Current.Dispatcher.BeginInvoke(() =>
                            {
                                Percent = percent;
                                DownloadFileSize = Math.Round(progressBarValue / 1024 / 1024, 2);
                            });
                        }
                        
                        realReadLen = netStream.Read(read, 0, read.Length);
                    }
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
