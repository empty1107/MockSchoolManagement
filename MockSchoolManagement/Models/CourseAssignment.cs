namespace MockSchoolManagement.Models
{
    /// <summary>
    /// 课程设置分配
    /// 我们不需要单独指定主键，因为 TeacherID 和 CourseID 属性是联合主键
    /// 要使用联合主键，需要配置 Fluent API 来完成
    /// </summary>
    public class CourseAssignment
    {
        public int TeacherID { get; set; }

        public int CourseID { get; set; }

        public Teacher Teacher { get; set; }

        public Course Course { get; set; }
    }
}