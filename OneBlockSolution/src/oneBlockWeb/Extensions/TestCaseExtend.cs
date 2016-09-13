using blockPlayDataEntities;
using oneBlockWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace oneBlockWeb {
    public static class TestCaseExtend {

        public static testCase bodyModel(this TestCase _testCase) {
            return JsonConvert.DeserializeObject<testCase>(_testCase.Body);
        }

        public static string toBodyString(this testCase _testCaseModel) {
            return JsonConvert.SerializeObject(_testCaseModel);
        }

        public static void ConvertForRun(this testCase _testCaseModel, blockPlayDBContext db, Dictionary<string, string> attrs) {
            setAttrs(_testCaseModel.steps,attrs);
            
            _testCaseModel.blockChannge(db);
        }

        /// <summary>
        /// 将参数替换到到步骤中
        /// </summary>
        public static void setAttrs(List<step> steps, Dictionary<string, string> attrs) {

            if (attrs == null || attrs.Count == 0) return;
            foreach (var step in steps) {
                string[] keys = step.attrs.Keys.ToArray();
                for (int i = 0; i < keys.Length; i++) {
                    string TempValue = step.attrs[keys[i]];
                    if (string.IsNullOrEmpty(TempValue)) continue; //空值退出
                    //循环替换
                    foreach (var attr in attrs) {
                        //Replace替换
                        TempValue = TempValue.Replace("{" + attr.Key + "}", attr.Value);
                    }
                    step.attrs[keys[i]] = TempValue;
                }
            }

        }


        /// <summary>
        /// 替换block节点
        /// </summary>
        public static void blockChannge(this testCase _testCaseModel, blockPlayDBContext db) {

            
            var blocksteps = _testCaseModel.steps.Where(t => t.blockID > 0).ToList();

            foreach (var bs in blocksteps) {
                var _blockstep = db.BlockStep.FirstOrDefault(t => t.Id == bs.blockID);

                var insertSteps =  _blockstep.blockSteps();
                setAttrs(insertSteps, bs.attrs);

                var index = _testCaseModel.steps.IndexOf(bs);
                _testCaseModel.steps.Remove(bs);
                _testCaseModel.steps.InsertRange(index, insertSteps);
            }

        }

        /// <summary>
        /// 获取参数列表
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> getCustomParamet(this testCase _testCaseModel) {
            Dictionary<string, string> Paramets = new Dictionary<string, string>();

            Regex reg = new Regex("{.*?}");
            //获得定义的变量
            foreach (var step in _testCaseModel.steps) {
                if(step.attrs ==null ) continue;
                foreach (var attr in step.attrs) {
                    if (attr.Value == null) continue;
                    MatchCollection matches = reg.Matches(attr.Value); // 在字符串中匹配
                    foreach (Match match in matches) {
                        string name = match.Value.Substring(1, match.Value.Count() - 2);
                        //确认没有重复的参数
                        if (!Paramets.ContainsKey(name)) {
                            Paramets.Add(name, "");
                        }
                    }
                }
            }

            return Paramets;
        }
    }


    //案例相关

    public class testCase {
        
        public List<step> steps { get; set; }

        
    }

    public class step {
        public int spaceID { get; set; }
        public int blockID { get; set; }
        public string name { get; set; }
        public string describe { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public Dictionary<string, string> attrs { get; set; }
    }


}
