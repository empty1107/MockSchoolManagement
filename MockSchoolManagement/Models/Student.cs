using MockSchoolManagement.Models.EnumTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Models
{
    /// <summary>
    /// 学生模型
    /// </summary>
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public MajorEnum? Major { get; set; }
        public string Email { get; set; }
        public string PhotoPath { get; set; }
    }
}
