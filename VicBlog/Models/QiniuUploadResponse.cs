using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VicBlog.Models
{
    public class QiniuUploadResponse
    {
        public bool Success { get; set; }
        public string AccessUrl { get; set; }
        public string Error { get; set; }
    }
}
