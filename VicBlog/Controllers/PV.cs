using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VicBlog.Data;

namespace VicBlog.Controllers
{
    public partial class DefaultApiController : Controller
    {
        [HttpGet]
        [Route("/pv")]
        [SwaggerOperation("PVArticleIDGet")]
        [SwaggerResponse(200, type: typeof(int), description: "Returns current PV for a article.")]
        public IActionResult PVArticleIDGet([FromQuery(Name = "ID")]string articleID)
        {
            return Json(PV.GetPV(articleID, context));
        }

    }
}
