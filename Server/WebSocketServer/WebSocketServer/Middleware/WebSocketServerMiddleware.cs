using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebSocketServer.Middleware;

public class WebSocketServerMiddleware
{
  private readonly RequestDelegate _next;
  private readonly WebSocketServerConnManager _manager;

  public WebSocketServerMiddleware(RequestDelegate next, WebSocketServerConnManager manager)
  {
    _next = next;
    _manager = manager;
  }

  public async Task SendConnId(WebSocket socket, string connId)
  {
    var buffer = Encoding.UTF8.GetBytes($"ConnID: {connId}");
    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
  }

  public async Task Invoke(HttpContext context)
  {
    if (context.WebSockets.IsWebSocketRequest)
    {
      WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();
      Console.WriteLine("Websocket connected");

      string conn = _manager.AddSocket(socket);
      await SendConnId(socket, conn);

      await ReceieveMessage(socket,  async (result, bytes) =>
      {
        if (result.MessageType == WebSocketMessageType.Text)
        {
          var receivedMessage = Encoding.UTF8.GetString(bytes, 0, result.Count);
          Console.WriteLine($"Message received: {receivedMessage}");
          await RouteMessage(receivedMessage);
          return;
        }
        else if (result.MessageType == WebSocketMessageType.Close)
        {
          var socId = _manager.GetAllSockets().FirstOrDefault(m => m.Value == socket).Key;
          _manager.GetAllSockets().TryRemove(socId, out _);

          await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
          return;
        }
      });
    }
    else
    {
      await _next(context);
    }
  }
  public async Task ReceieveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
  {
    var buffer = new byte[1024 * 4];
    while (socket.State == WebSocketState.Open)
    {
      var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
      handleMessage(result, buffer);
    }
  }

  public async Task RouteMessage(string message)
  {
    var desMessage = JsonSerializer.Deserialize<MessageBlock>(message);

    if (Guid.TryParse(desMessage.To, out Guid socketGuid))
    {
      var socket = _manager.GetAllSockets().FirstOrDefault(m => m.Key == desMessage.To).Value;
      if (socket.State == WebSocketState.Open)
      {
          await socket.SendAsync(Encoding.UTF8.GetBytes(desMessage.Message), WebSocketMessageType.Text, true, CancellationToken.None);
      }
      else
      {
        Console.WriteLine("Recipient not found");
      }
    }
    else
    {
      Console.WriteLine("Broadcasting");
      var buffer = Encoding.UTF8.GetBytes(desMessage.Message);
      foreach (var uniquWebSocket in _manager.GetAllSockets())
      {
        await uniquWebSocket.Value.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
      }
    }
  }
}
