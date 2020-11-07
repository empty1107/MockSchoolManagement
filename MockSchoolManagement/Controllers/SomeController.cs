using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MockSchoolManagement.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class SomeController : Controller
    {

        public string ABC()
        {
            return "我的ABC方法，只要有Admin或者User角色即可访问我";
        }

        [Authorize(Roles = "Admin")]
        public string XYZ()
        {
            return "我是XYZ，Admin角色可访问";
        }

        [AllowAnonymous]
        public string Anyone()
        {
            return "任何人可访问，我添加了allowanonymous属性.";
        }
    }
}
