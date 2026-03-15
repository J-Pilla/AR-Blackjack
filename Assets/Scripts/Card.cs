using UnityEngine;

// Defines the suit and rank enums, and the Card class
// it does not have to be a MonoBehaviour,
public enum Suit
{
    Hearts,
    Diamonds,
    Clubs,
    Spades
}

public enum Rank
{
    Ace = 1, // value of Ace is set to 1 here, the GameManager or a Hand script will handle the dual value of Ace (1 or 11) during gameplay.
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13
}
public class Card
{
    public Suit Suit;
    public Rank Rank;

    public Card(Suit suit, Rank rank)
    {
        Suit = suit;
        Rank = rank;
    }

    /// <summary>
    /// does not require summary.
    /// </summary>
    /// <returns></returns>
    public int GetValue()
    {
        if (Rank == Rank.Ace)
        {
            return 11;
        }
        if ((int)Rank >= 10)
        {
            return 10; // 10, J, Q, K
        }
        return (int)Rank; // 2-9 are worth their face value
    }

    /// <summary>
    /// Generates the resource path used by the CardVisual or FaceChanger to load the card-face texture.
    /// This matches the naming convention for the card-face textures in the Resources folder "card-suit-rank".
    /// </summary>
    /// <returns>
    ///     String in the format "card-suit-rank".
    /// </returns>
    public string ResourcePath()
    {
        string suitName = Suit.ToString().ToLower();
        int rankValue = (int)Rank;
        return $"card-{suitName}-{rankValue}"; // e.g., "card-hearts-1" || "card-spades-13"
    }
}
