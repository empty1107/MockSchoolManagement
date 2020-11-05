using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }
    }
}
