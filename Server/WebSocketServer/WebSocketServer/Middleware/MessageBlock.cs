﻿namespace WebSocketServer.Middleware;

public class MessageBlock
{
  public string? From { get; set; }
  public string? To { get; set; }
  public string? Message { get; set; }
}
