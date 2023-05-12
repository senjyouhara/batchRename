using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Senjyouhara.Main.Config
{
    internal class UpdateConfig
    {

        public class UpdateDataEntity
        {
            public string Version { get; set; }
            public string Path { get; set; }
        }

        // 是否开启自动更新
        public static bool IsEnableUpdate = false;
        // 是否强制更新，如果为否则可以用户取消更新
        public static bool IsForceUpdate = false;
        // 下载更新文件路径
        public static string UpdateFilePath = Directory.GetCurrentDirectory().Replace("\\", "/") + @"/update";
        public static UpdateDataEntity UpdateInfo;

        // 请求最新版本信息相关接口
        public static async Task<UpdateDataEntity> GetUpdateData()
        {
            return await Task.Run(() => {
                Thread.Sleep(1000);
                UpdateInfo = new UpdateDataEntity() { Path = "", Version = "1.1.0.0"};
                return UpdateInfo;
            });
        }


    }
}
