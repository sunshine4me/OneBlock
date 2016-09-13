using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace oneBlockWeb.ViewModels
{
    /// <summary>
    /// 案例List tree数据
    /// </summary>
    public class testCaseTreeNode
    {
        public int id { get; set; }
        public string name { get; set; }
        public int type { get; set; }
    }
    

    public class caseTabModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<step> steps { get; set; }
        public List<SelectListItem> spaceSelect { get; set; }
    }




   
    
}
