namespace BlackJack.Common;

public class Player
{
    public string UserName { get; set; }
    public string UserId { get; set; }
    public decimal Money { get; set; } = 100;

    public List<Card> Cards { get; set; } = new();

    public GameStateAction Action = GameStateAction.None;
    
    public Player(string name, string userId)
    {
        UserName = name;
        UserId = userId;
    }

    public void AddCard(Card c)
    {
        Cards.Add(c);
    }

    /// <summary>
    /// check if the player is bust
    /// </summary>
    /// <returns>returns true when player is bust, false otherwise</returns>
    public bool CheckPlayerBust()
    {
        if (Cards.Exists(x => x.Value == 1))
        {
            // player has ace so check for 11
            int sum = 0;
            foreach (var card in Cards)
            {
                if (card.Value == 1)
                {
                    sum += 11;
                }
                else
                {
                    sum += card.Value;
                }
            }

            if (sum <= 21)
            {
                return false;
            }
        }

        return Cards.Sum(x => x.Value) > 21;
    }
    
    
    
}