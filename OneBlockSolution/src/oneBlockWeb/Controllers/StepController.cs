using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using blockPlayDataEntities;
using Newtonsoft.Json;
using oneBlockWeb.ViewModels;

namespace oneBlockWeb.Controllers {
    public class StepController : Controller {
        #region 构造函数
        private blockPlayDBContext _context;
        public StepController(blockPlayDBContext context) {
            _context = context;
        }
        #endregion

        public IActionResult steps(int id) {

            if (id > 0) {
                var treeList = new List<step>();
                var _testSpace = _context.TestSpace.First(t => t.Id == id);
                treeList = _testSpace.spaceModel().toSteplList();
                return Json(treeList);
            } else {
                var treeList = new List<step>();
                int userID = User.userID();
                var bsList = (from t in _context.BlockStep
                              where t.UserId == userID
                              select new {
                                  blockID = t.Id,
                                  describe = t.Name,
                                  attrsString = t.Attrs
                              }).ToList();


                foreach (var bs in bsList) {
                    var _tmp = new step();
                    _tmp.blockID = bs.blockID;
                    _tmp.name = $"Block.{bs.blockID}";
                    _tmp.describe = bs.describe;
                    _tmp.attrs = JsonConvert.DeserializeObject<Dictionary<string, string>>(bs.attrsString);
                    treeList.Add(_tmp);
                }

                return Json(treeList);
            }

        }

        [HttpPost]
        public IActionResult edit([FromBody]stepModel model) {

            if (model.attrs == null) model.attrs = new Dictionary<string, string>(); //除错

             StepEditModel sem;
            if (model.spaceID > 0) {
                sem =  spaceStepEdit(model);
            }else if(model.blockID > 0) {
                sem = blockStepEdit(model);
            } else { 
                sem = unknownStepEdit(model);
            }

            return PartialView("_StepEdit", sem);

        }


        private StepEditModel spaceStepEdit(stepModel model) {
            var se = new StepEditModel();
      
            var sp = _context.TestSpace.FirstOrDefault(t => t.Id == model.spaceID);
            if (sp == null) return unknownStepEdit(model);

            var spStep = sp.spaceModel().steps.FirstOrDefault(t => t.name == model.name);
            if (spStep == null) return unknownStepEdit(model);

            //添加是否启用属性
            if (spStep.attrs.FirstOrDefault(t => t.name == "是否启用") == null) {
                var list = new Dictionary<string, string>();
                list.Add("启用","true");
                list.Add("不启用", "false");
                spStep.attrs.Add(new spaceStepAttr {
                    name = "是否启用",
                    defValue = "true",
                    describe="不启用的步骤将在生成时删除",
                    list = list
                });
            }

            se.fullName = $"{sp.Name} - {spStep.name}({spStep.describe})";
            se.describe = model.describe;
            se.attrs = (from t in spStep.attrs
                       select new StepEditAttrModel {
                           name = t.name,
                           value = model.attrs.ContainsKey(t.name)? model.attrs[t.name]:t.defValue,
                           describe = t.describe,
                           list = t.list
                       }).ToList();

       
            return se;
        }

        private StepEditModel blockStepEdit(stepModel model) {
            var se = new StepEditModel();

            var bs = _context.BlockStep.FirstOrDefault(t => t.Id == model.blockID);
            if (bs == null) return unknownStepEdit(model);


            se.fullName = $"Block - No.{bs.Id}:{bs.Name}";
            se.describe = model.describe;
            se.attrs = (from t in bs.blockAttrs()
                        select new StepEditAttrModel {
                            name = t.Key,
                            value = model.attrs.ContainsKey(t.Key) ? model.attrs[t.Key] : t.Value,
                            describe = t.Key
                        }).ToList();
            return se;
        }

        /// <summary>
        /// 未知步骤编辑
        /// </summary>
        private StepEditModel unknownStepEdit(stepModel model) {
            var se = new StepEditModel();
            
            se.fullName = "unknown";
            se.describe = model.describe;
            se.attrs = (from t in model.attrs
                        select new StepEditAttrModel {
                            name = t.Key,
                            value = t.Value,
                            describe = t.Key
                        }).ToList();


            return se;
        }

       

    }
}
