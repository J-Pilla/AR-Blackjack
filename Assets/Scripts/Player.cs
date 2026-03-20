using System.Collections.Generic;

[System.Serializable]
public class Player
{
    // fields
    private readonly List<Hand> _hands = new();

    // methods
    public int GetValue(int handIndex) => _hands[handIndex].Value;
    public void AddCard(Card card, int handIndex) => _hands[handIndex].AddCard(card);
    public void LowerAce(int handIndex) => _hands[handIndex].LowerAce();
}
