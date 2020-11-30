using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.ViewModels
{
    /// <summary>
    /// 课程创建视图模型
    /// </summary>
    public class CourseCreateViewModel
    {
        [Display(Name = "课程编号")]
        public int CourseID { get; set; }

        [Display(Name = "课程名称")]
        public string Title { get; set; }

        [Display(Name = "课程学分")]
        public int Credits { get; set; }

        public int DepartmentID { get; set; }

        [Display(Name ="学院")]
        public SelectList DepartmentList { get; set; }
    }
}
