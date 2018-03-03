using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VicBlogServer.DataService;
using VicBlogServer.ViewModels.Dto;

namespace VicBlogServer.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ArticleExistsAttribute : ActionFilterAttribute
    {
        private const string Key = "articleId";

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var articleService = context.RequireService<IArticleDataService>();

            var raw = context.ModelState[Key].RawValue;

            if (!int.TryParse(raw.ToString(), out var id))
            {
                context.Result = new BadRequestObjectResult(new StandardErrorDto()
                {
                    Code = "ArgumentInvalid",
                    Description = "Article id is not a int."
                });
            } else if (!(await articleService.IdExistsAsync(id)))
            {
                context.Result = new NotFoundObjectResult(new StandardErrorDto()
                {
                    Code = "ArticleNotExist",
                    Description = "Article specified doesn't exist."
                });
            } else
            {
                await next();
            }
        }
    }
}
