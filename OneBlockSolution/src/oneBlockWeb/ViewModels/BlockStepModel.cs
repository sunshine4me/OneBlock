using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace oneBlockWeb.ViewModels
{
    public class BlockStepAddModel {
        public string blockName { get; set; }
        public Dictionary<string,string>  attrs{ get; set; }

        public testCase testCase { get; set; }
    }

    public class blockTabModel {
        public int id { get; set; }
        public string name { get; set; }
        public List<step> steps { get; set; }
        public List<SelectListItem> spaceSelect { get; set; }
    }
}
