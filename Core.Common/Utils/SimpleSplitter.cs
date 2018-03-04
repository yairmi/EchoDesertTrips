using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Utils
{
    public class SimpleSplitter
    {
        public static string[] Split(string source)
        {
            string[] separators = { ",", ".", "!", "?", ";", ":" };
            return source.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] Split(string source, string[] separators)
        {
            return source.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }

        //public static string DictionaryToString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        //{
        //    return "{" + string.Join(",", dictionary.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}";
        //}


    }
}
