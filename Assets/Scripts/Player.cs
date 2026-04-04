using System.Collections.Generic;

/// <summary>
/// Represents either the human player or the dealer
/// </summary>
[System.Serializable]
public class Player
{
    // fields
    private readonly List<Hand> _hands = new();


    // constructor
    public Player() => _hands.Add(new());

    // methods
    public int GetValue(int handIndex = 0) => _hands[handIndex].Value;

    public void AddCard(Card card, int handIndex = 0) => _hands[handIndex].AddCard(card);

    public int GetCardValue(int cardIndex, int handIndex = 0) => _hands[handIndex].GetCardValue(cardIndex);

    public bool IsBust(int handIndex = 0) => _hands[handIndex].IsBust;

    public bool IsBlackjack(int handIndex = 0) => _hands[handIndex].IsBlackjack;

    /// <summary>
    /// Clears all hands and resets back to one empty hand, ready for a new round.
    /// </summary>
    public void Reset()
    {
        _hands.Clear();
        _hands.Add(new());
    }
}
