using System.Collections.Generic;

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

            foreach (Card card in _cards)
                value += card.Value;

            return value;
        }
    }

    // methods
    public void AddCard(Card card) => _cards.Add(card);
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
