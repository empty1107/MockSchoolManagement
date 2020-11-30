using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Application.Courses;
using MockSchoolManagement.Application.Dtos;
using MockSchoolManagement.DataRepositories;
using MockSchoolManagement.Infrastructure.Repositories;
using MockSchoolManagement.Models;
using MockSchoolManagement.ViewModels;

namespace MockSchoolManagement.Controllers
{
    /// <summary>
    /// 课程控制器
    /// </summary>
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly IRepository<Course, int> _courseRepository;
        private readonly IRepository<Department, int> _deptRepository;

        public CourseController(ICourseService courseService, IRepository<Course, int> courseRepository,IRepository<Department,int> deptRepository)
        {
            this._courseService = courseService;
            this._courseRepository = courseRepository;
            this._deptRepository = deptRepository;
        }
        public async Task<IActionResult> Index(GetCourseInput input)
        {
            var models = await _courseService.GetPaginatedResult(input);
            return View(models);
        }

        #region 添加课程
        public IActionResult Create()
        {
            var dtos = DepartmentsDropDownList();
            CourseCreateViewModel model = new CourseCreateViewModel
            {
                DepartmentList = dtos
            };
            return View(model);
        }

        /// <summary>
        /// 学院的下拉列表
        /// </summary>
        /// <returns></returns>
        private SelectList DepartmentsDropDownList(object selectedDepartment = null)
        {
            var models = _deptRepository.GetAll().OrderBy(a => a.Name).AsNoTracking().ToList();
            var dtos = new SelectList(models, "DepartmentID", "Name", selectedDepartment);
            return dtos;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CourseCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                Course course = new Course
                {
                    CourseID = model.CourseID,
                    Title = model.Title,
                    Credits = model.Credits,
                    DepartmentID = model.DepartmentID
                };
                await _courseRepository.InsertAsync(course);
                //nameof 用于获取属性的名称，并返回字符串
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        #endregion

    }
}
