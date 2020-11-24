using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Application.Dtos
{
    /// <summary>
    /// 获取学生输入
    /// </summary>
    public class GetStudentInput : PagedSortedAndFilterInput
    {
        public GetStudentInput()
        {
            Sorting = "Id";//默认使用ID排序
        }
    }
}
