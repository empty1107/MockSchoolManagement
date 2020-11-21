using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.ViewModels
{
    /// <summary>
    /// 邮箱地址视图模型
    /// </summary>
    public class EmailAddressViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
