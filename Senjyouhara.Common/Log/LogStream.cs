using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Common.Log
{
    internal class LogStream
    {
        public FileStream CurrentFileStream { get; set; }

        public int CurrentArchiveIndex { get; set; }

        public long CurrentFileSize { get; set; }

        public string CurrentLogFileName { get; set; }

        public string CurrentLogFilePath { get; set; }

        public string CurrentLogFileDir { get; set; }
    }
}
