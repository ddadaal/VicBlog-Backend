using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        private const int DefaultPageSize = 10;

        public static PagingResult<T> Page<T>(this IEnumerable<T> list, int? pageNumber, int? pageSize)
        {
            var totalCount = list.Count();
            var actualPageSize = pageSize ?? DefaultPageSize;
            var actualPageNumber = pageNumber ?? 1;

            var totalPageNumber = (int)Math.Ceiling((double)totalCount / actualPageSize);
            var pagedList = list.Skip(actualPageSize * (actualPageNumber-1)).Take(actualPageSize);
            return new PagingResult<T>()
            {
                CurrentPageNumber = actualPageNumber,
                TotalCount = totalCount,
                PageSize = actualPageSize,
                TotalPageNumber = totalPageNumber,
                List = pagedList
            };
        }
    }
}
