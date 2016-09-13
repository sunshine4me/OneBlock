using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace oneBlockWeb {
    public class easyUI_datagrid<T>
    {
        public int total { get; set; }

        public List<T> rows { get; set; }
    }
}
