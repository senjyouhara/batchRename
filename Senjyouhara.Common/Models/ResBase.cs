using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Common.Models
{
    public class ResBase<T>
    {
        public bool Success { get; set; }

        public int ResultCode { get; set; } = 101;

        public string Msg { get; set; } = "请求失败";

        public T Data { get; set; }

        public Dictionary<string, object> ExpandMap { get; set; } = new Dictionary<string, object>();

        public override string ToString()
        {
            var s = new StringBuilder();
            s.Append("ResBase { ")
                .Append(nameof(Success) + " ")
            .Append(Success + " ,")
            .Append(nameof(ResultCode) + " ")
                .Append(ResultCode + " ,")
                .Append(nameof(ExpandMap) + " ")
                .Append(ExpandMap + " ,")
                .Append(nameof(Msg) + " ")
                .Append(Msg + " ,")
                .Append(nameof(Data) + " ")
                .Append(Data + " }");
            return s.ToString();
        }
    }
}
