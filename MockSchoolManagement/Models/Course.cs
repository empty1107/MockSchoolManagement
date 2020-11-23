using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MockSchoolManagement.Models
{
    public class Course
    {
        //用户自行指定主键值，而不是令数据库自动生成主键值
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CourseID { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }
        public ICollection<StudentCourse> StudentCourses { get; set; }
    }
}