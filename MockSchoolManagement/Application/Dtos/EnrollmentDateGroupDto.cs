using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Application.Dtos
{
    /// <summary>
    /// 入学时间分组
    /// </summary>
    public class EnrollmentDateGroupDto
    {
        /// <summary>
        /// 入学时间，ApplyFormatInEditMode 表示文本框中的值可进行编辑
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EnrollmentDate { get; set; }
        /// <summary>
        /// 学生人数
        /// </summary>
        public int StudentCount { get; set; }
    }
}
