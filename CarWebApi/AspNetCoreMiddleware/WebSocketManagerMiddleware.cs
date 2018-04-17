using CarWebApi.WebSocketManager;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CarWebApi.AspNetCoreMiddleware
{
    public static class WebSocketManagerMiddleExtension
    {
        public static IApplicationBuilder MapWebSocketManager(this IApplicationBuilder app, PathString path, WebSocketHandler handler)
        {
            return app.Map(path, (_app) =>
            {
                _app.UseMiddleware<WebSocketManagerMiddleware>(handler);
            });
        }

        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient<WebSocketConnectionManager>();

            foreach (var type in Assembly.GetEntryAssembly().ExportedTypes)
            {
                if (type.GetTypeInfo().BaseType == typeof(WebSocketHandler))
                {
                    services.AddSingleton(type);
                }
            }

            return services;
        }
    }

    /// <summary>
    /// WebSocketManagerMiddleware
    /// </summary>
    public class WebSocketManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private WebSocketHandler _webSocketHandler { get; set; }

        public WebSocketManagerMiddleware(RequestDelegate next, WebSocketHandler webSocketHandler)
        {
            _next = next;
            _webSocketHandler = webSocketHandler;
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next.Invoke(context);
            }

            var userId = context.Request.Path.Value.Replace("/", "", StringComparison.CurrentCultureIgnoreCase);

            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await _webSocketHandler.OnConnected(userId, webSocket);
            await Receive(webSocket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    await _webSocketHandler.ReceiveAsync(webSocket, result, buffer);
                    return;
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocketHandler.OnDisconnected(webSocket);
                    return;
                }
                else
                {
                    // Do nothing
                }
            });
        }


        /// <summary>
        /// Echo
        /// </summary>
        /// <param name="context"></param>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        private async Task Receive(WebSocket webSocket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var buffer = new byte[1024 * 4];
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                handleMessage(result, buffer);
                // var type = result.MessageType;
                // var str = Encoding.Default.GetString(buffer);
                // await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                // result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            //await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
