using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using VicBlogServer.Entitites;
using VicBlogServer.Models;
using VicBlogServer.Models.Dto;

namespace VicBlogServer.Controllers
{
    [Produces("application/json")]
    [Route("api/Article")]
    public abstract class ArticleControllerSpec : Controller
    {
        [HttpGet]
        [SwaggerOperation]
        [SwaggerResponse(200, type: typeof(List<ArticleBrief>), 
            description: "Filters articles")]
        public abstract Task<IActionResult> GetArticles();

        [HttpGet("Filter")]
        [SwaggerOperation]
        [SwaggerResponse(200, type: typeof(List<ArticleBrief>), description: "Filters articles")]
        public abstract Task<IActionResult> GetArticleFilter([FromQuery]ArticleFilterDto filter);


        [HttpGet("Tags")]
        [SwaggerOperation]
        [SwaggerResponse(200, type: typeof(List<string>),
            description: "Gets all tags")]
        public abstract Task<IActionResult> GetTags();

        [HttpGet("Categories")]
        [SwaggerOperation]
        [SwaggerResponse(200, type: typeof(List<string>),
            description: "Gets all Categories")]
        public abstract Task<IActionResult> GetCategories();

        [HttpGet("{articleId}")]
        [SwaggerOperation]
        [SwaggerResponse(200, type: typeof(Article),
            description: "Gets an Article from articleId")]
        [SwaggerResponse(400, type: typeof(Article),
            description: "Found no article with that articleId")]
        public abstract Task<IActionResult> GetAArticle([FromRoute]string articleId);
    }

    public class ArticleController : ArticleControllerSpec
    {
        private BlogContext context;
        public ArticleController(BlogContext context)
        {
            this.context = context;
        }

        public override async Task<IActionResult> GetAArticle([FromRoute] string articleId)
        {
            var article = await context.Articles.FindAsync(articleId);
            if (article == null)
            {
                return NotFound();
            }
            else
            {
                return Json(article);
            }

        }

        public override async Task<IActionResult> GetArticleFilter([FromQuery]ArticleFilterDto filter)
        {
            if (filter == null)
            {
                return BadRequest();
            }

            return Json(filter.Execute(context.Articles));
        }

        public override async Task<IActionResult> GetArticles()
        {
            return Json(context.Articles.Select(x => x));
        }


        public override async Task<IActionResult> GetCategories()
        {
            throw new NotImplementedException();
        }

        public override async Task<IActionResult> GetTags()
        {
            throw new NotImplementedException();
        }
    }
}
