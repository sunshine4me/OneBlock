using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace oneBlockWeb.ViewModels
{
    public class stepModel {
        public int blockID { get; set; }
        public int spaceID { get; set; }
        public string name { get; set; }
        public string describe { get; set; }
        public Dictionary<string, string> attrs { get; set; }
    }

    public class spaceStepEditModel {

        public stepModel caseStep { get; set; }
        public spaceStep spaceStep { get; set; }

        public string spaceName { get; set; }


    }

    public class StepEditModel {

       public List<StepEditAttrModel> attrs { get; set; }

        public string describe { get; set; }

        public string fullName{ get; set; }
    }

    public class StepEditAttrModel {

        public string name { get; set; }
        public string describe { get; set; }
        public string value { get; set; }
        public Dictionary<string, string> list { get; set; }
        
    }

}
