using MockSchoolManagement.Application.Dtos;
using MockSchoolManagement.Application.Teachers.Dtos;
using MockSchoolManagement.Infrastructure.Repositories;
using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace MockSchoolManagement.Application.Teachers
{
    public class TeacherService : ITeacherService
    {
        private readonly IRepository<Teacher, int> _teacherRepository;

        public TeacherService(IRepository<Teacher, int> teacherRepository)
        {
            this._teacherRepository = teacherRepository;
        }

        public async Task<PagedResultDto<Teacher>> GetPagedTeacherList(GetTeacherInput input)
        {
            var query = _teacherRepository.GetAll();
            if (!string.IsNullOrEmpty(input.FilterText))
            {
                query = query.Where(s => s.Name.Contains(input.FilterText));
            }
            //总记录数
            var count = query.Count();
            //分页排序
            query = query.OrderBy(input.Sorting).Skip((input.CurrentPage - 1) * input.MaxResultCount).Take(input.MaxResultCount);
            //查询结果转化为List
            var models = await query.Include(a => a.OfficeLocation) //加载导航属性 OfficeLocation
                                    .Include(a => a.CourseAssignments) //加载导航属性 CourseAssignments
                                    .ThenInclude(a => a.Course) //加载导航属性 Course
                                    .ThenInclude(a => a.StudentCourses) //加载导航属性 StudentCourses
                                    .ThenInclude(a => a.Student)//加载导航属性关联学生信息
                                    .Include(i => i.CourseAssignments)
                                    .ThenInclude(i => i.Course)
                                    .ThenInclude(i => i.Department)
                                    .AsNoTracking().ToListAsync();
            var dtos = new PagedResultDto<Teacher>
            {
                TotalCount = count,
                CurrentPage = input.CurrentPage,
                MaxResultCount = input.MaxResultCount,
                Data = models,
                FilterText = input.FilterText,
                Sorting = input.Sorting
            };
            return dtos;
        }
    }
}
