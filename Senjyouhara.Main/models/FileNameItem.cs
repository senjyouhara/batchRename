using Senjyouhara.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Main.models
{
    public class FileNameItem:NotifycationObject
    {

        private string _FilePath;

        public string FilePath
        {
            get { return _FilePath; }
            set { _FilePath = value; RaisePropertyChanged(nameof(FilePath)); }
        }

        private string _FileName;

        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; RaisePropertyChanged(nameof(FileName)); }
        }

        private string _PreviewFileName;

        public string PreviewFileName
        {
            get { return _PreviewFileName; }
            set { _PreviewFileName = value; RaisePropertyChanged(nameof(PreviewFileName)); }
        }

        private string _PreviewFilePath;

        public string PreviewFilePath
        {
            get { return _PreviewFilePath; }
            set { _PreviewFilePath = value; RaisePropertyChanged(nameof(PreviewFilePath)); }
        }

        
        private string _SubtitleFileName;

        public string SubtitleFileName
        {
            get { return _SubtitleFileName; }
            set { _SubtitleFileName = value; RaisePropertyChanged(nameof(SubtitleFileName)); }
        }

        private string _SuffixName;

        public string SuffixName
        {
            get { return _SuffixName; }
            set { _SuffixName = value; RaisePropertyChanged(nameof(SuffixName)); }
        }

    }
}
