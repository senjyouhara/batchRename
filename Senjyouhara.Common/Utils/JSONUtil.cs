using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Senjyouhara.Common.Utils
{
    public class JSONUtil
    {

        public static string ToJSON(object data)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            // 设置日期格式
            settings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            // 忽略空值
            settings.NullValueHandling = NullValueHandling.Ignore;
            // 缩进
            settings.Formatting = Formatting.Indented;
            return JsonConvert.SerializeObject(data, settings);
        }

        public static T ToData<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

    }
}
