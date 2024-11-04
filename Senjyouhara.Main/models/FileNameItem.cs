using System.Collections.Generic;
using PropertyChanged;
using Senjyouhara.Common.Utils;
using System.ComponentModel.DataAnnotations;

namespace Senjyouhara.Main.models
{
    [AddINotifyPropertyChangedInterface]
    public class FileNameItem
    {

        //[Required(ErrorMessage = "不能为空")]
        //[MinLength(2, ErrorMessage = "不能少于两个字符")]
        //[MaxLength(10, ErrorMessage = "最大10个字符")]
        //[Range(3, 120, ErrorMessage = "范围需在3-120岁之间")]
        //[EmailAddress(ErrorMessage = "邮箱地址不合法")]
        //[RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "手机号不正确")]
        public string Uid { get; set; }
        public long Timestamp { get; set; }
        public string FilePath { get; set; }
        public string OriginFileName { get; set; }
        public string FileName { get; set; }
        public string PreviewFileName { get; set; }
        public string PreviewFilePath { get; set; }
        public string SubtitleFileName { get; set; }
        public string SuffixName { get; set; }

        public List<string> SubUidList { get; set; } = new();

    }
}
