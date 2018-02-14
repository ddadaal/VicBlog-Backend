using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VicBlogServer.ViewModels.Dto
{

    public class ErrorDto
    {
        public string Code { get; set; }
    }

    public class StandardErrorDto : ErrorDto
    {
        public string Description { get; set; }
    }

    public class MultipleErrorsDto : ErrorDto
    {
        public IEnumerable<string> ErrorDescriptions { get; set; }
    }
}
