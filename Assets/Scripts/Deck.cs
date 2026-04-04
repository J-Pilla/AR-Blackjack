using static UnityEngine.Random;

/// <summary>
/// static class representing a deck of cards,
/// uses an array of integers to initialize unique Card objects
/// </summary>
public static class Deck
{
    // fields
    private static readonly int[] _cardIds = new int[Size];
    private static int _cardIndex = Size;
    private static int _shufflePoint; // this signifies when the deck should be shuffled after a round is over
    private static bool _isInitialized;

    // constants
    private const int DeckSize = 52; // standard 52 card deck, no jokers
    private const int DeckCount = 6; // in a casino, 6 decks are used
    private const int ShuffleCount = 4; // arbitrary number to make it "more random"
    public const int Size = DeckSize * DeckCount;

    // properties
    public static bool IsShuffleTime => _cardIndex < _shufflePoint;

    // handling visual deck shrinking as cards are drawn
    public static int CardsDrawn => _cardIndex;
    public static int CardsRemaining => Size - _cardIndex;
    public static float RemainingRatio => Size == 0 ? 0f : (float)CardsRemaining / Size;


    /* public API */

    /// <summary>
    /// randomize the order of the card ids in the deck
    /// and sets deckIndex back down to 0
    /// </summary>
    public static void Shuffle()
    {
        _cardIndex = 0;

        if (!_isInitialized)
            InitializeDeck();

        for (int ctr = 0; ctr < ShuffleCount; ctr++)
        {
            for (int index = 0; index < Size; index++)
            {
                int randomIndex = Range(index, Size - 1);
                (_cardIds[index], _cardIds[randomIndex]) = (_cardIds[randomIndex], _cardIds[index]);
            }
        }

        Cut();
        SetShufflePoint();
    }


    /// <summary>
    /// Returns the next card id from the shoe. Forces a shuffle if exhausted.
    /// </summary>
    public static int DistributeCardId()
    {
        if (_cardIndex >= Size)
            Shuffle();

        return _cardIds[_cardIndex++];
    }

    /* private methods */

    /// <summary>
    /// initializes the deck so each element carries a card id equal to the index
    /// </summary>
    private static void InitializeDeck()
    {
        _isInitialized = true;

        for (int index = 0; index < Size; index++)
            _cardIds[index] = index;
    }

    /// <summary>
    /// finds a random point close to the centre of the CardId array,
    /// then moves the ids after the point to the start of the array,
    /// bumping the ids before the point to the end of the array
    /// </summary>
    private static void Cut()
    {
        const int MidPoint = Size / 2 - 1;
        const int Variance = 4 * DeckCount;

        int cutPoint = Range(MidPoint - Variance, MidPoint + Variance);
        int remainingCards = Size - cutPoint;
        int[] lowerIds = new int[cutPoint];

        for (int index = 0; index < cutPoint; index++)
            lowerIds[index] = _cardIds[index]; // copy the cards before the cut point into a local array

        for (int index = 0; index < Size; index++)
        {
            _cardIds[index] = index < remainingCards ?
                _cardIds[index + cutPoint] : // copy the cards after the cut point to replace the cards before the cut point
                lowerIds[index - remainingCards]; // copy the cards before the cut point to replace the cards after the cut point
        }
    }

    private static void SetShufflePoint()
    {
        const int min = (int)(Size - DeckCount * 12.5f); // truncate the min if DeckCount is odd
        const int max = Size - DeckCount * 10;
        // with 6 decks the range is 60 - 75
        _shufflePoint = Range(min, max);
    }
}
