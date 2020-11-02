using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.ViewModels
{
    /// <summary>
    /// 注册模型
    /// </summary>
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "请输入邮箱地址")]
        [EmailAddress]
        [Display(Name = "邮箱地址")]
        public string Email { get; set; }
        [Required(ErrorMessage = "请输入密码")]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }
        [Required(ErrorMessage = "请输入确认密码")]
        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码与确认密码不一致，请重新输入。")]
        public string ConfirmPassword { get; set; }
    }
}
