using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Models
{
    /// <summary>
    /// 用户声明
    /// </summary>
    public class UserClaim
    {
        public string ClaimType { get; set; }
        public bool IsSelected { get; set; }
    }
}
