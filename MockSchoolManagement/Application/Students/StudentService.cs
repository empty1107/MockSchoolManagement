using MockSchoolManagement.Infrastructure.Repositories;
using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<Student>> GetPaginatedResult(int currentPage, string searchString, string sortBy, int pageSize = 10)
        {
            var query = _stuRepository.GetAll();
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.Name.Contains(searchString) || s.Email.Contains(searchString));
            }
            query = query.OrderBy(sortBy);
            var list = await query.Skip((currentPage - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
            return list;
        }
    }
}
