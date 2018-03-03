using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VicBlogServer.Controllers;

namespace VicBlogServer.ViewModels
{
    public class PagingInfo
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPageNumber { get; set; }
    }

    public static class PagingInfoExtension
    {
        public static PagingInfo ToPagingInfo<T>(this PagingResult<T> result)
        {
            return new PagingInfo()
            {
                TotalCount = result.TotalCount,
                PageSize = result.PageSize,
                CurrentPage = result.CurrentPageNumber,
                TotalPageNumber = result.TotalPageNumber
            };
        }
    }
}
