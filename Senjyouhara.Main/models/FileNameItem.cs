using PropertyChanged;
using Senjyouhara.Common.Utils;

namespace Senjyouhara.Main.models
{
    [AddINotifyPropertyChangedInterface]
    public class FileNameItem
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string PreviewFileName { get; set; }
        public string PreviewFilePath { get; set; }
        public string SubtitleFileName { get; set; }
        public string SuffixName { get; set; }

    }
}
