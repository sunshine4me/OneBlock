using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using oneBlockWeb.ViewModels;
using blockPlayDataEntities;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace oneBlockWeb.Controllers
{
    [Authorize]
    public class BlockStepController : Controller {
        #region 构造函数
        private blockPlayDBContext _context;
        public BlockStepController(blockPlayDBContext context) {
            _context = context;
        }
        #endregion

        // GET: /<controller>/
        [HttpPost]
        public IActionResult build([FromBody]testCase model) {

            var Paramets = model.getCustomParamet();

            return PartialView("_build", Paramets);

        }

        [HttpPost]
        public IActionResult building([FromBody]BlockStepAddModel model) {
            var bolckStep = new BlockStep();
            bolckStep.Name = model.blockName;


            bolckStep.Body = model.testCase.toBodyString();
            //重新筛选参数, 是否必要?
            var Paramets = model.testCase.getCustomParamet();
            foreach (var attr in model.attrs) {
                if (Paramets.ContainsKey(attr.Key))
                    Paramets[attr.Key] = attr.Value;
            }

            bolckStep.Attrs = JsonConvert.SerializeObject(Paramets);
            bolckStep.UserId = User.userID();

            _context.BlockStep.Add(bolckStep);
            _context.SaveChanges();

            return Content(bolckStep.Id + "");

        }


      

        public IActionResult editTab(int id) {
            var userID = User.userID();
            var blockStep = _context.BlockStep.FirstOrDefault(t => t.Id == id);

            var model = new blockTabModel();
            model.id = blockStep.Id;
            model.name = blockStep.Name;
            model.steps = blockStep.blockSteps();


            model.spaceSelect = _context.spaceSelectList(userID, model.steps);

            var bloclSelect = model.spaceSelect.First(t => t.Value == "0");
            model.spaceSelect.Remove(bloclSelect);

            return View("blockTab", model);
        }

        [HttpPost]
        public IActionResult edit(int id, [FromBody]testCase model) {

            var bs = _context.BlockStep.First(t => t.Id == id);
            var bsAttr = bs.blockAttrs();
            
            var Paramets = model.getCustomParamet();
            Paramets.merge(bsAttr);

            ViewBag.id = id;
            ViewBag.name = bs.Name;
            return PartialView("_build", Paramets);

        }
        [HttpPost]
        public void save(int id,[FromBody]BlockStepAddModel model) {

            var bolckStep = _context.BlockStep.First(t => t.Id == id && t.UserId == User.userID());
            bolckStep.Name = model.blockName;

 
            bolckStep.Body = model.testCase.toBodyString();
            //重新筛选参数, 是否必要?
            var Paramets = model.testCase.getCustomParamet();
            Paramets.merge(model.attrs);
            bolckStep.Attrs = JsonConvert.SerializeObject(Paramets);


            _context.SaveChanges();

            return;
        }
    }
}
