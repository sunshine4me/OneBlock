using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using blockPlayDataEntities;
using oneBlockWeb.ViewModels;
using System.Threading;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace oneBlockWeb.Controllers {

    [Authorize]
    public class TestCaseController : Controller {

        #region 构造函数
        private blockPlayDBContext _context;
        public TestCaseController(blockPlayDBContext context) {
            _context = context;
        }
        #endregion

        public IActionResult Index() {
            return View();
        }


        public IActionResult caseTreeData(int? id) {
            if (id == null)
                return _caseTreeRoot();
            else
                return _caseTreeData(id.Value);
        }

        public IActionResult folderAdd(int? id) {
            TestCase folder = new TestCase();
            folder.Type = 0;
            folder.Name = "新建文件夹";
            folder.UserId = User.userID();
            if (id > 0)
                folder.ParentId = id;
            _context.TestCase.Add(folder);
            _context.SaveChanges();
            
            testCaseTreeNode newNode = new testCaseTreeNode { id = folder.Id, name = folder.Name, type = folder.Type };

            return Json(newNode);
        }

        public IActionResult fileAdd(int? id, int spaceID) {
            TestCase folder = new TestCase();
            folder.Type = 1;
            folder.Name = "新建测试案例";
            if (id > 0)
                folder.ParentId = id;
            folder.UserId = User.userID();
            folder.Body = tmpCase();
            _context.TestCase.Add(folder);
            _context.SaveChanges();

            testCaseTreeNode newNode = new testCaseTreeNode { id = folder.Id, name = folder.Name, type = folder.Type };

            return Json(newNode);
        }

        [HttpGet]
        public IActionResult caseTab(int id) {
            var userID = User.userID();
            var testCase = _context.TestCase.First(t => t.Id == id && t.UserId == userID);

            caseTabModel md = new caseTabModel();
            md.id = testCase.Id;
            md.name = testCase.Name;
            var bd = testCase.bodyModel();
            md.steps = bd.steps;

            
            md.spaceSelect = _context.spaceSelectList(userID, bd.steps);

            return View("caseTab", md);
        }
    

        [HttpPost]
        public void save(int id , [FromBody]testCase model) {
            if (model.steps.Count == 0) {
                Response.StatusCode = 500;
                return;
            }
            var testcase = _context.TestCase.FirstOrDefault(t => t.Id == id);
            testcase.Body = model.toBodyString();
            _context.SaveChanges();
            

        }

        [HttpPost]
        public IActionResult readyRun(int id,[FromBody]testCase model) {
            //先保存
            save(id, model);
            
            var pms = model.getCustomParamet();
            if (pms.Count > 0)
                return PartialView("_readyRun", pms);
            else
                return Content("");

        }

        [HttpPost]
        public IActionResult runCase(int id, Dictionary<string,string> attrs) {
            var testcase = _context.TestCase.FirstOrDefault(t => t.Id == id);
            var tsetcase = testcase.bodyModel();
            tsetcase.ConvertForRun(_context, attrs);
            return Content(tsetcase.toBodyString());
        }



        [HttpPost]
        public void delete(int id) {
            
            var testcase = _context.TestCase.FirstOrDefault(t => t.Id == id);
            testcase.Type = -1;
            _context.SaveChanges();
            
        }

        [HttpPost]
        public void editName(int id,string name) {

            var testcase = _context.TestCase.FirstOrDefault(t => t.Id == id);
            testcase.Name = name;
            _context.SaveChanges();

        }

        [HttpPost]
        public void sort(int? targetID, int sourceID) {

            var source = _context.TestCase.FirstOrDefault(t => t.Id == sourceID);
            if (targetID > 0) {
                var target = _context.TestCase.FirstOrDefault(t => t.Id == targetID);
                if (target.Type > 0) {
                    Response.StatusCode = 500;
                    return;
                }
            } else
                targetID = null;

            source.ParentId = targetID;
            _context.SaveChanges();

        }

       
        

        #region 私有方法
        private IActionResult _caseTreeRoot() {
            var rootNode = new testCaseTreeNode();
            rootNode.name = "RootNode";
            rootNode.id = 0;
            rootNode.type = 0;
            var roots = new List<testCaseTreeNode>();
            roots.Add(rootNode);

            return Json(roots);
        }

        private IActionResult _caseTreeData(int id) {
            var userID = User.userID();
            if (id == 0) {
                var query = from t in _context.TestCase
                            where t.ParentId == null && t.UserId == userID && t.Type>=0
                            select new testCaseTreeNode {
                                id = t.Id,
                                name = t.Name,
                                type = t.Type
                            };

                var nodes = query.ToList();

                return Json(nodes);
            } else {
                var query = from t in _context.TestCase
                            where t.ParentId == id && t.UserId == userID && t.Type >= 0
                            select new testCaseTreeNode {
                                id = t.Id,
                                name = t.Name,
                                type = t.Type
                            };

                var nodes = query.ToList();

                return Json(nodes);
            }

        }

        /// <summary>
        /// 新建案例时默认的空案例
        /// </summary>
        /// <returns></returns>
        private string tmpCase() {
            var csm = new testCase();
            csm.steps = new List<step>();
            var step = new step();
            csm.steps.Add(step);
            step.name = "emptyStep";
            step.describe = "起始空节点";
            step.spaceID = -1;//空sapce
            step.attrs = new Dictionary<string, string>();
            return JsonConvert.SerializeObject(csm);
        }
        #endregion

    }
}
