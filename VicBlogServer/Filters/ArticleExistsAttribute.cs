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
        const string key = "articleId";

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var articleService = context.RequireService<IArticleDataService>();

            var raw = context.ModelState[key].RawValue;

            if (!int.TryParse(raw.ToString(), out int s))
            {
                context.Result = new BadRequestObjectResult(new StandardErrorDto()
                {
                    Code = "ArgumentInvalid",
                    Description = "Article id is not a int."
                });
            } else if (await articleService.FindByIdAsync(s) == null)
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
