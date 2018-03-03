using System;
using System.Collections.Generic;
using System.Linq;

namespace VicBlogServer.Controllers
{
    public class PagingResult<T>
    {
        public int TotalCount { get; set; }
        public int CurrentPageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPageNumber { get; set; }
        public IEnumerable<T> List { get; set; }
    }

    public static class Paging
    {
        public const int DefaultPageSize = 1;

        public static PagingResult<T> Page<T>(this IEnumerable<T> list, int? pageNumber, int? pageSize)
        {
            var totalCount = list.Count();
            var actualPageSize = pageSize.GetValueOrDefault(DefaultPageSize);
            var actualPageNumber = pageNumber.GetValueOrDefault(1);

            var totalPageNumber = (int)Math.Ceiling((double)totalCount / actualPageSize);
            var paged = list.Skip(actualPageSize * (actualPageNumber-1)).Take(actualPageSize);
            return new PagingResult<T>()
            {
                CurrentPageNumber = actualPageNumber,
                TotalCount = totalCount,
                PageSize = actualPageSize,
                TotalPageNumber = totalPageNumber,
                List = paged
            };
        }
    }
}
