using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using CarWebApi.HttpHelper.ActionResultExtension;
using CarWebApi.HttpHelper.HttpContent;

namespace CarWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Video")]
    public class VideoController : Controller
    {
        // GET: api/Video

        /// <summary>
        /// Get list of videos filename
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            List<string> listFilename = new List<string>();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var files = System.IO.Directory.GetFiles(path, "*.mp4");

            foreach (var file in files)
            {
                listFilename.Add(Path.GetFileName(file));
            }

            return listFilename;
        }

        /// <summary>
        /// Play video async support seeking
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [HttpGet("PlayVideoAsync")]
        public VideoStreamResult PlayVideoAsync(string filename)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filename);
            var httpContentHelper = new HttpContentHelper();
            return new VideoStreamResult(new FileInfo(path).OpenRead(), httpContentHelper.GetContentType(path));
        }
    }
}
