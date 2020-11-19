using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.ViewModels
{
    /// <summary>
    /// 用户角色分配模型
    /// </summary>
    public class RolesInUserViewModel
    {
        [Display(Name ="角色ID")]
        public string RoleId { get; set; }
        [Display(Name = "角色名称")]
        public string RoleName { get; set; }
        [Display(Name = "是否选择")]
        public bool IsSelected { get; set; }
    }
}
