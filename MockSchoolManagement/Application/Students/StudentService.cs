using MockSchoolManagement.Infrastructure.Repositories;
using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using MockSchoolManagement.Application.Dtos;

namespace MockSchoolManagement.Application.Students
{
    /// <summary>
    /// 用于实现学生服务
    /// </summary>
    public class StudentService : IStudentService
    {
        private readonly IRepository<Student, int> _stuRepository;

        public StudentService(IRepository<Student, int> stuRepository)
        {
            this._stuRepository = stuRepository;
        }

        public async Task<PagedResultDto<Student>> GetPaginatedResult(GetStudentInput input)
        {
            var query = _stuRepository.GetAll();
            if (!string.IsNullOrEmpty(input.FilterText))
            {
                query = query.Where(s => s.Name.Contains(input.FilterText) || s.Email.Contains(input.FilterText));
            }
            //统计总记录数，用于分页计算总页数
            var count = query.Count();
            //根据需求排序
            query = query.OrderBy(input.Sorting).Skip((input.CurrentPage - 1) * input.MaxResultCount).Take(input.MaxResultCount);
            //查询结果List，加载到内存中
            var models = await query.AsNoTracking().ToListAsync();

            var dtos = new PagedResultDto<Student>
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
