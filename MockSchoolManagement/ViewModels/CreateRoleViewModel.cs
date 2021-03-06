﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.ViewModels
{
    /// <summary>
    /// 创建角色模型
    /// </summary>
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "角色名称不能为空")]
        [Display(Name = "角色")]
        public string RoleName { get; set; }
    }
}
