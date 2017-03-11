using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VicBlog.Data;
using Swashbuckle.AspNetCore.SwaggerGen;
using VicBlog.Models;
using System.Collections;
using Microsoft.AspNetCore.Hosting;
using System.Security.Cryptography;

namespace VicBlog.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public partial class DefaultApiController : Controller
    {

        private readonly BlogContext context;
        private readonly IHostingEnvironment hostingEnv;
        public DefaultApiController(BlogContext context, IHostingEnvironment hostingEnv)
        {
            this.context = context;
            this.hostingEnv = hostingEnv;
        }

        [HttpPost]
        [Route("/upload")]
        public async Task<IActionResult> UploadPost([FromHeader]string token)
        {
            User user = null;
            try
            {
                user = await Utils.GetUserAsync(token,context);
            }
            catch (TokenOutdatedException)
            {
                return Forbid();
            }
            catch
            {
                return Unauthorized();
            }

            var path = Path.Combine(hostingEnv.ContentRootPath, "wwwroot", "upload", user.Username);
            var uploadedFiles = new List<string>();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var fileList = Request.Form.Files;
            foreach (var file in Request.Form.Files)
            {
                var filename = $"({DateTime.Now.ToUnixUTCTime()}){file.FileName}";
                var filePath = Path.Combine(path, filename);

                using (FileStream fs = new FileStream(filePath, System.IO.FileMode.Create))
                {
                    await file.CopyToAsync(fs);
                }
                uploadedFiles.Add($"upload/{user.Username}/{filename}");
            }

            return Created(path, Json(uploadedFiles));
        }

        [HttpDelete]
        [Route("/upload")]
        public async Task<IActionResult> UploadDelte([FromHeader]string token, [FromBody]string filename)
        {
            User user = null;
            try
            {
                user = await Utils.GetUserAsync(token,context);
            }
            catch (TokenOutdatedException)
            {
                return Forbid();
            }
            catch
            {
                return Unauthorized();
            }

            var path = Path.Combine(hostingEnv.ContentRootPath, "wwwroot", "upload", user.Username);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var fileList = Directory.GetFiles(path).Select(x => new FileInfo(x));
            var deletedFiles = new List<string>();
            foreach (FileInfo x in fileList)
            {
                var trueName = x.Name.Substring(x.Name.Split(')')[0].Length + 1);
                if (trueName == filename)
                {
                    deletedFiles.Add(x.Name);
                    x.Delete();
                }
            }
            return Json(deletedFiles);


        }


        [HttpGet]
        [Route("/tags")]
        [SwaggerOperation("TagsGet")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerResponse(200, type: typeof(List<string>), description: "Returns all existing tags.")]
        public IActionResult TagsGet()
        {
            return Json(context.TagLinks.Select(x => x.TagName).Distinct());
        }

        [HttpGet]
        [Route("/categories")]
        [SwaggerOperation("CategoriesGet")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerResponse(200, type: typeof(List<string>), description: "Returns all categories existing.")]
        public IActionResult CategoriesGet()
        {
            return Json(context.ArticleBriefs.Select(x => x.Category).Distinct());
        }

        /// <summary>
        /// Delete the article
        /// </summary>
        /// <remarks>Delete the article</remarks>
        /// <param name="articleID">Article ID</param>
        /// <param name="token">Valid admin token</param>
        /// <response code="201">Deleted successfully.</response>
        /// <response code="401">Token Not valid.</response>
        /// <response code="404">Article Not found</response>
        /// <response code="0">Failed</response>
        [HttpDelete]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("/articles/{articleID}")]
        [SwaggerOperation("ArticlesArticleIDDelete")]
        [SwaggerResponse(200, type: typeof(Article), description: "Deleted successfully. Returns the article.")]
        [SwaggerResponse(401, description: "User token is invalid or operator is not the author or operator is not an admin.")]
        [SwaggerResponse(403, description: "Token outdated.")]
        [SwaggerResponse(404, description: "Article specified by the articleID is not found.")]
        public async Task<IActionResult> ArticlesArticleIDDelete([FromRoute]string articleID, [FromHeader]string token)
        {
            User user = null;
            try
            {
                user = await Utils.GetUserAsync(token,context);
            }
            catch (TokenOutdatedException)
            {
                return Forbid();
            }
            catch
            {
                return Unauthorized();
            }
            var article = await context.Articles.FindAsync(articleID);
            if (article == null)
            {
                return NotFound();
            }

            var brief = await context.ArticleBriefs.FindAsync(articleID);

            context.ArticleBriefs.Remove(brief);

            context.Articles.Remove(article);

            context.TagLinks.RemoveRange(context.TagLinks.Where(x => x.ArticleID == articleID));


            await context.SaveChangesAsync();
            return Json(article);
        }


        /// <summary>
        /// Get the article by ID
        /// </summary>
        /// <remarks>Returns an Article according to the ID provided</remarks>
        /// <param name="articleID">Article ID</param>
        /// <response code="200">Success</response>
        /// <response code="404">Article Not found</response>
        /// <response code="0">Failed</response>
        [HttpGet]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("/articles/{articleID}")]
        [SwaggerOperation("ArticlesArticleIDGet")]
        [SwaggerResponse(200, type: typeof(ArticleRequestModel), description: "Returns the article specified by the articleID")]
        [SwaggerResponse(404, description: "Article specified by the articleID is not found.")]

        public IActionResult ArticlesArticleIDGet([FromRoute]string articleID)
        {
            ArticleBrief brief = context.ArticleBriefs.Find(articleID);

            if (brief == null)
            {
                return NotFound();
            }

            context.ArticlePVs.Add(new ArticlePVModel()
            {
                ArticleID = articleID,
                IP = Request.GetIPAddress(),
                ViewTime = DateTime.Now
            });
            context.SaveChangesAsync();
          
            var result = new ArticleRequestModel(
                brief.LoadTheRest(context),
                context.Articles.Find(articleID).Content
            );

            return Json(result);

        }

        [HttpGet]
        [Route("/pv")]
        [SwaggerOperation("PVArticleIDGet")]
        [SwaggerResponse(200, type: typeof(int), description: "Returns current PV for a article.")]
        public IActionResult PVArticleIDGet([FromQuery(Name = "ID")]string articleID)
        {
            return Json(context.ArticlePVs.Where(x => x.ArticleID == articleID).Count());
        }


        /// <summary>
        /// Modify the article
        /// </summary>
        /// <remarks>Modify the article</remarks>
        /// <param name="articleID">Article ID</param>
        /// <param name="token">Valid JWT</param>
        /// <param name="newArticle">New info</param>
        /// <response code="200">Successfully modified</response>
        /// <response code="400">New article in wrong form</response>
        /// <response code="401">Not authorized</response>
        /// <response code="404">Article not found</response>
        /// <response code="0">Failed</response>
        [HttpPatch]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("/articles/{articleID}")]
        [SwaggerOperation("ArticlesArticleIDPatch")]
        [SwaggerResponse(200, type: typeof(ArticleRequestModel), description: "Patched Successfully. Returns new Article.")]
        [SwaggerResponse(404, description: "Article specified by articleID is not found")]
        [SwaggerResponse(403, description: "Token outdated.")]
        [SwaggerResponse(401, description: "User token is invalid or operator is not the author or operator is not an admin.")]
        public async Task<IActionResult> ArticlesArticleIDPatch([FromHeader]string token, [FromRoute]string articleID, [FromBody]ArticlePatchingModel newArticle)
        {
            User user = null;
            try
            {
                user = await Utils.GetUserAsync(token,context);
            }
            catch (TokenOutdatedException)
            {
                return Forbid();
            }
            catch
            {
                return Unauthorized();
            }

            ArticleBrief brief = await context.ArticleBriefs.FindAsync(articleID);
            Article article = await context.Articles.FindAsync(articleID);
            if (brief == null)
            {
                return NotFound();
            }
            if (user.Role != Role.Admin && user.Username != brief.Username)
            {
                return Unauthorized();
            }

            if (newArticle == null)
            {
                return BadRequest();
            }

            article.Content = newArticle.Content;
            brief.Title = newArticle.Title;
            brief.Category = newArticle.Category;
            context.TagLinks.RemoveRange(context.TagLinks.Where(x => x.ArticleID == articleID));
            context.TagLinks.AddRange(newArticle.Tags.Select(x=> new TagLink() { ArticleID = articleID, TagName = x }));

            brief.LastEditedTime = DateTime.Now;
            await context.SaveChangesAsync();



            return Ok(new ArticleRequestModel(brief.LoadTheRest(context), article.Content));

        }

        [HttpPost]
        [Route("/articles/filter")]
        [SwaggerOperation("ArticleFilterPost")]
        [SwaggerResponse(200, type: typeof(List<ArticleBriefRequestModel>), description: "Returns filtered ArticleBriefs.")]
        public IActionResult ArticleFilterPost([FromBody]ArticleFilter filter)
        {
            if (filter == null) return BadRequest();

            var allData = context.ArticleBriefs.Select(x => x);

            if (filter.Categories != null && filter.Categories.Any())
            {
                allData = allData.Where(x => filter.Categories.Contains(x.Category));

            }
            if (!string.IsNullOrEmpty(filter.TitleText))
            {
                allData = allData.Where(x => x.Title.Contains(filter.TitleText));
            }

            var list = allData.ToList().Select(x => x.LoadTheRest(context));




            if (filter.Tags != null && filter.Tags.Any())
            {

                list = list.Where(x => x.Tags.Intersect(filter.Tags).Any());

            }

            switch (filter.Order)
            {
                case ArticleBriefListOrder.SubmitEarliestToLatest:
                    list = list.OrderBy(x => x.SubmitTime);
                    break;
                case ArticleBriefListOrder.SubmitLatestToEarliest:
                    list = list.OrderByDescending(x => x.SubmitTime);
                    break;
                case ArticleBriefListOrder.LastEditedEarliestToLatest:
                    list = list.OrderBy(x => x.LastEditedTime);
                    break;
                case ArticleBriefListOrder.LastEditedLatestToEarlist:
                    list = list.OrderByDescending(x => x.LastEditedTime);
                    break;
                case ArticleBriefListOrder.RankHighestToLowest:
                    list = list.OrderByDescending(x => x.Rate);
                    break;
                case ArticleBriefListOrder.RankLowestToHighest:
                    list = list.OrderBy(x => x.Rate);
                    break;
            }

            return Json(list.Select(x => new ArticleBriefRequestModel(x)));
        }

        [HttpGet]
        [Produces("application/json")]
        [Route("/articles")]
        [SwaggerOperation("ArticlesGet")]
        [SwaggerResponse(200, type: typeof(List<ArticleBriefRequestModel>), description: "Returns all filtered ArticleBriefs.")]
        public virtual IActionResult ArticlesGet()
        {
            return Json(context.ArticleBriefs.ToList().Select(x =>
            {
                return new ArticleBriefRequestModel(x.LoadTheRest(context));
            }));
        }

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
                user = await Utils.GetUserAsync(token,context);
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



        /// <summary>
        /// Delete a comment
        /// </summary>
        /// <remarks>Delete a comment</remarks>
        /// <param name="commentID">Comment ID</param>
        /// <param name="token">Valid JWT of author or admin</param>
        /// <response code="200">Deleted successfully.</response>
        /// <response code="401">Not authorized or not author or admin</response>
        /// <response code="0">Failed</response>
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
                user = await Utils.GetUserAsync(token,context);
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



        /// <summary>
        /// Get a comment
        /// </summary>
        /// <remarks>Get a comment by ID</remarks>
        /// <param name="commentID">Comment ID</param>
        /// <response code="200">Get successfully.</response>
        /// <response code="404">Not found.</response>
        /// <response code="0">Failed</response>
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


        /// <summary>
        /// Get all comments
        /// </summary>
        /// <remarks>Returns an array containing all comments of the Article</remarks>
        /// <param name="articleID">Article ID</param>
        /// <response code="200">Get successfully.</response>
        /// <response code="404">ID not existing</response>
        /// <response code="0">Failed.</response>
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



        /// <summary>
        /// Post a new comment
        /// </summary>
        /// <remarks>Creates a new comments on the article.</remarks>
        /// <param name="articleID">Article ID</param>
        /// <param name="data">Formatted as DataOnCommentCreating</param>
        /// <response code="201">Comment posted successfully.</response>
        /// <response code="401">Not authorized or access denied</response>
        /// <response code="404">Article Not Found</response>
        /// <response code="0">Failed</response>
        [HttpPost]
        [Route("/comments")]
        [SwaggerOperation("CommentsArticleIDPost")]
        [SwaggerResponse(201, description: "Comment has been successfully created. Returns the comment.")]
        [SwaggerResponse(404, description: "Article specified by articleID is not found.")]
        [SwaggerResponse(401, description: "User token is not valid.")]
        [SwaggerResponse(403, description: "Token outdated.")]
        public async Task<IActionResult> CommentsArticleIDPost([FromHeader]string token, [FromBody]CommentCreatingModel data)
        {
            User user = null;
            try
            {
                user = await Utils.GetUserAsync(token,context);
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

        /// <summary>
        /// Post a new Article
        /// </summary>
        /// <remarks>Creates a new Article</remarks>
        /// <params name="data">Formatted as DataOnArticleCreating</params>
        /// <response code="201">Created successfully.</response>
        /// <response code="401">User not admin</response>
        /// <response code="0">Failed</response>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("/articles")]
        [SwaggerOperation("ArticlesArticlePOST")]
        [SwaggerResponse(201, type: typeof(ArticleRequestModel), description: "Created successfully. Returns new article.")]
        [SwaggerResponse(401, description: "User token is not valid or user is not an admin.")]
        [SwaggerResponse(403, description: "Token outdated.")]
        public async Task<IActionResult> ArticlesArticlePOST([FromHeader]string token, [FromBody]ArticleCreatingModel data)
        {
            User user = null;
            try
            {
                user = await Utils.GetUserAsync(token, context);
            }
            catch (TokenOutdatedException)
            {
                return Forbid();
            }
            catch
            {
                return Unauthorized();
            }

            string guid = Guid.NewGuid().ToString();

            DateTime now = DateTime.Now;

            ArticleBrief brief = new ArticleBrief()
            {
                ID = guid,
                Username = user.Username,
                LastEditedTime = now,
                SubmitTime = now,
                Title = data.Title,
                Category = data.Category
            };

            context.ArticleBriefs.Add(brief);

            Article article = new Article()
            {
                ID = guid,
                Content = data.Content
            };
            context.Articles.Add(article);


            context.TagLinks.AddRange(data.Tags.Select(x => new TagLink() { ArticleID = guid, TagName = x }));



            Rate rate = new Rate()
            {
                ArticleID = guid,
                Score = data.Rate,
                Username = user.Username,
                SubmitTime = now
            };

            context.Rates.Add(rate);



            await context.SaveChangesAsync();

            return Created("/articles/" + guid, new ArticleRequestModel(brief.LoadTheRest(context), article.Content));
        }


        /// <summary>
        /// Login
        /// </summary>
        /// <remarks>Attempts to login with provided credentials and returns a JWT if successful.</remarks>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <response code="200">Login Successful.</response>
        /// <response code="401">Login Invalid.</response>
        /// <response code="0">Failed</response>
        [HttpGet]
        [Route("/login")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerOperation("LoginGet")]
        [SwaggerResponse(200, type: typeof(User), description: "Login successful. Returns token, username, registerTime and role.")]
        [SwaggerResponse(401, description: "Credential provided is not valid.")]
        public IActionResult LoginGet([FromQuery]UserLoginModel data)
        {
            var user = context.Users.Find(data.Username);
            if (user == null || user.Password != data.Password)
            {
                return Unauthorized();
            }

            var now = DateTime.Now;

            UserLoginSuccessModel returnData = new UserLoginSuccessModel()
            {
                Username = user.Username,
                Role = user.Role,
                Token = user.ComputeToken(now),
                LoginTime = now
            };

            return Json(returnData);
        }


        /// <summary>
        /// Register
        /// </summary>
        /// <remarks>Attempts to register with provided information.</remarks>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <response code="200">Register Successful.</response>
        /// <response code="0">Failed. Read description for details.</response>
        [HttpPost]
        [Route("/register")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerOperation("RegisterPost")]
        [SwaggerResponse(409, description: "Username exists.")]
        [SwaggerResponse(201, type: typeof(UserLoginSuccessModel), description: "User registered successfully. Returns user info.")]
        public async Task<IActionResult> RegisterPost([FromBody]UserRegisterModel data)
        {
            var user = await context.Users.FindAsync(data.Username);
            if (user != null)
            {
                return StatusCode(409);
            }

            User newUser = new Models.User()
            {
                Password = data.Password.ComputeMD5(),
                Username = data.Username,
                RegisterTime = DateTime.Now,
                Role = Role.User,
            };


            context.Users.Add(newUser);
            await context.SaveChangesAsync();

            UserLoginSuccessModel model = new UserLoginSuccessModel()
            {
                Username = newUser.Username,
                Role = newUser.Role,
                LoginTime = DateTime.Now,
                Token = newUser.ComputeToken(DateTime.Now)
            };

            return Created("/users/" + data.Username, model);


        }

        [HttpGet]
        [Route("/users/{username}")]
        [SwaggerOperation("RegisterPost")]
        [SwaggerResponse(200, type: typeof(User), description: "Returns user profile.")]
        [SwaggerResponse(404, description: "Username not found!")]
        public IActionResult UsersUsernameGet([FromRoute]string username)
        {
            var user = context.Users.Find(username);
            if (user != null)
            {
                return Json(user);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("/")]
        [SwaggerOperation("Index")]
        [SwaggerResponse(200)]
        public IActionResult Index()
        {
            return Redirect("/swagger");
        }
    }
}
