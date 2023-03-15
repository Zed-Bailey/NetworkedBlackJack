using BlackJack.Server;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

// add game repo to track game state
builder.Services.AddSingleton<GameRepository>();

// add the background game service
builder.Services.AddHostedService<GameService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// http://localhost:5259/blackjack
app.MapHub<BlackJackHub>("/blackjack");

app.Run();