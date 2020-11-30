using System.ComponentModel.DataAnnotations;

namespace MockSchoolManagement.Models
{
    /// <summary>
    /// 办公室地点
    /// </summary>
    public class OfficeLocation
    {
        //因为TeacherId是Teacher表的外键，但是又想让 TeacherId 当 OfficeLocation 表主键，所以使用Key
        //因为EF Core 无法自动识别 TeacherId 作为此实体的主键
        [Key]
        public int TeacherId { get; set; }

        [StringLength(50)]
        [Display(Name ="办公室位置")]
        public string Location { get; set; }

        public Teacher Teacher { get; set; }
    }
}