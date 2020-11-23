using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MockSchoolManagement.DataRepositories;

namespace MockSchoolManagement.Controllers
{
    /// <summary>
    /// 课程控制器
    /// </summary>
    public class CourseController : Controller
    {
        private readonly ICourseRepository _courseRepository;

        public CourseController(ICourseRepository courseRepository)
        {
            this._courseRepository = courseRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
