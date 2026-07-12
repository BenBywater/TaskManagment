using System.Threading.Channels;
using Microsoft.OpenApi;
using TaskManagement.NotificationService.Models;

var builder = WebApplication.CreateBuilder(args);

// Register the channel as a singleton — both the controller (writer)
// and the BackgroundService (reader) must share the same instance
builder.Services.AddSingleton(Channel.CreateUnbounded<NotificationMessage>());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
