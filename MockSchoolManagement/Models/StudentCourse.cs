using System.ComponentModel.DataAnnotations;

namespace MockSchoolManagement.Models
{
    public class StudentCourse
    {
        [Key]
        public int StudentsCourseId { get; set; }
        public int CourseID { get; set; }
        public int StudentID { get; set; }
        public Course Course { get; set; }
        public Student Student { get; set; }
    }
}