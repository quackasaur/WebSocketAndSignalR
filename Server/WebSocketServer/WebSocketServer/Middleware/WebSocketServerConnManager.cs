using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace WebSocketServer.Middleware;

public class WebSocketServerConnManager
{
  private ConcurrentDictionary<string, WebSocket> _socketsDict = new ConcurrentDictionary<string, WebSocket>();

  public ConcurrentDictionary<string, WebSocket> GetAllSockets()
  {
    return _socketsDict;
  }

  public string AddSocket(WebSocket socket)
  {
    var connId = Guid.NewGuid().ToString();
    _socketsDict.TryAdd(connId, socket);

    return connId;
  }
}
