using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Main.Comparer
{

        public class FileComparer : IComparer<string>
        {
        //引入API文件:Shlwapi.dlL
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        static extern int StrCmpLogicalW(String x, String y);
        public int Compare(string str_1_ForCompare, string str_2_ForCompare)
            {
                return StrCmpLogicalW(str_1_ForCompare, str_2_ForCompare);
            }
        }
}
