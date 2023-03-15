using BlackJack.Common;

namespace BlackJack.Server;

public class Dealer
{
    public List<Card> Deck { get; set; } = new();
    private readonly Suit[] _suits = new[] {Suit.Hearts, Suit.Diamonds, Suit.Clubs, Suit.Spades};

    public List<Player> Players = new();

    public List<Card> DealerCards = new();

    public void NewDeck()
    {
        foreach (var s in _suits)
        {
            // init deck
            for (int i = 1; i <= 13; i++)
            {
                Deck.Add(new Card
                {
                    Value = i >= 10 ? 10 : i,
                    Suit = s
                });
            }
        }
    }

    public void AddCard(Card c)
    {
        DealerCards.Add(c);
    }
    public void Shuffle()
    {
        Random rnd = new Random();
        this.Deck = Deck.OrderBy(a => rnd.Next()).ToList();
    }

    /// <summary>
    /// Takes the first card from the deck
    /// </summary>
    /// <returns></returns>
    public Card TakeCard()
    {
        var card = Deck[0];
        Deck.RemoveAt(0);
        return card;
    }
    
}