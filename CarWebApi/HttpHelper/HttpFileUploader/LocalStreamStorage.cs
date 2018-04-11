using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CarWebApi.HttpHelper.HttpFileUploader
{
    public class LocalStreamStorage
    {
        public string hostEnvironment { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="hostEnvironment"></param>
        public LocalStreamStorage(string hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }

        public Stream GetLoalStream(string filename)
        {
            var fullFileName = $@"{ hostEnvironment }\{ Guid.NewGuid() }_{ filename }";
            return File.Create(fullFileName);
        }
    }
}
