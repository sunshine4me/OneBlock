using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Authentication;
using blockPlayDataEntities;
using oneBlockWeb.ViewModels;
using Microsoft.Extensions.Options;
using oneBlockWeb.DI;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace oneBlockWeb.Controllers
{
    [Authorize]
    public class SettingController : Controller
    {
        public static Dictionary<string, string> MenuDic = new Dictionary<string, string> {
            {"Account","帐号" },
            {"TestSpaces","TestSpaces" }
        };

        private blockPlayDBContext _context;
        public SettingController(blockPlayDBContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        [HttpGet]
        public IActionResult Account()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(resetPasswordModel model)
        {
            if (ModelState.IsValid) {
                var user = _context.PlayUser.First(t => t.Id == User.userID());
                if(user.Password==model.CurrentPassword) {
                    user.Password = model.Password;
                    _context.SaveChanges();
                    return Redirect("/");
                } else {
                    ModelState.AddModelError("", "旧密码错误!");
                    return View("Account", model);
                }
            }
            return View("Account", model);
        }

        [HttpGet]
        public IActionResult TestSpaces()
        {
            var userID = User.userID();
            var spaces = from t in _context.TestSpace
                         where t.UserId == userID
                         select new spaceListViewModel
                         {
                             id= t.Id,
                             name = t.Name,
                             describe = t.Describe
                         };


            return View(spaces.ToList());
        }

      
        

    }
}
