using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.ViewModels
{
    /// <summary>
    /// 修改密码视图模型
    /// </summary>
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "当前密码不能为空")]
        [DataType(DataType.Password)]
        [Display(Name = "当前密码")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "新密码不能为空")]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "确认新密码不能为空")]
        [DataType(DataType.Password)]
        [Display(Name = "确认新密码")]
        [Compare("NewPassword", ErrorMessage = "新密码和确认密码不匹配")]
        public string ConfirmPassword { get; set; }
    }
}
