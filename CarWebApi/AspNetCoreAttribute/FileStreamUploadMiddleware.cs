using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CarWebApi.AspNetCoreAttribute
{
    public class FileStreamUploadMiddleware
    {
        private readonly RequestDelegate _next;

        public FileStreamUploadMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.ContentType != null)
            {
                if (context.Request.Headers.Any(x => x.Key == "Content-Disposition"))
                {
                    ContentDispositionHeaderValue v = ContentDispositionHeaderValue.Parse(context.Request.Headers.First(x => x.Key == "Content-Disposition").Value);
                    if (HasFileContentDisposition(v))
                    {
                        var memoryStream = new MemoryStream();
                        context.Request.Body.CopyTo(memoryStream);
                        var length = memoryStream.Length;
                        var formCollection = context.Request.Form =
                          new FormCollection(new Dictionary<string, StringValues>(),
                          new FormFileCollection() { new FormFile(memoryStream, 0, length, v.Name, v.FileName) });
                    }
                }
            }

            await _next.Invoke(context);
        }

        #region Helper

        public static bool HasFileContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            return contentDisposition != null
                   && contentDisposition.DispositionType.Equals("form-data")
                   && (!string.IsNullOrEmpty(contentDisposition.FileName)
                       || !string.IsNullOrEmpty(contentDisposition.FileNameStar));
        }
        #endregion
    }

            // Extension method used to add the middleware to the HTTP request pipeline.
        public static class FileStreamUploadMiddlewareExtensions
        {
            public static IApplicationBuilder UseFileStreamUploadMiddleware(this IApplicationBuilder builder)
            {
                return builder.UseMiddleware<FileStreamUploadMiddleware>();
            }
        }
}
