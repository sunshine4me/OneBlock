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
using System.IO;
using NETCore.Extensions.Excel;
using System.Xml.Linq;
using NETCore.Extensions.Excel.Infrastructure;

namespace oneBlockWeb {
    public static class TestCaseExtend {

        public static testCase bodyModel(this TestCase _testCase) {
            return JsonConvert.DeserializeObject<testCase>(_testCase.Body);
        }

        public static string toBodyString(this testCase _testCaseModel) {
            return JsonConvert.SerializeObject(_testCaseModel);
        }

        /// <summary>
        /// 执行前转换
        /// </summary>
        /// <param name="_testCaseModel"></param>
        /// <param name="db"></param>
        /// <param name="attrs"></param>
        public static void ConvertForRun(this testCase _testCaseModel, blockPlayDBContext db, Dictionary<string, string> attrs) {
            setAttrs(_testCaseModel.steps,attrs);
            
            _testCaseModel.blockChannge(db);
            //删除不启用的组件
            var rsteps = _testCaseModel.steps.Where(t => t.attrs.FirstOrDefault(a => a.Key == "是否启用").Value=="false").ToList();
            foreach (var s in rsteps) {
                _testCaseModel.steps.Remove(s);
            }
        }

        public static MemoryStream getSceneExcelMS(List<int> ids, blockPlayDBContext db) {

            MemoryStream ms = new MemoryStream(1024 * 1024);

            using (ExcelStream _ExcelStream = new ExcelStream()) {
                _ExcelStream.Create(ms);
                _ExcelStream.ReNameSheet(1, "配置表");
                var config = _ExcelStream.LoadSheet(1);


                var row = config.CreateRow(1);

                var cell1 = row.CreateCell(1);
                cell1.value = "案例名";
                config.SetColumnWidth(1, 50);

                var cell2 = row.CreateCell(2);
                cell2.value = "案例ID";

                var cell3 = row.CreateCell(3);
                cell3.value = "参数数量";

                foreach (int id in ids) {

                    var tc = (from t in db.TestCase
                                      where t.Id == id
                                      select t).FirstOrDefault();
                    if (tc == null) continue;
                    var caseSheet = _ExcelStream.CreateSheet(tc.Id.ToString());
                    creatCaseSheet(caseSheet, config, tc);
                    caseSheet.SaveChanges();
                }

                config.SaveChanges();


            }
            ms.Position = 0;
            return ms;
        }



   
        private static void creatCaseSheet(ISheet caseSheet, ISheet Config, TestCase tc) {
            var pdl = tc.bodyModel().getCustomParamet();

            IRow row = caseSheet.CreateRow(1);

            ICell cell1 = row.CreateCell(1);
            cell1.value = "案例名";
            caseSheet.SetColumnWidth(1, 30); //宽度

            foreach (var str in pdl) {
                ICell cell = row.CreateCell(row.LastCellNum + 1);
                caseSheet.SetColumnWidth(row.LastCellNum, 10); //宽度
                cell.value = str.Key;
            }

            IRow DemoRow = caseSheet.CreateRow(2);
            DemoRow.CreateCell(1).value = tc.Name;



            
            IRow c_row = Config.CreateRow(Config.LastRowNum + 1);
            ICell c_cell1 = c_row.CreateCell(1);
            c_cell1.value = tc.Name;



            //创建一个超链接对象
            HSSFHyperlink link = new HSSFHyperlink();
            // strTableName 这个参数为 sheet名字 A1 为单元格 其他是固定格式
            link.Address = "'" + tc.Id + "'!A1";
            //设置 cellTableName 单元格 的连接对象
            c_cell1.Hyperlink = link;



            ICell c_cell2 = c_row.CreateCell(2);
            c_cell2.value = tc.Id.ToString();

            ICell c_cell3 = c_row.CreateCell(3);
            c_cell3.value = pdl.Count.ToString();

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
