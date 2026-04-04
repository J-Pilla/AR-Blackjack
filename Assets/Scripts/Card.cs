/// <summary>
/// handle the data aspect of a card
/// </summary>
public class Card
{
    // fields
    public readonly int _id; // make these private later
    public readonly Rank _rank;
    public readonly Suit _suit;

    // properties
    public int Value
    {
        get
        {
            if (_rank == Rank.Ace) return 11; // Ace is worth 11

            if (_rank > Rank.Ten) return 10; // Jack, Queen, and King are worth 10

            return (int)_rank;
        }
    }

    /// <summary>
    /// Generates the resource path used by the CardVisual or FaceChanger to load the card-face texture.<br/>
    /// This matches the naming convention for the card-face textures in the Resources folder "card-suit-rank".
    /// </summary>
    /// <returns>
    /// String in the format "card-suit-rank".
    /// </returns>
    public string ResourcePath { get => $"card-{_suit}-{(_rank < Rank.Ace ? (int)_rank : 1)}".ToLower(); } // e.g., "card-hearts-1" || "card-spades-13" 

    // constructors
    public Card()
    {
        _id = Deck.DistributeCardId();
        _rank = (Rank)(_id % 13 + 2);
        _suit = (Suit)(_id % 4);
    }

    public override string ToString() => $"Card; _id: {_id}, _rank: {_rank}, _suit: {_suit}";
}

public enum Suit
{
    Clubs,
    Diamonds,
    Hearts,
    Spades
}

public enum Rank
{
    Two = 2,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Ace
}
