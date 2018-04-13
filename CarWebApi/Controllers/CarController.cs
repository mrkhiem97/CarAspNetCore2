using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CarWebApi.AspNetCoreAttribute;
using CarWebApi.HttpHelper.HttpContent;
using CarWebApi.HttpHelper.HttpFileUploader;
using CarWebApi.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Linq;

namespace CarWebApi.Controllers
{
    [Route("api/[controller]")]
    public class CarController : Controller
    {
        private IHostingEnvironment hostingEnv;

        public CarController(IHostingEnvironment env)
        {
            this.hostingEnv = env;
        }

        // GET api/car
        [HttpGet]
        public IEnumerable<CarModel> Get()
        {
            List<CarModel> listCarModel = new List<CarModel>();
            listCarModel.Add(new CarModel() { ModelId = "123456", ModelName = "Nissan X-Trail 1", Version = 1234 });
            listCarModel.Add(new CarModel() { ModelId = "523451", ModelName = "Nissan X-Trail 2", Version = 3456 });
            listCarModel.Add(new CarModel() { ModelId = "423452", ModelName = "Nissan X-Trail 3", Version = 5435 });
            listCarModel.Add(new CarModel() { ModelId = "323453", ModelName = "Nissan X-Trail 4", Version = 3451 });
            return listCarModel;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public JsonResult Post([FromBody]CarModel model)
        {
            return Json(model);
        }

        // PUT api/values/
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpPost("PostDynamicJson")]
        public JObject PostDynamicJsonObject([FromBody]JObject jCarModel)
        {
            // dynamic input from inbound JSON
            dynamic carModel = jCarModel;

            // create a new JSON object to write out
            dynamic newCarModel = new JObject();

            // Create properties on the new instance
            // with values from the first
            newCarModel.ModelId = carModel.ModelId + " New";
            newCarModel.ModelName = "something new";
            newCarModel.Tags = new JArray();

            foreach (dynamic tag in carModel.Tags)
            {
                tag.TagName = tag.TagName + " New";
                newCarModel.Tags.Add(tag);
            }

            return newCarModel;
        }

        /// <summary>
        /// Upload small file
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost("UploadSmallFile")]
        public async Task<IActionResult> UploadSmallFile(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var filePath = Path.GetTempFileName();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size, filePath });
        }

        /// <summary>
        /// Upload small file using Ajax
        /// </summary>
        /// <returns></returns>
        [HttpPost("UploadSmallFilesAjax")]
        [DisableRequestSizeLimit]
        public IActionResult UploadSmallFilesAjax()
        {
            long size = 0;
            var files = Request.Form.Files;
            foreach (var file in files)
            {
                var filename = ContentDispositionHeaderValue
                                .Parse(file.ContentDisposition)
                                .FileName
                                .Trim('"');
                filename = hostingEnv.WebRootPath + $@"\{filename}";
                size += file.Length;
                using (FileStream fs = System.IO.File.Create(filename))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
            }
            string message = $"{files.Count} file(s) / { size } bytes uploaded successfully!";
            return Json(message);
        }

        /// <summary>
        /// Upload large file using Ajax
        /// </summary>
        /// <returns></returns>
        [HttpPost("UploadLargeFile")]
        [DisableFormValueModelBinding]
        [DisableRequestSizeLimit]
        //[RequestSizeLimit(100_000_000_000)]
        public async Task<IActionResult> UploadLargeFile()
        {
            FormValueProvider formModel;
            var streamStorage = new LocalStreamStorage(hostingEnv.WebRootPath);
            formModel = await Request.StreamFile(streamStorage);
            // formModel = await Request.StreamFile(stream);

            var viewModel = new CarModel();

            var bindingSuccessful = await TryUpdateModelAsync(viewModel, prefix: "", valueProvider: formModel);

            if (!bindingSuccessful)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
            }

            return Ok(viewModel);
        }

        /// <summary>
        /// Download file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [HttpGet("Download")]
        public async Task<FileResult> Download(string filename)
        {
            if (filename == null)
                return null;

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            var httpContentHelper = new HttpContentHelper();
            return File(memory, httpContentHelper.GetContentType(path), Path.GetFileName(path));
        }
    }
}
