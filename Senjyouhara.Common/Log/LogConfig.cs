using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Common.Log
{
    public class LogConfig
    {

        //是否写入日志文件
        public static bool IsWriteFile { get; set; } = true;
        //日志级别，只有该级别之上的才会打印写入
#if(DEBUG)
        public static LogType LogLevel { get; set; } = LogType.Debug;
#else
        public static LogType LogLevel { get; set; } = LogType.Info;
#endif

    }
}
