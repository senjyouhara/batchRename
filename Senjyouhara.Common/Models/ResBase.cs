using System;
using System.Collections.Generic;
using System.Linq;
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

    }
}
