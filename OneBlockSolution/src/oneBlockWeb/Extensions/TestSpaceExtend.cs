using blockPlayDataEntities;
using oneBlockWeb.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace oneBlockWeb {
    public static class TestSpaceExtend
    {
        public static testSapce spaceModel(this TestSpace _testSpace)
        {
            var md = new testSapce();
            md.id = _testSpace.Id;
            md.steps =  JsonConvert.DeserializeObject<List<spaceStep>>(_testSpace.SapceData);
            return md;
        }

        /// <summary>
        /// 将space对象转成可用的 step对象
        /// </summary>
        public static List<step> toSteplList(this testSapce _testSpaceModel) {
            var str = new List<step>();
            foreach(var s in _testSpaceModel.steps) {
                var m = new step();
                m.name = s.name;
                m.spaceID = _testSpaceModel.id;
                m.describe = s.describe;
                m.attrs = s.attrs.ToDictionary(key => key.name, value => value.defValue);
                str.Add(m);
            }
            return str;
        }
        /// <summary>
        /// 获取 spaceList
        /// </summary>
        public static List<SelectListItem> spaceSelectList(this blockPlayDBContext _db ,int userID) {
            
            var sps = from t in _db.TestSpace
                      where t.UserId==1 && t.UserId != userID
                      select new {
                          spacename = t.Name,
                          userID = t.UserId,
                          username = t.User.Username,
                          id = t.Id
                      };

            var mysps = from t in _db.TestSpace
                      where t.UserId == userID 
                      select new {
                          spacename = t.Name,
                          userID = t.UserId,
                          username = t.User.Username,
                          id = t.Id
                      };


            List<SelectListItem> list = new List<SelectListItem>();
            

            SelectListGroup slg = new SelectListGroup { Name = "其他人的TestSpace" };
            foreach (var s in sps) {
                    list.Add(new SelectListItem { Text = $"{s.spacename}({s.username})", Value = s.id.ToString(), Group = slg });
            }

            SelectListGroup myslg = new SelectListGroup { Name = "你的TestSpace" };
            foreach (var s in mysps) {
                    list.Add(new SelectListItem { Text = s.spacename, Value = s.id.ToString(), Group = myslg });
                
            }

            list.Add(new SelectListItem { Text = "你的模块", Value = "0" ,Group = new SelectListGroup { Name = "BlockStep" } });

            return list;
        }

        /// <summary>
        /// 获取 spaceList
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="steps">案例步骤用于识别使用哪个框架</param>
        public static List<SelectListItem> spaceSelectList(this blockPlayDBContext _db, int userID,List<step> steps) {

            var list = _db.spaceSelectList(userID);

            //尝试获取默认的space
            var stp = steps.FirstOrDefault(t => t.spaceID > 0);
            int defSpaceID = stp == null ? 0 : stp.spaceID;

            var selectItem = list.FirstOrDefault(t => t.Value == defSpaceID.ToString());
            if (selectItem != null) selectItem.Selected = true;

            return list;
        }
        

    }



    public class testSapce
    {
        public int id { get; set; }
        //public string describe { get; set; }
        public List<spaceStep> steps { get; set; }

       
    }

    public class spaceStep {
        public string name { get; set; }

        public string describe { get; set; }

        public List<spaceStepAttr> attrs { get; set; }
    }

    public class spaceStepAttr {
        public string name { get; set; }

        public string describe { get; set; }

        public string defValue { get; set; }
        public Dictionary<string, string> list { get; set; }
    }



}
