using BlackJack.Common;

namespace BlackJack.Server;

public class GameRepository
{
    public List<Player> Players { get; private set; } = new();
    public bool GameInProgress { get; private set; } = false;

    public void AddPlayer(Player p)
    {
        Players.Add(p);
    }

    public void ToggleGameInProgress()
    {
        GameInProgress = !GameInProgress;
    }

    public void PlayerAction(string userId, string action)
    {
        var playerIndex = Players.FindIndex(x => x.UserId == userId);
        switch (action.ToLower())
        {
            case "hit":
                Players[playerIndex].Action = GameStateAction.Hit;
                break;
            
            case "stay":
                Players[playerIndex].Action = GameStateAction.Stay;
                break;
            case "bust":
                Players[playerIndex].Action = GameStateAction.Bust;
                break;
        }
    }
}