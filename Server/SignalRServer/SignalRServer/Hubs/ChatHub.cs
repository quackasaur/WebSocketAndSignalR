using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using SignalRServer.Models;

namespace SignalRServer.Hubs;

public class ChatHub: Hub
{
  public override Task OnConnectedAsync()
  {
    Console.WriteLine("Connection established: "+ Context.ConnectionId);
    Clients.Client(Context.ConnectionId).SendAsync("ReceiveConnID", Context.ConnectionId);
    return base.OnConnectedAsync();
  }

  public async Task SendMessageAsync(string message)
  {
    MessageBlock? desMessage = JsonSerializer.Deserialize<MessageBlock>(message);

    if (desMessage.To == String.Empty)
    {
      await Clients.All.SendAsync("ReceiveMessage", desMessage.Message);
    }
    else
    {
      await Clients.Client(desMessage.To).SendAsync("ReceiveMessage", desMessage.Message);
    }
  }
}
