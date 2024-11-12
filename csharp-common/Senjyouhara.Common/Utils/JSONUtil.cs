using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Senjyouhara.Common.Utils
{
    public class JSONUtil
    {
        public static string ToJSON(object data)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            // 设置日期格式
            settings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            // 忽略空值
            settings.NullValueHandling = NullValueHandling.Ignore;
            // 缩进
            settings.Formatting = Formatting.Indented;

            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return JsonConvert.SerializeObject(data, settings);
        }


        public static T InnerJSSONPath<T>(string jsonPath)
        {
            var fileUrl = new Uri(@$"{jsonPath}", UriKind.Relative);
            var src = Application.GetResourceStream(fileUrl);
            StreamReader sr = new StreamReader(src.Stream);
            var json = sr.ReadToEnd();
            return ToData<T>(json);
        }

        // 需要捕获异常， 有可能json格式不正确
        public static T ToData<T>(string json)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            // 设置日期格式
            settings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            // 忽略空值
            settings.NullValueHandling = NullValueHandling.Ignore;
            // 缩进
            settings.Formatting = Formatting.Indented;
// 解决乱码
            settings.StringEscapeHandling = StringEscapeHandling.EscapeNonAscii;
            // 大小写映射
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}