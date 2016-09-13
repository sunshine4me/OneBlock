using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace oneBlockWeb.DI
{
    public class userLV
    {
        private Dictionary<int, string> _lv;
        public userLV()
        {
            _lv = new Dictionary<int, string>();
            _lv.Add(1, "普通用户");
            _lv.Add(90, "管理员");
            _lv.Add(99, "超级管理员");
        }

        public List<SelectListItem> getListItem(int selectId)
        {
            List<SelectListItem> slt = new List<SelectListItem>();

            foreach (var d in _lv.Where(t => t.Key < 99))
            {
                slt.Add(new SelectListItem() { Text = d.Value, Value = d.Key.ToString(), Selected = selectId == d.Key });
            }

            return slt;
        }
    }
}
