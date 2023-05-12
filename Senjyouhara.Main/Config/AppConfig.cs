using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Senjyouhara.Main.Config
{
    public class AppConfig
    {
        public static string Name = "";
        public static string Version = "";
        static AppConfig()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName thisAssemName = assembly.GetName();
            Name = thisAssemName.Name;
            Version ver = thisAssemName.Version;
            //Version = $"{ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}";
            Version = $"{ver.Major}.{ver.Minor}.{ver.Build}";
        }
    }
}
