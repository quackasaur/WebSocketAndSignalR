using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using WebSocketServer.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<WebSocketServerConnManager>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseWebSockets();
app.UseWebSocketServer();
app.Run();


