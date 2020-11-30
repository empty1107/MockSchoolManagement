using MockSchoolManagement.Application.Dtos;
using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.ViewModels
{
    /// <summary>
    /// 教师集合视图模型
    /// </summary>
    public class TeacherListViewModel
    {
        public PagedResultDto<Teacher> Teachers { get; set; }
        public List<Course> Courses { get; set; }
        public List<StudentCourse> StudentCourses { get; set; }
        /// <summary>
        /// 选中的教师ID
        /// </summary>
        public int SelectedId { get; set; }
        /// <summary>
        /// 选中的课程ID
        /// </summary>
        public int SelectedCourseId { get; set; }
    }
}
