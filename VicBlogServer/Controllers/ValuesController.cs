using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace VicBlogServer.Controllers
{

    [Route("/")]
    public class ValuesController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Redirect("/swagger");
        }

        //[HttpPost("Test")]
        //public async Task<IActionResult> Test()
        //{
        //    return Json(Request.Form.Select(x => new {  x.Key, x.Value }));
        //}
    }

}
