/// <summary>
/// handle the data aspect of a card
/// </summary>
//[System.Serializable]
public class Card
{
    // fields
    public int _id;
    public Rank _rank;
    public Suit _suit;

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
    public string ResourcePath { get => $"card-{_suit}-{(int)(_rank < Rank.Ace ? _rank : Rank.LowAce)}".ToLower(); } // e.g., "card-hearts-1" || "card-spades-13" 

    // constructors
    public Card()
    {
        _id = Deck.DistributeCardId();
        _rank = (Rank)(_id % 13 + 2);
        _suit = (Suit)(_id % 4);
    }

    [System.Obsolete("Use the default constructor outside of forcing cards for testing")]
    public Card(Suit suit, Rank rank)
    {
        _suit = suit;
        _rank = rank;
    }

    // methods
    /// <summary>
    /// changes Ace to LowAce, reducing the card's value to 1
    /// </summary>
    public void LowerAce()
    {
        if (_rank == Rank.Ace)
            _rank = Rank.LowAce;
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
    LowAce = 1,
    Two,
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
