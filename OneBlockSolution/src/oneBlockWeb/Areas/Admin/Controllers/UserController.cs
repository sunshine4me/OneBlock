using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using blockPlayDataEntities;
using oneBlockWeb.Areas.Admin.ViewModels;
using System;
using Microsoft.Extensions.Options;
using oneBlockWeb.DI;

namespace oneBlockWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class UserController : Controller
    {
        private blockPlayDBContext _context;

        public UserController(blockPlayDBContext context)
        {
            _context = context;
        }
       
        
        public IActionResult userManage()
        {
            return View();
        }

        [HttpGet]
        public IActionResult edit(int id)
        {
            var u = _context.PlayUser.FirstOrDefault(t => t.Id == id);
            if (u == null)
                HttpContext.Response.StatusCode = 404;

            editUserModel model = new editUserModel();
            model.id = u.Id;
            model.username = u.Username;
            model.lv = u.Lv;
            model.name = u.Name;
            model.locked = u.Locked;
            return PartialView("_editUser", model);
        }

        [HttpPost]
        public void edit(editUserModel model)
        {
            if (ModelState.IsValid)
            {
                var u = _context.PlayUser.FirstOrDefault(t => t.Id == model.id);
                if (u.Lv == 99) return;//超级管理员不给修改
                u.Name = model.name;
                u.Lv = model.lv;
                _context.SaveChanges();
            }else
            {
                Response.StatusCode = 400;
            }
            
        }

        [HttpPost]
        public void add(addUserModel model,[FromServices]IOptions<WebSetting> settings)
        {
            if (ModelState.IsValid)
            {
                var u = _context.PlayUser.SingleOrDefault(t => t.Username == model.username);
                if (u != null) return;//已经存在用户不让注册

                var newUser = new PlayUser();
                newUser.Username = model.username;
                newUser.Name = model.name;
                newUser.Password = settings.Value.DefultPassword;
                newUser.Lv = model.lv;
                newUser.JoinDate = DateTime.Now;
                _context.PlayUser.Add(newUser);
                ////为新增用户添加adminSpace
                //if(settings.Value.DefultUseAdminSpace)
                //{
                //    //ef core 目前不支持 LazyLoading ,只能自己查询
                //    var adminUser = _context.PlayUser.First(t => t.Lv == 99);
                //    var sps = _context.TestSpace.Where(t => t.UserId == adminUser.Id);

                //    foreach (var s in sps)
                //    {
                //        TestSpaceMap tsm = new TestSpaceMap();
                //        tsm.UserId = newUser.Id;
                //        tsm.SpaceId = s.Id;
                //        _context.TestSpaceMap.Add(tsm);
                //    }
                //}

                _context.SaveChanges();
            }
            else
            {
                Response.StatusCode = 400;
            }

        }

        [HttpPost]
        public void resetPassword(int id, [FromServices]IOptions<WebSetting> settings)
        {
            var u = _context.PlayUser.FirstOrDefault(t => t.Id == id);
            u.Password = settings.Value.DefultPassword;
            _context.SaveChanges();
        }

        

        [HttpPost]
        [ActionName("lock")]
        public void lockUser(int id)
        {
            var u = _context.PlayUser.FirstOrDefault(t => t.Id == id);
            u.Locked = true;
            _context.SaveChanges();
        }

        [HttpPost]
        [ActionName("unlock")]
        public void unlockUser(int id)
        {
            var u = _context.PlayUser.FirstOrDefault(t => t.Id == id);
            u.Locked = false;
            _context.SaveChanges();
        }

        public IActionResult userList(int page, int rows)
        {
            easyUI_datagrid<userListModel> ulm = new easyUI_datagrid<userListModel>();
            ulm.total = _context.PlayUser.Count();
            var users = from t in _context.PlayUser
                        where t.Lv > 0
                        select new userListModel
                        {
                            id = t.Id,
                            username = t.Username,
                            name = t.Name,
                            joinDate = t.JoinDate,
                            lv = t.Lv,
                            locked = t.Locked
                        };
            ulm.rows = users.Skip(rows * (page - 1)).Take(rows).ToList();

            return Json(ulm);


        }


       
    }
}
