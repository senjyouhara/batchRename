using Senjyouhara.Common.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Common.Utils
{
    public class Log
    {

        static Log()
        {
            LogUtil.IsWriteFile= false;
        }

        public static void Debug(string msg, [CallerMemberName] string CallerMemberName = "",
            [CallerFilePath] string CallerFilePath = "",
            [CallerLineNumber] int CallerLineNumber = 0
            )
        {
            var str = "";
            //取得当前代码的命名空间    
            //父方法名
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
            System.Reflection.MethodBase mb = st.GetFrame(1).GetMethod();
            ////取得父方法命名空间    
            //str += mb.DeclaringType.Namespace + "\n";
            ////取得父方法类名    
            //str += mb.DeclaringType.Name + "\n";
            ////取得父方法类全名    
            //str += mb.DeclaringType.FullName + "\n";
            ////取得父方法名    
            //str += mb.Name + "\n";

            LogUtil.Debug($" [{CallerLineNumber} Line] [{mb.DeclaringType.FullName + (mb.Name.Equals(".ctor") ? "" : "." + mb.Name) }] " + msg);
        }

        public static void Debug(string msg, params object[] args)
        {
            LogUtil.Debug(string.Format(msg, args));
        }
        public static void Info(string msg, [CallerMemberName] string CallerMemberName = "",
            [CallerFilePath] string CallerFilePath = "",
            [CallerLineNumber] int CallerLineNumber = 0)
        {
            var str = "";
            //取得当前代码的命名空间    
            //父方法名
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
            System.Reflection.MethodBase mb = st.GetFrame(1).GetMethod();
            ////取得父方法命名空间    
            //str += mb.DeclaringType.Namespace + "\n";
            ////取得父方法类名    
            //str += mb.DeclaringType.Name + "\n";
            ////取得父方法类全名    
            //str += mb.DeclaringType.FullName + "\n";
            ////取得父方法名    
            //str += mb.Name + "\n";
            LogUtil.Info($" [{CallerLineNumber}] [{mb.DeclaringType.FullName + (mb.Name.Equals(".ctor") ? "" : "." + mb.Name)}] " + msg);
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

        public static void Error(string msg, Exception err, [CallerMemberName] string CallerMemberName = "",
            [CallerFilePath] string CallerFilePath = "",
            [CallerLineNumber] int CallerLineNumber = 0)
        {
            var str = "";
            //取得当前代码的命名空间    
            //父方法名
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
            System.Reflection.MethodBase mb = st.GetFrame(1).GetMethod();
            ////取得父方法命名空间    
            //str += mb.DeclaringType.Namespace + "\n";
            ////取得父方法类名    
            //str += mb.DeclaringType.Name + "\n";
            ////取得父方法类全名    
            //str += mb.DeclaringType.FullName + "\n";
            ////取得父方法名    
            //str += mb.Name + "\n";
            LogUtil.Error($" [{CallerLineNumber}] [{mb.DeclaringType.FullName + (mb.Name.Equals(".ctor") ? "" : "." + mb.Name)}] " + msg, err);

        }

    }
}
