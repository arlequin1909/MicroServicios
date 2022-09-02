using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Subastalo.Models.File
{
    public class FileRequeride
    {
        public IFormFile IMG { get; set; }

    }

    public class ResponseUrl
    {
        public string URL { get; set; }

    }
}
