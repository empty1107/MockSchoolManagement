using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MockSchoolManagement.Models
{
    /// <summary>
    /// 学院
    /// </summary>
    public class Department
    {
        public int DepartmentID { get; set; }

        /// <summary>
        /// StringLength 用于设置数据库中字段的最大长度，注意 MinimumLength 不会影响到数据库的架构映射
        /// </summary>
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        /// <summary>
        /// 预算
        /// </summary>
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")] //更改SQL数据类型映射
        public decimal Budget { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "成立时间")]
        public DateTime StartDate { get; set; }

        public int? TeacherID { get; set; }
        /// <summary>
        /// 学院主任
        /// </summary>
        public Teacher Administrator { get; set; }
        public ICollection<Course> Courses { get; set; }
    }
}