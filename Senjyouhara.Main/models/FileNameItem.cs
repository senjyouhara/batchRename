using PropertyChanged;
using Senjyouhara.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Main.models
{
    [AddINotifyPropertyChangedInterface]
    public class FileNameItem:NotifycationObject
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string PreviewFileName { get; set; }
        public string PreviewFilePath { get; set; }
        public string SubtitleFileName { get; set; }
        public string SuffixName { get; set; }

    }
}
