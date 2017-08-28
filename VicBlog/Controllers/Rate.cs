using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VicBlog.Models;

namespace VicBlog.Controllers
{
    public partial class DefaultApiController : Controller
    {
        [HttpGet]
        [Route("/rate/{articleID}")]
        [SwaggerOperation("RateArticleIDGet")]
        [SwaggerResponse(200, type: typeof(int), description: "Returns the average rate score of this article.")]
        [SwaggerResponse(404, description: "Article specified by the articleID is not found.")]
        public async Task<IActionResult> RateArticleIDGet([FromRoute]string articleID)
        {
            var brief = await context.ArticleBriefs.FindAsync(articleID);
            if (brief == null)
            {
                return NotFound();
            }
            return Json(brief.Rate);
        }
        [HttpPost]
        [Route("/rate/{articleID}")]
        [SwaggerOperation("RateArticleIDPost")]
        [SwaggerResponse(200, type: typeof(int), description: "Rated or patched successfully. Returns the new average score.")]
        [SwaggerResponse(401, description: "User token is not valid.")]
        [SwaggerResponse(400, description: "Score must be within 0 to 5.")]
        [SwaggerResponse(404, description: "Article specified by the articleID is not found.")]
        [SwaggerResponse(403, description: "Token outdated.")]
        public async Task<IActionResult> RateArticleIDPost([FromRoute]string articleID, [FromHeader]string token, [FromBody]RatePostModel model)
        {

            User user = null;
            try
            {
                user = Utils.GetUser(token, context);
            }
            catch (TokenOutdatedException)
            {
                return Forbid();
            }
            catch
            {
                return Unauthorized();
            }


            var brief = await context.ArticleBriefs.FindAsync(articleID);
            if (brief == null)
            {
                return NotFound();
            }

            if (model.Score <= 0 || model.Score > 5)
            {
                return BadRequest();
            }

            var existing = context.Rates.Where(x => x.Username == user.Username);
            context.Rates.RemoveRange(existing);

            context.Rates.Add(new Rate()
            {
                ArticleID = articleID,
                Score = model.Score,
                Username = user.Username
            });

            await context.SaveChangesAsync();

            return Json(brief.LoadRate(context));
        }
    }
}
