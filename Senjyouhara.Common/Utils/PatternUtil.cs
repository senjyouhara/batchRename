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

        public static List<string> GetPatternGroupResultFirst(string pattern, string input)
        {
            var result = GetPatternGroupResult(pattern, input);
            return result.Count > 0 ? result[0] : new List<string>() ;
        }
        public static List<List<string>> GetPatternGroupResult(string pattern, string input)
        {
            var result = new Regex(pattern).Matches(input);
            var list = new List<List<string>>();
            foreach (Match match in result)
            {
                var groups = match.Groups;
                var l = new List<string>();
                foreach (Group group in groups)
                {
                    if (group.Success)
                    {
                        l.Add(group.Value);
                    }
                }
                list.Add(l);
            }
            return list;
        }

        public static string GetPatternResultFirst(string pattern, string input)
        {
            var result = GetPatternResult(pattern, input);
            return result.Count > 0 ? result[0] : string.Empty;
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

    }
}
