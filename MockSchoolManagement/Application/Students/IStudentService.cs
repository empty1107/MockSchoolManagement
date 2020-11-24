using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Application.Students
{
    /// <summary>
    /// 用于定义提供的学生服务
    /// </summary>
    public interface IStudentService
    {
        /// <summary>
        /// 获取学生分页数据
        /// </summary>
        /// <param name="currentPage">当前页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns></returns>
        Task<List<Student>> GetPaginatedResult(int currentPage, string searchString, string sortBy, int pageSize = 10);
    }
}
