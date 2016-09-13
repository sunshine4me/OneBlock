using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace oneBlockWeb {
    public static class otherExtend
    {
        /// <summary>
        /// 合并两个 Dictionary
        /// </summary>
        public static void merge(this Dictionary<string,string> _dic, Dictionary<string, string> _dic2) {
            foreach (var d in _dic2) {
                if (_dic.ContainsKey(d.Key))
                    _dic[d.Key] = d.Value;
            }
        }

       

    }
}
