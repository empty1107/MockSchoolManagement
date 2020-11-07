using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.ViewModels
{
    /// <summary>
    /// 修改用户模型
    /// </summary>
    public class EditUserViewModel
    {
        [Display(Name ="用户Id")]
        public string Id { get; set; }

        [Required(ErrorMessage ="用户名不能为空")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "邮箱不能为空")]
        [Display(Name = "邮箱")]
        public string Email { get; set; }

        [Display(Name = "城市")]
        public string City { get; set; }

        [Display(Name = "声明")]
        public List<string> Claims { get; set; }

        [Display(Name = "角色")]
        public IList<string> Roles { get; set; }

        public EditUserViewModel()
        {
            Claims = new List<string>();
            Roles = new List<string>();
        }
    }
}
