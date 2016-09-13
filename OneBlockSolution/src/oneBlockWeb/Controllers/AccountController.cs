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

namespace oneBlockWeb.Controllers
{
    public class AccountController : Controller
    {

        private blockPlayDBContext _context;
        public AccountController(blockPlayDBContext context)
        {
            _context = context;
        }

        public IActionResult Login(string ReturnUrl)
        {
            ViewData["ReturnUrl"] = ReturnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = "/")
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {

              
                var user = _context.PlayUser.SingleOrDefault(t => t.Username == model.UserName && t.Password == model.Password);
                
                if (user == null)
                {
                        ModelState.AddModelError("", "用户名或密码错误");
                    return View(model);
                }

                if (user.Locked == true)
                {
                    ModelState.AddModelError("", "用户已被禁用");
                    return View(model);
                }


                ClaimsIdentity _identity = new ClaimsIdentity("mcookie");
                _identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
                _identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));

                _identity.AddClaim(new Claim("userName", user.Username));

                if (user.Lv > 90)
                {
                    _identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));
                }


                //var AllowRefresh = model.RememberMe;
                var ExpiresUtc = DateTime.UtcNow.AddDays(1);
                if(model.RememberMe)
                    ExpiresUtc = DateTime.UtcNow.AddDays(30);

                //await HttpContext.Authentication.SignOutAsync("mcookie");
                await HttpContext.Authentication.SignInAsync("mcookie", new ClaimsPrincipal(_identity), new AuthenticationProperties
                {
                    ExpiresUtc = ExpiresUtc,
                    IsPersistent = true,//cookie持久化
                    AllowRefresh = false //自动刷新cookie
                });



                return Redirect(returnUrl);
            }
            return View(model);

        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model )
        {
            if (ModelState.IsValid)
            {
                var u = _context.PlayUser.FirstOrDefault(t => t.Username == model.UserName);
                if(u!=null)
                {
                    ModelState.AddModelError("", "帐号已存在!");
                    return View(model);
                }
                var newUser = new PlayUser();
                newUser.Username = model.UserName;
                newUser.Name = model.Name;
                newUser.Password = model.Password;
                newUser.Lv = 1;
                newUser.JoinDate = DateTime.Now;
                _context.PlayUser.Add(newUser);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            
            return View(model);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.Authentication.SignOutAsync("mcookie");
            return RedirectToAction("Index", "Home");

        }
        


        public IActionResult Forbidden()
        {
            return View("Error");
        }

    }
}
