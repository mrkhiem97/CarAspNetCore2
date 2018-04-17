using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace CarWebApi.WebSocketManager
{
    public class ChatMessageHandler : WebSocketHandler
    {
        public ChatMessageHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
        }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);

            var socketId = WebSocketConnectionManager.GetId(socket);
            await SendMessageToAllAsync($"{socketId} is now connected");
        }

        public override async Task OnConnected(string id, WebSocket socket)
        {
            await base.OnConnected(id, socket);

            var socketId = WebSocketConnectionManager.GetId(socket);
            await SendMessageToAllAsync($"{socketId} is now connected");
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var socketId = WebSocketConnectionManager.GetId(socket);
            var message = $"{socketId} said: {Encoding.UTF8.GetString(buffer, 0, buffer.Length)}";
            message = message.TrimEnd('\0');

            await SendMessageToAllAsync(message);
        }
    }
}
