using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Senjyouhara.Common.Utils
{
    public class PatternUtil
    {
        public static bool IsMatch(string pattern, string input)
        {
            return new Regex(pattern).IsMatch(input);
        }
        public static List<string> GetPatternResult(string pattern, string input)
        {
            var result = new Regex(pattern).Matches(input);
            var list = new List<string>();
            foreach (Match match in result)
            {
                if (match.Success)
                {
                    list.Add(match.Value);
                }
            }
            return list;
        }
        public static string GetFirstPatternResult(string pattern, string input)
        {
            var result = GetPatternResult(pattern, input);
            return result.Count > 0 ? result[0] : null;
        }

    }
}
