/// <summary>
/// handle the data aspect of a card
/// </summary>
public class Card
{
    // fields
    public readonly int Id;
    public readonly Rank Rank;
    public readonly Suit Suit;

    // properties
    public int Value
    {
        get
        {
            if (Rank == Rank.Ace) return 11; // Ace is worth 11

            if (Rank > Rank.Ten) return 10; // Jack, Queen, and King are worth 10

            return (int)Rank;
        }
    }

    /// <summary>
    /// Generates the resource path used by the CardVisual or FaceChanger to load the card-face texture.<br/>
    /// This matches the naming convention for the card-face textures in the Resources folder "card-suit-rank".
    /// </summary>
    /// <returns>
    /// String in the format "card-suit-rank".
    /// </returns>
    public string ResourcePath => $"card-{Suit}-{(int)Rank}".ToLower(); // e.g., "card-hearts-1" || "card-spades-13"

    /// <summary>
    /// name used for game objects spawned with the card
    /// </summary>
    public string Name => $"Card_{Suit}_{Rank}";

    // constructors
    public Card()
    {
        Id = Deck.DistributeCardId();
        Rank = (Rank)(Id % 13 + 1);
        Suit = (Suit)(Id % 4);
    }

    public override string ToString() => $"Card; _id: {Id}, _rank: {Rank}, _suit: {Suit}";
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
    Ace = 1,
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
    King
}
