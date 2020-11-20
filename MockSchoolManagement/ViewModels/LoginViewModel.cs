using Microsoft.AspNetCore.Authentication;
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
        [Display(Name = "用户名")]
        [Required(ErrorMessage = "请输入用户名")]
        public string UserName { get; set; }
        [Display(Name = "密码")]
        [Required(ErrorMessage = "请输入密码")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "记住我")]
        public bool RememberMe { get; set; }
        [Display(Name = "回调地址")]
        public string ReturnUrl { get; set; }
        [Display(Name = "第三方登录列表")]
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }
}
