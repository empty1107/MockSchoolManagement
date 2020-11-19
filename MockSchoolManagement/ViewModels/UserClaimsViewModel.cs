using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MockSchoolManagement.ViewModels
{
    /// <summary>
    /// 用户声明视图模型
    /// </summary>
    public class UserClaimsViewModel
    {
        public UserClaimsViewModel()
        {
            Cliams = new List<UserClaim>();
        }
        public string UserId { get; set; }
        public List<UserClaim> Cliams { get; set; }
    }
}
