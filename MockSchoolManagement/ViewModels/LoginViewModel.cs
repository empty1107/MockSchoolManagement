using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.ViewModels
{
    /// <summary>
    /// 登录模型
    /// </summary>
    public class LoginViewModel
    {
        [Display(Name = "邮箱地址")]
        [Required(ErrorMessage = "请输入邮箱地址")]
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name = "密码")]
        [Required(ErrorMessage = "请输入密码")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "记住我")]
        public bool RememberMe { get; set; }
    }
}
