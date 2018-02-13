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

        IActionResult ArgumentInvalid => new BadRequestObjectResult(new StandardErrorDto()
        {
            Code = "ArgumentInvalid",
            Description = "Article id is not a string or is not attached on querystring."
        });

        IActionResult ArticleNotExist => new NotFoundObjectResult(new StandardErrorDto()
        {
            Code = "ArticleNotExist",
            Description = "Article specified doesn't exist."
        });

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var articleService = context.RequireService<IArticleDataService>();

            var articleId = context.ModelState[key].RawValue as string;

            if (articleId == null)
            {
                context.Result = ArgumentInvalid;
            }
            else if (await articleService.FindByIdAsync(articleId) == null)
            {
                context.Result = ArticleNotExist;
            }
            else
            {
                await next();
            }




        }
    }
}
