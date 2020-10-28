using Microsoft.AspNetCore.Http;
using MockSchoolManagement.Models.EnumTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.ViewModels
{
    /// <summary>
    /// 学生编辑模型
    /// </summary>
    public class StudentEditViewModel : StudentCreateViewModel
    {
        public int Id { get; set; }
        ///  <summary>
        ///  已经存在数据库中的图片文件路径
        ///  </summary>
        public string ExistingPhotoPath { get; set; }
    }
}
