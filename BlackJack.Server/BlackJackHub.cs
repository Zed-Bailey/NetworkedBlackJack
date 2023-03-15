using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using BlackJack.Common;

namespace BlackJack.Server;

public class BlackJackHub : Hub
{
    private readonly GameRepository _gameRepository;
    
    public BlackJackHub(GameRepository repository)
    {
        _gameRepository = repository;
    }

    public async Task ConnectNewPlayer(string userName) {
        var connectionId = Context.ConnectionId;
        _gameRepository.AddPlayer(new Player(userName, connectionId));
        await Clients.Client(connectionId).SendAsync("ReceiveNewConnection", connectionId, _gameRepository.GameInProgress);
        await SendChatMessage("SYSTEM", $"{userName} joined");
    }



    /// <summary>
    /// Sends a message to all clients when a user sends a message
    /// </summary>
    /// <param name="userId">user who sent message id</param>
    /// <param name="username">user who sent message</param>
    /// <param name="message">message text</param>
    public async Task SendChatMessage(string username, string message)
    {
        await Clients.All.SendAsync("MessageReceived", username, message);
    }

    public void DoAction(string userId, string action) 
    {
        _gameRepository.PlayerAction(userId, action);
    }
    
}