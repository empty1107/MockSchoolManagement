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
    public class PagedResultDto<TEntity> : PagedSortedAndFilterInput
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 每页记录数
        /// </summary>
        public int PageSize { get; set; } = 10;
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(TotalCount, PageSize));
        /// <summary>
        /// 查询数据（泛型）
        /// </summary>
        public List<TEntity> Data { get; set; }
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
