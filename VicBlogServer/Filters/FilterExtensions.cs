using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VicBlogServer.Filters
{
    public static class FilterExtensions
    {
        public static T RequireService<T>(this ActionExecutingContext context) where T : class
        {
            return context.HttpContext.RequestServices.GetService(typeof(T)) as T;
        }

    }
}
