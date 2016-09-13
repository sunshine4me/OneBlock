using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace oneBlockWeb.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "用户名")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9_]*$", ErrorMessage = "英文开头,并只能使用英文、数字或_(下划线)")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "{0} 必须在 {2} ~ {1} 个字符之间")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        [StringLength(20, ErrorMessage = "{0} 不大于{2}个字符")]
        public string Password { get; set; }

        [Display(Name = "记住我?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "用户名")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9_]*$", ErrorMessage = "英文开头,并只能使用英文、数字或_(下划线)")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "{0} 必须在 {2} ~ {1} 个字符之间")]
        public string UserName { get; set; }

        [Display(Name = "昵称")]
        [Required]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "{0} 必须在 {2} ~ {1} 个字符之间")]
        public string Name { get; set; }


        [Required]
        [StringLength(20, ErrorMessage = "{0} 必须大于 {2} 位", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "两次密码不一致")]
        public string ConfirmPassword { get; set; }
    }


    public class resetPasswordModel
    {

        [Required]
        [StringLength(20, ErrorMessage = "{0} 必须大于 {2} 位", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "旧密码")]
        public string CurrentPassword { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "{0} 必须大于 {2} 位", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "两次密码不一致")]
        public string ConfirmPassword { get; set; }
    }
}
