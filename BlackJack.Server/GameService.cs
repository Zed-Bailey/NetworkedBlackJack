using BlackJack.Common;
using Microsoft.AspNetCore.SignalR;

namespace BlackJack.Server;

public class GameService : BackgroundService
{
    private IHubContext<BlackJackHub> _hubContext;
    private GameRepository _gameRepository;

    private Dealer dealer = new Dealer();
    
    public GameService(IHubContext<BlackJackHub> hubContext, GameRepository repository)
    {
        _hubContext = hubContext;
        _gameRepository = repository;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {

            if (_gameRepository.Players.Count == 0)
            {
                await Task.Delay(15_000, stoppingToken);
                continue;
            }

            // can start a game
            if (!_gameRepository.GameInProgress)
            {
                _gameRepository.ToggleGameInProgress();
                dealer.NewDeck();
                dealer.Shuffle();
                
                // deals 2 cards to all players
                for (var deal = 0; deal < 2; deal++)
                {
                    foreach (var player in _gameRepository.Players)
                    {
                        player.AddCard(dealer.TakeCard());
                    }
                    dealer.AddCard(dealer.TakeCard());
                }
                await NotifyPlayerCards(_gameRepository.Players, dealer.DealerCards);
            }

           
            // Run Game
            // loop over each player and wait until each player has done their action
            foreach (var player in _gameRepository.Players)
            {
                while (player.Action != GameStateAction.Stay || player.Action != GameStateAction.Bust)
                {
                    switch (player.Action)
                    {
                        case GameStateAction.Hit:
                            player.AddCard(dealer.TakeCard());
                            await PlayerHitCard(player);
                            var bust = player.CheckPlayerBust();
                            if (bust)
                            {
                                _gameRepository.PlayerAction(player.UserId, "bust");
                                await PlayerBust(player);
                            }
                            break;
                    }

                    await Task.Delay(1000, stoppingToken);
                }
            }
            
            // all players have had their turn, show dealer cards
            await ShowDealerCards();

            var dealerSum = dealer.DealerCards.Sum(x => x.Value);
            var dealerBust = dealerSum > 21;
            if (!dealerBust)
            {
                foreach (var player in _gameRepository.Players)
                {
                    if (player.Action == GameStateAction.Bust)
                        continue;
                    
                    if (player.Cards.Sum(x => x.Value) > dealerSum)
                    {
                        // player wins
                        await PlayerWin(player);
                    }
                    else
                    {
                        //player looses
                        await PlayerLoose(player);
                    }
                }
            }
            else
            {
                // dealer bust, all players win
                foreach (var player in _gameRepository.Players)
                {
                    if (player.Action != GameStateAction.Bust)
                    {
                        await PlayerWin(player);
                    }
                }
            }
            
            dealer.NewDeck();
            dealer.Shuffle();
            await ClearGame();
            await Task.Delay(30_000, stoppingToken);

        }
    }
    
        /**
         * Sends the players cards to each client along with the dealers cards
         */
        public async Task NotifyPlayerCards(List<Player> players, List<Card> dealerCards)
        {
            foreach (var player in players)
            {
                await _hubContext.Clients.Client(player.UserId).SendAsync("ReceiveCardDeal", player.Cards, dealerCards);
            }
        }
    
        public async Task PlayerBust(Player player)
        {
            await _hubContext.Clients.Client(player.UserId).SendAsync("PlayerBust");
        }
    
        public async Task PlayerHitCard(Player player)
        {
            await _hubContext.Clients.Client(player.UserId).SendAsync("HitCard", player.Cards.Last());
        }

        public async Task PlayerWin(Player player)
        {
            await _hubContext.Clients.Client(player.UserId).SendAsync("Win");
        }
        
        public async Task PlayerLoose(Player player)
        {
            await _hubContext.Clients.Client(player.UserId).SendAsync("Loose");
        }
        
        public async Task ShowDealerCards()
        {
            await _hubContext.Clients.All.SendAsync("ShowDealerCards");
        }

        public async Task ClearGame()
        {
            await _hubContext.Clients.All.SendAsync("ClearGame");
        }
}