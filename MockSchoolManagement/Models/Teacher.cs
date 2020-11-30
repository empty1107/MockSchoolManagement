using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Models
{
    /// <summary>
    /// 教师信息
    /// </summary>
    public class Teacher
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "姓名")]
        [StringLength(50)]
        [Column("TeacherName")] // Column 用于更改列名称映射
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "聘用时间")]
        public DateTime HireDate { get; set; }

        /// <summary>
        /// 教师课程分配，选择集合类型为 ICollection<T>，EF Core 会默认创建一个 HashSet<T>集合
        /// </summary>
        public ICollection<CourseAssignment> CourseAssignments { get; set; }

        /// <summary>
        /// 办公室位置
        /// </summary>
        public OfficeLocation OfficeLocation { get; set; }
    }
}
