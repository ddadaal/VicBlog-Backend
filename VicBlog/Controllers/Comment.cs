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
        [HttpDelete]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("/comments")]
        [SwaggerOperation("CommentsDelete")]
        [SwaggerResponse(200, type: typeof(Comment), description: "Deletion request has been received.")]
        [SwaggerResponse(401, description: "User token is not valid or the operator is not an admin. ")]
        [SwaggerResponse(404, description: "Comment specified by commentID is not found.")]
        [SwaggerResponse(403, description: "Token outdated.")]
        public async Task<IActionResult> CommentsDelete([FromQuery]string commentID, [FromHeader]string token)
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

            var comment = await context.Comments.FindAsync(commentID);
            if (comment == null)
            {
                return NotFound();
            }
            context.Comments.Remove(comment);
            await context.SaveChangesAsync();
            return Json(comment);
        }

        [HttpGet]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("/comments/{commentID}")]
        [SwaggerOperation("CommentsGet")]
        [SwaggerResponse(200, type: typeof(Comment), description: "Returns the comment specified by ID.")]
        [SwaggerResponse(404, description: "Comment specified by commentID is not found.")]
        public async virtual Task<IActionResult> CommentsGet([FromRoute]string commentID)
        {
            var result = await context.Comments.FindAsync(commentID);
            if (result == null)
            {
                return NotFound();
            }
            else
            {
                return Json(result);
            }
        }

        [HttpGet]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("/comments")]
        [SwaggerOperation("CommentsArticleIDGet")]
        [SwaggerResponse(200, type: typeof(List<Comment>), description: "Returns all comments under the article specified by articleID")]
        public virtual IActionResult CommentsArticleIDGet([FromQuery]string articleID)
        {
            var result = context.Comments.Where(x => x.ArticleID == articleID).ToArray();
            return Json(result);
        }

 
        [HttpPost]
        [Route("/comments")]
        [SwaggerOperation("CommentsArticleIDPost")]
        [SwaggerResponse(201, description: "Comment has been successfully created. Returns the comment.")]
        [SwaggerResponse(404, description: "Article specified by articleID is not found.")]
        [SwaggerResponse(401, description: "User token is not valid.")]
        [SwaggerResponse(403, description: "Token outdated.")]
        public async Task<IActionResult> CommentsArticleIDPost([FromHeader]string token, [FromBody]CommentCreationModel data)
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

            if (await context.Articles.FindAsync(data.ArticleID) == null)
            {
                return NotFound();
            }

            Comment comment = new Comment()
            {
                ID = Guid.NewGuid().ToString(),
                Content = data.Content,
                ArticleID = data.ArticleID,
                ReplyTo = data.ReplyToCommentID,
                SubmitTime = DateTime.Now,
                Username = user.Username
            };


            context.Comments.Add(comment);
            await context.SaveChangesAsync();

            return Created("/articles/" + comment.ID, comment);
        }
    }
}
