namespace BlackJack.Common;


public enum Suit
{
    Hearts, Spades, Diamonds, Clubs
    
}


public record Card
{
    public int Value { get; set; }
    public Suit Suit { get; set; }
}