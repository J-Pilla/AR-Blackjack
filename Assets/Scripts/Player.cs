using System.Collections.Generic;

/// <summary>
/// Represents either the human player or the dealer
/// </summary>
[System.Serializable]
public class Player
{
    // fields
    private readonly List<Hand> _hands = new();

    // Constructor
    // The original script never added a hand to the list
    public Player()
    {
        _hands.Add(new Hand());
    }

    /// <summary>
    /// convenience accessor.
    /// </summary>
    public Hand PrimaryHand => _hands[0];

    // methods
    public int GetValue(int handIndex = 0) => _hands[handIndex].Value;
    public void AddCard(Card card, int handIndex = 0) => _hands[handIndex].AddCard(card);

    public bool IsBust(int handIndex = 0) => _hands[handIndex].IsBust;

    public bool IsBlackjack(int handIndex = 0) => _hands[handIndex].IsBlackjack;

    /// <summary>
    /// Clears all hands and resets back to one empty hand, ready for a new round.
    /// </summary>
    public void Reset()
    {
        _hands.Clear();
        _hands.Add(new Hand());
    }
}
