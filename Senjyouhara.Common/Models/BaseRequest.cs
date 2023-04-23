using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Common.Models
{
    public class BaseRequest
    {
        public Method Method { get; set; }

        public string Url { get; set; }

        public Dictionary<string, string> Files { get; set; }

        public object Data { get; set; }
        public Dictionary<string, string> QueryParams { get; set; }

        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        public HttpClientOptions Options { get; set; } = new HttpClientOptions();

    }
}
