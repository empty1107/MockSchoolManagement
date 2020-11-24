using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.Application.Dtos
{
    /// <summary>
    /// 分页模型
    /// </summary>
    public class PaginationModel
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage { get; set; } = 1;
        /// <summary>
        /// 总记录数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 每页记录数
        /// </summary>
        public int PageSize { get; set; } = 1;
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));

        public List<Student> Data { get; set; }
        /// <summary>
        /// 是否显示上一页
        /// </summary>
        public bool ShowPrevious => CurrentPage > 1;
        /// <summary>
        /// 是否显示下一页
        /// </summary>
        public bool ShowNext => CurrentPage < TotalPages;
        /// <summary>
        /// 是否显示第一页
        /// </summary>
        public bool ShowFirst => CurrentPage != 1;
        /// <summary>
        /// 是否显示最后一页
        /// </summary>

        public bool ShowLast => CurrentPage != TotalPages;
    }
}
