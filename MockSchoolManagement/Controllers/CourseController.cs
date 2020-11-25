using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MockSchoolManagement.Application.Courses;
using MockSchoolManagement.Application.Dtos;
using MockSchoolManagement.DataRepositories;

namespace MockSchoolManagement.Controllers
{
    /// <summary>
    /// 课程控制器
    /// </summary>
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            this._courseService = courseService;
        }
        public async Task<IActionResult> Index(GetCourseInput input)
        {
            var models = await _courseService.GetPaginatedResult(input);
            return View(models);
        }
    }
}
