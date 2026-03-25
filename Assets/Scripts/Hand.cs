using System.Collections.Generic;


/// <summary>
/// Represents a single hand of cards held by a player or the dealer
/// </summary>
[System.Serializable]
public class Hand
{
    // fields
    private readonly List<Card> _cards = new();

    // properties
    public int Value
    {
        get
        {
            if (_cards.Count == 0) return 0;

            int value = 0;
            int aceCount = 0;

            foreach (Card card in _cards)
            {
                value += card.Value;
                if (card._rank == Rank.Ace) aceCount++;
            }
            /* 
             * The original script never reduced Aces automatically.
             * This version counts the aces and reduces 10 per ace as needed without mutating the card data.
             */
            while (value > 21 && aceCount > 0)
            {
                value -= 10;
                aceCount--;
            }

            return value;
        }
    }


    /// <summary>
    /// IsBust
    /// </summary>
    public bool IsBust => Value > 21;

    /// <summary>
    /// True if the hand is a natural blackjack (Ace + 10-value card on deal).
    /// </summary>
    public bool IsBlackjack => Value == 21 && _cards.Count == 2;

    /// <summary>
    /// Readonly list of cards in this hand.
    /// </summary>
    public IReadOnlyList<Card> Cards => _cards;

    // methods
    public void AddCard(Card card) => _cards.Add(card);
    public void Clear() => _cards.Clear();


    [System.Obsolete("Ace reduction is now handled automatically in the Value getter.")]
    public void LowerAce()
    {
        foreach (Card card in _cards)
        {
            if (card.Value == 11)
            {
                card.LowerAce();

                if (Value > 21)
                    LowerAce();
            }
        }
    }
}
