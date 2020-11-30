using MockSchoolManagement.Infrastructure.Repositories;
using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Application.Dtos;

namespace MockSchoolManagement.Application.Courses
{
    /// <summary>
    /// 课程接口实现类
    /// </summary>
    public class CourseService : ICourseService
    {
        private readonly IRepository<Course, int> _courseRepository;

        public CourseService(IRepository<Course, int> courseRepository)
        {
            this._courseRepository = courseRepository;
        }
        public async Task<PagedResultDto<Course>> GetPaginatedResult(GetCourseInput input)
        {
            var query = _courseRepository.GetAll();
            var count = query.Count();
            if (!string.IsNullOrEmpty(input.FilterText))
            {
                query = query.Where(s => s.Title.Contains(input.FilterText));
            }
            query = query.OrderBy(input.Sorting).Skip((input.CurrentPage - 1) * input.MaxResultCount).Take(input.MaxResultCount);
            //将查询结果转为List集合，加载到内存中
            var models = await query.Include(a => a.Department).AsNoTracking().ToListAsync();

            var dtos = new PagedResultDto<Course>
            {
                TotalCount = count,
                CurrentPage = input.CurrentPage,
                PageSize = input.MaxResultCount,
                MaxResultCount = input.MaxResultCount,
                Data = models,
                FilterText = input.FilterText,
                Sorting = input.Sorting
            };
            return dtos;
        }
    }
}
