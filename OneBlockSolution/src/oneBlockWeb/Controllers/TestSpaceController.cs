using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using oneBlockWeb.ViewModels;
using blockPlayDataEntities;
using Newtonsoft.Json;

namespace oneBlockWeb.Controllers {
    [Authorize]
    public class TestSpaceController : Controller {
        private blockPlayDBContext _context;
        public TestSpaceController(blockPlayDBContext context) {
            _context = context;
        }

        [HttpGet]
        public IActionResult Add() {
            TestSapceAddModel tsa = new TestSapceAddModel();

            tsa.spacedata = "[{\"name\":\"click\",\"describe\":\"点击操作\",\"attrs\":[{\"name\":\"id\",\"describe\":\"控件id\"},{\"name\":\"side\",\"describe\":\"点击位置\",\"defvalue\":\"2\",\"list\":{\"控件左侧\":\"1\",\"中间\":\"2\",\"控件右侧\":\"3\"}}]}]";


            return View(tsa);
        }


        [HttpPost]
        public IActionResult Add(TestSapceAddModel md) {
            if (ModelState.IsValid) {
                var userID = User.userID();
                TestSpace newts = new TestSpace();
                newts.Name = md.name;
                newts.Describe = md.describe;
                var sp = JsonConvert.DeserializeObject<List<spaceStep>>(md.spacedata);
                newts.SapceData = JsonConvert.SerializeObject(sp);
                newts.UserId = userID;
                _context.TestSpace.Add(newts);

                ////默认添加map
                //TestSpaceMap tsm = new TestSpaceMap();
                //tsm.UserId = userID;
                //tsm.SpaceId = newts.Id;
                //_context.TestSpaceMap.Add(tsm);


                _context.SaveChanges();

                return RedirectToAction("TestSpaces", "Setting");
            }

            return View(md);


        }

        [HttpGet]
        public IActionResult Edit(int id) {
            var ts = _context.TestSpace.FirstOrDefault(t => t.Id == id && t.UserId == User.userID());

            TestSapceAddModel md = new TestSapceAddModel();
            md.name = ts.Name;
            md.describe = ts.Describe;
            md.spacedata = ts.SapceData;
            ViewData["id"] = ts.Id;
            return View(md);
        }


        [HttpPost]
        public IActionResult Edit(int id, TestSapceAddModel md) {
            if (ModelState.IsValid) {
                var ts = _context.TestSpace.FirstOrDefault(t => t.Id == id && t.UserId == User.userID());

                

                ts.Name = md.name;
                ts.Describe = md.describe;
                var sp = JsonConvert.DeserializeObject<List<spaceStep>>(md.spacedata);
                
                ts.SapceData = JsonConvert.SerializeObject(sp);

                _context.SaveChanges();

            }
            return RedirectToAction("TestSpaces", "Setting");
        }

        [HttpPost]
        public IActionResult Delete(int id) {
            var ts = _context.TestSpace.FirstOrDefault(t => t.Id == id && t.UserId == User.userID());

            if (ts != null) {
                ts.UserId = 0;
                _context.SaveChanges();
            }

            return RedirectToAction("TestSpaces", "Setting");
        }



        [HttpPost]
        public IActionResult treeNodeJson(int id) {
            var space = _context.TestSpace.First(t => t.Id == id);
            var ts = _context.TestSpace.FirstOrDefault(t => t.Id == id && t.UserId == User.userID());

            TestSapceAddModel md = new TestSapceAddModel();
            md.name = ts.Name;
            md.describe = ts.Describe;
            md.spacedata = ts.SapceData;
            ViewData["id"] = ts.Id;
            return View(md);
        }

    }
}
