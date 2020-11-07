using Microsoft.AspNetCore.Http;
using MockSchoolManagement.Models.EnumTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.ViewModels
{
    public class StudentCreateViewModel
    {
        [Required(ErrorMessage = "请输入名字"), MaxLength(50, ErrorMessage = "名字的长度不能超过50个字符")]
        [Display(Name = "名字")]
        public string Name { get; set; }
        [Required(ErrorMessage ="请选择一门科目")]
        [Display(Name = "主修科目")]
        public MajorEnum? Major { get; set; }
        [Display(Name = "电子邮箱")]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "邮箱的格式不正确")]
        [Required(ErrorMessage = "请输入邮箱地址")]
        public string Email { get; set; }
        /// <summary>
        /// 上传至服务器的文件均可通过IFormFile接口使用模型绑定的形式进行访问。
        /// 如果要支持多个文件上传，则将Photo属性的数据类型设置为List即可
        /// </summary>
        [Display(Name = "头像")]
        public List<IFormFile> Photos { get; set; }

        public StudentCreateViewModel()
        {
            Photos = new List<IFormFile>();
        }
    }
}
