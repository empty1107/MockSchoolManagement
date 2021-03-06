﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.ViewModels
{
    /// <summary>
    /// 重置密码视图模型
    /// </summary>
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage ="邮箱地址不能为空")]
        [Display(Name = "邮箱地址")]
        public string Email { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Required(ErrorMessage = "确认密码不能为空")]
        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码与确认密码不一致，请重新输入")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "邮箱地址")]
        public string Token { get; set; }
    }
}
