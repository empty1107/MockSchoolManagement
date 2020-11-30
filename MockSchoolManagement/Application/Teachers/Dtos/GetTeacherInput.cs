using MockSchoolManagement.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Application.Teachers.Dtos
{
    public class GetTeacherInput : PagedSortedAndFilterInput
    {
        /// <summary>
        /// 教师ID
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// 课程ID
        /// </summary>
        public int? CourseId { get; set; }
        public GetTeacherInput()
        {
            Sorting = "Id";
            MaxResultCount = 3;
        }
    }
}
