using LitJson;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Senjyouhara.Common.Utils
{
    public class JSONUtil
    {

        public static string ToJSON(object data)
        {
            return ToJSON(data, true);
        }

        public static string ToJSON(object data, bool IsLowercase)
        {
            //JsonSerializerSettings settings = new JsonSerializerSettings()
            //{
            //    ContractResolver = new CamelCasePropertyNamesContractResolver()
            //};
            //// 设置日期格式
            //settings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            //// 忽略空值
            //settings.NullValueHandling = NullValueHandling.Ignore;
            //// 缩进
            //settings.Formatting = Formatting.Indented;
            //return JsonConvert.SerializeObject(data, settings);

            LitJson.JsonWriter jw = new LitJson.JsonWriter();
            jw.PrettyPrint = true;
            jw.IndentValue = 4;//缩进空格个数
            JsonMapper.ToJson(data, jw);

            var str = jw.ToString();
            str = Regex.Unescape(str);
            if (str.StartsWith("\r\n"))
            {
                str = str.Substring(str.IndexOf("\r\n") + 2);
            }
            str = Regex.Replace(str, @"(?<="")([0-9]{2})\/([0-9]{2})\/([0-9]{4}) ([0-9]{2}:[0-9]{2}:[0-9]{2})(?="")", "$3-$1-$2 $4");
            if (IsLowercase)
            {
                var list = PatternUtil.GetPatternResult(@"\""[A-Z](.*)\""\s:", str);
                if(list?.Count> 0)
                {
                    list.ForEach(item =>
                    {
                        var value = Regex.Replace(item, @"^\""", "");
                        value = Regex.Replace(value, @"\"".:$", "");
                        if(value.Length > 0)
                        {
                            value = value.Substring(0, 1).ToLower() + value.Substring(1);
                        }
                        str = str.Replace(item, $@"""{value}"" :");
                    });
                }
            }

            str = Regex.Replace(str, @"("".+"")\s+:", "$1:");
            return str;
        }

        // 需要捕获异常， 有可能json格式不正确
        public static T ToData<T>(string json)
        {
            //return JsonConvert.DeserializeObject<T>(json);

            var list = PatternUtil.GetPatternResult(@"\""(.*)\""\s*:", json);
            if (list?.Count > 0)
            {
                list.ForEach(item =>
                {
                    var value = Regex.Replace(item, @"^\""", "");
                    value = Regex.Replace(value, @"\"".*:$", "");
                    if (value.Length > 0)
                    {
                        value = value.Substring(0, 1).ToUpper() + value.Substring(1);
                    }
                    json = json.Replace(item, $@"""{value}"":");
                });
            }
            try
            {
                return JsonMapper.ToObject<T>(json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw ex;
            }
        }

    }
}
