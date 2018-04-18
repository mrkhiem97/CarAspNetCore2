using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CarWebApi.HttpHelper.ActionResultExtension
{
    public class VideoStreamResult : FileStreamResult
    {
        /// <summary>
        /// Buffer Size
        /// </summary>
        private const int BUFFER_SIZE = 65536;

        public VideoStreamResult(Stream fileStream, string contentType) : base(fileStream, contentType)
        {
        }

        public VideoStreamResult(Stream fileStream, MediaTypeHeaderValue contentType) : base(fileStream, contentType)
        {
        }

        private bool IsRangeRequest(RangeHeaderValue range)
        {
            return range != null && range.Ranges != null && range.Ranges.Count > 0;
        }

        /// <summary>
        /// Write video stream to response body
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        protected async Task WriteVideoStreamToBody(HttpRequest request, HttpResponse response)
        {
            var bufferingFeature = response.HttpContext.Features.Get<IHttpBufferingFeature>();
            bufferingFeature?.DisableResponseBuffering();

            RangeHeaderValue rangeHeader = request.GetTypedHeaders().Range;
            if (IsRangeRequest(rangeHeader))
            {
                long totalLength = this.FileStream.Length;
                var range = rangeHeader.Ranges.First();
                var start = range.From ?? 0;
                var end = range.To ?? totalLength - 1;

                response.Headers.Add("Accept-Ranges", "bytes");
                // response.GetTypedHeaders().ContentRange = new ContentRangeHeaderValue(start, end, totalLength);
                response.Headers.Add("Content-Range", $"bytes {start}-{end}/{totalLength}");
                response.ContentLength = end - start + 1;
                response.StatusCode = StatusCodes.Status206PartialContent;


                // Read video by range header
                try
                {
                    var buffer = new byte[BUFFER_SIZE];

                    var position = start;
                    var bytesLeft = end - start + 1;
                    this.FileStream.Seek(position, SeekOrigin.Begin);
                    while (position <= end)
                    {
                        var bytesRead = this.FileStream.Read(buffer, 0, (int)Math.Min(bytesLeft, buffer.Length));
                        await response.Body.WriteAsync(buffer, 0, bytesRead);
                        position += bytesRead;
                        bytesLeft = end - position + 1;
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    await response.Body.FlushAsync();
                    return;
                }
                finally
                {
                    await response.Body.FlushAsync();
                }
            }
            else
            {
                await this.FileStream.CopyToAsync(response.Body);
            }
        }

        /// <summary>
        /// ExecuteResultAsync
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            await this.WriteVideoStreamToBody(context.HttpContext.Request, context.HttpContext.Response);
        }
    }
}
