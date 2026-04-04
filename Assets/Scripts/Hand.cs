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

                if (card.Rank == Rank.Ace)
                    aceCount++;
            }

            while (value > 21 && aceCount > 0)
            {
                value -= 10;
                aceCount--;
            }

            return value;
        }
    }

    public bool IsBust => Value > 21;

    /// <summary>
    /// True if the hand is a natural blackjack (Ace + 10-value card on deal).
    /// </summary>
    public bool IsBlackjack => Value == 21 && _cards.Count == 2;

    // methods
    public void AddCard(Card card) => _cards.Add(card);

    public int GetCardValue(int cardIndex) => _cards[cardIndex].Value;
}
