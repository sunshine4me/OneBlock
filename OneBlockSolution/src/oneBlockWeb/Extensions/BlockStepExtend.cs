using blockPlayDataEntities;
using oneBlockWeb.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace oneBlockWeb {
    public static class BlockStepExtend {
        public static List<step> blockSteps(this BlockStep _blockStep) {
           
            return JsonConvert.DeserializeObject<testCase>(_blockStep.Body).steps;
          
        }

        public static Dictionary<string,string> blockAttrs(this BlockStep _blockStep) {

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(_blockStep.Attrs);

        }

    }


  

}
