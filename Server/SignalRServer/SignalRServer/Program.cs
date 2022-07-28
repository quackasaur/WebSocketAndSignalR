using SignalRServer.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseRouting();
// Configure the HTTP request pipeline.
app.UseCors(builder => builder
  .WithOrigins("null")
  .AllowAnyHeader()
  .AllowAnyMethod()
  .AllowCredentials());

app.UseEndpoints(endpoints =>
{
  endpoints.MapHub<ChatHub>("/chatHub");
});

app.Run();
