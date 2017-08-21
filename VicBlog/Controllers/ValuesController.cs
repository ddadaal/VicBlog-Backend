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
using Microsoft.AspNetCore.Http;

namespace VicBlog.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public partial class DefaultApiController : Controller
    {

        private readonly BlogContext context;
        private readonly IHostingEnvironment hostingEnv;
        private readonly Data.Qiniu qiniu;
        public DefaultApiController(BlogContext context, IHostingEnvironment hostingEnv)
        {
            this.context = context;
            this.hostingEnv = hostingEnv;
            this.qiniu = new Data.Qiniu();
        }

        [HttpPost]
        [Route("/upload")]
        public async Task<IActionResult> UploadPost([FromHeader]string token, [FromForm]List<IFormFile> files)
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

            var uploadedFiles = new List<string>();

            foreach (var file in Request.Form.Files)
            {
                var response = await qiniu.UploadFileAsync(file, user.Username);
                uploadedFiles.Add(response.Success
                    ? response.AccessUrl
                    : "error "+response.Error);
            }

            return Created(Data.Qiniu.PostUrl, Json(uploadedFiles));
        }

        [HttpGet]
        [Route("/")]
        [SwaggerOperation("Index")]
        [SwaggerResponse(200)]
        public IActionResult Index()
        {
            return Redirect("/swagger");
        }

        [HttpGet]
        [Route("/test")]
        [SwaggerOperation("TEST")]
        [SwaggerResponse(200)]
        public IActionResult TEST()
        {
            return Json(Utils.LOGIN_EXPIRE_SECONDS);
        }
    }
}
