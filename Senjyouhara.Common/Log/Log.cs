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

        // 需要在App.xaml.cs 定义一些异常捕获事件
        static Log()
        {
            LogConfig.IsWriteFile = true;
            Application.Current.Exit += App_Exit;
        }

        private static void App_Exit(object sender, ExitEventArgs e)
        {
            Dispose();
        }

        public static void Debug(string msg)
        {
            LogUtil.Debug(msg);
        }

        public static void Debug(string msg, params object[] args)
        {
            LogUtil.Debug(string.Format(msg, args));
        }
        public static void Info(string msg)
        {
            LogUtil.Info(msg);
        }
        public static void Info(string msg, params object[] args)
        {
            LogUtil.Info(string.Format(msg, args));
        }


        public static void Error(string msg)
        {
            Error(msg, null as Exception);
        }

        public static void Error(string msg, params object[] args)
        {
            Error(string.Format(msg, args), null as Exception);
        }

        public static void Error(string msg, Exception err)
        {
            LogUtil.Error(msg, err);
        }

        public static void Dispose()
        {
            LogUtil.Dispose();
        }
    }
}
