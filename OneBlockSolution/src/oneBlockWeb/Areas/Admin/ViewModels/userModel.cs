
using System;
using System.ComponentModel.DataAnnotations;

namespace oneBlockWeb.Areas.Admin.ViewModels
{
    public class editUserModel
    {
        [Required]
        public int id { get; set; }

        public string username { get; set; }

        [Display(Name = "昵称")]
        [Required]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "{0} 必须在 {2} ~ {1} 个字符之间")]
        public string name { get; set; }

        [Display(Name = "用户类型")]
        [Required]
        [Range(1, 98)]
        public int lv { get; set; }

        public bool? locked { get; set; }
    }

    //暂时没用到
    public class addUserModel
    {

        [Required]
        [Display(Name = "用户名")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9_]*$", ErrorMessage = "英文开头,并只能使用英文、数字或_(下划线)")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "{0} 必须在 {2} ~ {1} 个字符之间")]
        public string username { get; set; }

        [Display(Name = "昵称")]
        [Required]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "{0} 必须在 {2} ~ {1} 个字符之间")]
        public string name { get; set; }

        [Display(Name = "用户类型")]
        [Required]
        [Range(1, 98)]
        public int lv { get; set; }
    }

    public class userListModel
    {
        public int id { get; set; }
        public string username { get; set; }
        public string name { get; set; }

        public DateTime joinDate { get; set; }
        public int lv { get; set; }

        public bool? locked { get; set; }
    }
}
