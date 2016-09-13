using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace oneBlockWeb.ViewModels
{
    public class TestSapceAddModel
    {
        [Required]
        [Display(Name = "TestSpace 名字")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "{0} 必须在 {2} ~ {1} 个字符之间")]
        public string name { get; set; }

        [Display(Name = "简介 (可选) ")]
        [StringLength(100, ErrorMessage = "{0} 不能大于{1} 字")]
        public string describe { get; set; }

        [Required]
        [Display(Name = "TestSpace 描述文件")]
        public string spacedata { get; set; }
    }


    public class spaceListViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string describe { get; set; }
    }


}
