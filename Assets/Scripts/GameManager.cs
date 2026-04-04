using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the full Blackjack game loop.
///
/// Flow:
///   WaitingForPlacement -> Dealing -> PlayerTurn -> DealerTurn -> Evaluation -> NewRound
/// </summary>
public class GameManager : MonoBehaviour
{
    /* inspector fields */
    [Header("References")]
    [SerializeField] private AutoPlaceBoard _arService;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private GameObject _cardPrefab;

    [Header("Table Layout")]
    [Tooltip("How far apart cards are spaced on the X-axis.")]
    [SerializeField] private float _cardSpacing = 0.5f;
    [Tooltip("Small Z and Y step so cards fan slightly rather than stacking exactly.")]
    [SerializeField] private float _cardStackOffset = 0.1f;

    [Header("Timing")]
    [SerializeField] private float _dealDelay = 0.4f;      // pause between each dealt card
    [SerializeField] private float _dealerDelay = 0.8f;    // 
    [SerializeField] private float _dealMoveDuration = 0.35f;
    [SerializeField] private float _dealArcHeight = 0.15f;

    /* private fields */

    // The two logical participants
    private Player _humanPlayer;
    private Player _dealer;

    // The spawned board and the transforms that act as card anchors
    private GameObject _board;
    private GameObject _playingBoard;
    private GameObject _bettingBoard;
    private Transform _playerCardRoot; // cards are spawned as children of these roots, which are positioned in the table prefab
    private Transform _dealerCardRoot;

    // deck references
    private Transform _deckRoot;
    private DeckVisualController _deckVisualController;

    // To track all spawned CardVisual objects so we can destroy them on reset
    private readonly List<CardVisual> _playerCardVisuals = new();
    private readonly List<CardVisual> _dealerCardVisuals = new();

    // Total player chips
    private int _totalChips = 1000;


    /* Game state management */
    private enum GamePhase
    {
        WaitingForPlacement,
        Betting,
        Dealing,
        PlayerTurn,
        DealerTurn,
        Evaluation
    }

    private GamePhase _phase = GamePhase.WaitingForPlacement;


    /* unity lifecycle */

    private void Awake()
    {
        // Validate inspector references
        if (_arService == null) Debug.LogError("GameManager: ARService is not assigned.", this);
        if (_uiManager == null) Debug.LogError("GameManager: UIManager is not assigned.", this);
        if (_cardPrefab == null) Debug.LogError("GameManager: CardPrefab is not assigned.", this);
    }

    private void Start()
    {
        // Initialise logical players
        _humanPlayer = new Player();
        _dealer = new Player();

        // Listen for the AR board being placed
        _arService.OnBoardPlaced += OnBoardPlaced;
        
        // Show the "aim at a surface" instruction
        _uiManager.ShowPlacementHint();
    }

    private void OnDestroy()
    {
        if (_arService != null)
            _arService.OnBoardPlaced -= OnBoardPlaced;

        if (_bettingBoard != null)
            _bettingBoard.GetComponent<BettingManager>().OnBetPlaced -= HandleBetPlaced;
    }


    /* AR callbacks */

    /// <summary>
    /// Called once by AutoPlaceBoard after the user taps to confirm placement.
    /// Finds the card root transforms inside the board prefab and starts dealing.
    /// </summary>
    private void OnBoardPlaced(GameObject board) // 
    {
        _board = board;

        // The BlackjackTable prefab originaly has two children named "PlayerCards"
        // one at z = -0.5 (dealer) and one at z = +0.5 (player).
        // I renamed them to "PlayerCardSlots" and "DealerCardSlots" for clarity
        // and we find them here by name.
        // Also finding UI elements and boards for betting and playing 
        Transform[] roots = board.GetComponentsInChildren<Transform>();
        foreach (Transform t in roots)
        {
            if(t.gameObject.name == "BoardRoot")
            {
                _playingBoard = t.gameObject;
            }
            else if (t.gameObject.name == "BettingRoot")
            {
                _bettingBoard = t.gameObject;
                _bettingBoard.GetComponent<BettingManager>().SetUpBet(_totalChips);
                _bettingBoard.GetComponent<BettingManager>().OnBetPlaced += HandleBetPlaced;
            }
            else if (t.gameObject.name == "PlayerCardSlots")
            {
                _playerCardRoot = t;
            }
            else if (t.gameObject.name == "DealerCardSlots")
            {
                _dealerCardRoot = t;
            }
            else if (t.gameObject.name == "DeckRoot")
            {
                _deckRoot = t;
            }
            else if (t.gameObject.name == "DeckVisual")
            {
                _deckVisualController = t.GetComponent<DeckVisualController>();
            }
            else if (t.gameObject.name == "HitButton")
            {
                _uiManager.setHitButton(t.gameObject);
            }
            else if (t.gameObject.name == "StandButton")
            {
                _uiManager.setStandButton(t.gameObject);
            }
            else if (t.gameObject.name == "DealerScoreTile")
            {
                _uiManager.setDealerText(t.gameObject.GetComponentInChildren<TextMeshPro>());
            }
            else if (t.gameObject.name == "PlayerScoreTile")
            {
                _uiManager.setPlayerText(t.gameObject.GetComponentInChildren<TextMeshPro>());
            }
        }

        if (_playerCardRoot == null || _dealerCardRoot == null)
        {
            Debug.LogError("GameManager: Could not find PlayerCards roots in the board prefab.");
            return;
        }

        if (_deckRoot == null)
        {
            Debug.LogError("GameManager: Could not find DeckRoot in board prefab");
            return;
        }

        if (_deckVisualController != null)
            _deckVisualController.Refresh();

        ClearStaticCards(_playerCardRoot);
        ClearStaticCards(_dealerCardRoot);

        _playingBoard.SetActive(false);
        _uiManager.ShowBettingState();

        _phase = GamePhase.Betting;
    }

    private void HandleBetPlaced(int totalBet)
    {
        // Now you can unlock dealing, update UI, etc.
        _bettingBoard.SetActive(false);
        _playingBoard.SetActive(true);

        StartCoroutine(DealInitialCards());
    }



    /* Dealing and player actions */

    /// <summary>
    /// Deals the standard 2-card opening: player, dealer, player, dealer.
    /// The dealer's first card is dealt face-down.
    /// </summary>
    private IEnumerator DealInitialCards()
    {
        _phase = GamePhase.Dealing;
        _uiManager.ShowDealingState();

        // Ensure the deck is shuffled before the first round
        if (!Deck.IsReady)
        {
            Deck.Shuffle();
            
            if (_deckVisualController != null)
                _deckVisualController.OnDeckReset();
        }

        // Deal order: player, dealer (face down), player, dealer (face up)
        yield return StartCoroutine(DealCardToPlayer( true));
        yield return new WaitForSeconds(_dealDelay);
        yield return StartCoroutine(DealCardToDealer(false));  // hole card
        yield return new WaitForSeconds(_dealDelay);
        yield return StartCoroutine(DealCardToPlayer(true));
        yield return new WaitForSeconds(_dealDelay);
        yield return StartCoroutine(DealCardToDealer(true));
        yield return new WaitForSeconds(_dealDelay);

        // Check for immediate blackjack
        if (_humanPlayer.IsBlackjack())
        {
            yield return StartCoroutine(RevealDealerHoleCard());
            EvaluateResult();
            yield break;
        }

        _phase = GamePhase.PlayerTurn;
        _uiManager.ShowPlayerTurn(_humanPlayer.GetValue(), _dealer.GetCardValue(1));
    }

    private IEnumerator DealCardToPlayer(bool faceUp)
    {
        Card card = new();
        _humanPlayer.AddCard(card);
        // SpawnCardVisual(card, faceUp, _playerCardRoot, _playerCardVisuals);
        // yield return new WaitForSeconds(_dealDelay);

        yield return StartCoroutine(SpawnAnimateCard(
            card,
            faceUp,
            _playerCardRoot,
            _playerCardVisuals
        ));
    }

    private IEnumerator DealCardToDealer(bool faceUp)
    {
        Card card = new();
        _dealer.AddCard(card);
        //SpawnCardVisual(card, faceUp, _dealerCardRoot, _dealerCardVisuals);
        //yield return new WaitForSeconds(_dealDelay);

        yield return StartCoroutine(SpawnAnimateCard(
            card,
            faceUp,
            _dealerCardRoot,
            _dealerCardVisuals
        ));
    }


    /* Player actions — called from UIManager button callbacks */

    /// <summary>
    /// Called when the player taps the Hit button.
    /// </summary>
    public void OnPlayerHit()
    {
        if (_phase != GamePhase.PlayerTurn) return;
        StartCoroutine(PlayerHitCoroutine());
    }

    private IEnumerator PlayerHitCoroutine()
    {
        _phase = GamePhase.Dealing;             // lock buttons during animation
        _uiManager.SetActionsInteractable(false);

        yield return StartCoroutine(DealCardToPlayer(faceUp: true));

        if (_humanPlayer.IsBust())
        {
            yield return StartCoroutine(RevealDealerHoleCard());
            EvaluateResult();
            yield break;
        }

        _phase = GamePhase.PlayerTurn;
        _uiManager.UpdatePlayerScore(_humanPlayer.GetValue());
        _uiManager.SetActionsInteractable(true);
    }

    /// <summary>
    /// Called when the player taps the Stand button.
    /// </summary>
    public void OnPlayerStand()
    {
        if (_phase != GamePhase.PlayerTurn) return;
        StartCoroutine(DealerTurnCoroutine());
    }


    /* Dealer's automated turn */

    /// <summary>
    /// Runs the dealer's automated turn:
    /// - Reveal the hole card.
    /// - Hit until the dealer's hand value is 17 or more.
    /// - Evaluate.
    /// </summary>
    private IEnumerator DealerTurnCoroutine()
    {
        _phase = GamePhase.DealerTurn;
        _uiManager.SetActionsInteractable(false);
        _uiManager.ShowDealerTurn();

        yield return StartCoroutine(RevealDealerHoleCard());

        while (_dealer.GetValue() < 17)
        {
            yield return new WaitForSeconds(_dealerDelay);
            yield return StartCoroutine(DealCardToDealer(true));
            _uiManager.UpdateDealerScore(_dealer.GetValue());
        }

        EvaluateResult();
    }

    /// <summary>
    /// Flips the dealer's first card (index 0) face-up.
    /// </summary>
    private IEnumerator RevealDealerHoleCard()
    {
        if (_dealerCardVisuals.Count > 0)
        {
            _dealerCardVisuals[0].FlipFaceUp();
            yield return new WaitForSeconds(FlipWaitTime());
            _uiManager.UpdateDealerScore(_dealer.GetValue());
        }
    }


    /* Evaluation */

    private void EvaluateResult()
    {
        _phase = GamePhase.Evaluation;

        int playerValue = _humanPlayer.GetValue();
        int dealerValue = _dealer.GetValue();
        bool playerBust = _humanPlayer.IsBust();
        bool dealerBust = _dealer.IsBust();
        bool playerBJ = _humanPlayer.IsBlackjack();
        bool dealerBJ = _dealer.IsBlackjack();

        GameResult result;

        if (playerBust)
            result = GameResult.DealerWins;
        else if (dealerBust)
            result = GameResult.PlayerWins;
        else if (playerBJ && !dealerBJ)
            result = GameResult.Blackjack;
        else if (dealerBJ && !playerBJ)
            result = GameResult.DealerWins;
        else if (playerValue > dealerValue)
            result = GameResult.PlayerWins;
        else if (dealerValue > playerValue)
            result = GameResult.DealerWins;
        else
            result = GameResult.Push;

        if(result == GameResult.PlayerWins || result == GameResult.Blackjack)
            _totalChips += _bettingBoard.GetComponent<BettingManager>().GetBet();
        else if (result == GameResult.DealerWins)
            _totalChips -= _bettingBoard.GetComponent<BettingManager>().GetBet();

        _uiManager.ShowResult(result, playerValue, dealerValue);
    }


    /* New round / Reset */

    /// <summary>
    /// Called from UIManager's "New Round" button.
    /// Destroys all card visuals, resets player data, and returns to betting state.
    /// The AR table stays in place.
    /// </summary>
    public void StartNewRound()
    {
        // Check whether the deck needs to be reshuffled (past the shuffle point)
        if (Deck.NeedsReshuffle)
        {
            Deck.Shuffle();
            
            if (_deckVisualController != null)
                _deckVisualController.OnDeckReset();
        }

        // Destroy all spawned card GameObjects
        DestroyCardVisuals(_playerCardVisuals);
        DestroyCardVisuals(_dealerCardVisuals);

        // Reset logical state
        _humanPlayer.Reset();
        _dealer.Reset();

        _bettingBoard.SetActive(true);
        _playingBoard.SetActive(false);

        _bettingBoard.GetComponent<BettingManager>().SetUpBet(_totalChips);
        _uiManager.ShowBettingState();

        _phase = GamePhase.Betting;
    }


     /* Visual helpers */

    /// <summary>
    /// Instantiates a Card prefab, initialises its CardVisual component,
    /// and positions it in a row under the given root transform.
    /// Each new card is offset along the local X-axis so they fan out.
    /// A tiny Z and Y offset prevents Z-fighting when cards overlap.
    /// </summary>
    
    private IEnumerator SpawnAnimateCard(Card card, bool faceUp, Transform targetRoot, List<CardVisual> visualList)
    {
        if (_deckRoot == null)
        {
            Debug.LogError("GameManager: DeckRoot is missing");
            yield break;
        }

        int index = visualList.Count;

        // spawned as child of the root so they move with the table, and position them in a row with some spacing
        Vector3 targetLocalPos = new(
            index * _cardSpacing,
            index * _cardStackOffset,
            index * _cardStackOffset);

        // Cards lie flat on the table (rotated 90 on X so the face points up)
        Quaternion targetLocalRot = Quaternion.Euler(90f, 0f, 0f);

        Vector3 targetWorldPos = targetRoot.TransformPoint(targetLocalPos);
        Quaternion targetWorldRot = targetRoot.rotation * targetLocalRot;

        GameObject cardGO = Instantiate(_cardPrefab, _deckRoot.position, _deckRoot.rotation);
        cardGO.name = card.Name;

        CardVisual visual = cardGO.GetComponent<CardVisual>();
        
        if (visual == null)
        {
            Debug.LogError("GameManager: Card prefab does not have a CardVisual component");
            Destroy(cardGO);
            yield break;
        }
        
        visual.Initialize(card, false);

        if (_deckVisualController != null) // shrink deck after drawn
            _deckVisualController.OnCardDrawn();
        
        yield return StartCoroutine(AnimateCardToTarget(
            cardGO.transform,
            targetWorldPos,
            targetWorldRot
        ));

        cardGO.transform.SetParent(targetRoot, true);
        cardGO.transform.SetLocalPositionAndRotation(targetLocalPos, targetLocalRot);

        if (faceUp)
            visual.FlipFaceUp();
        
        visualList.Add(visual);

        // GameObject cardGO = Instantiate(_cardPrefab, root);
        // cardGO.transform.SetLocalPositionAndRotation(localPos, localRot);
        // cardGO.name = $"Card_{card._suit}_{card._rank}";

        // CardVisual visual = cardGO.GetComponent<CardVisual>();
        // if (visual == null)
        // {
        //     Debug.LogError("GameManager: Card prefab does not have a CardVisual component.");
        //     return;
        // }

        // visual.Initialize(card, false);

        // if (faceUp)
        // {
        //     visual.FlipFaceUp(); // Only face-up if specified, so we can deal the dealer's hole card face-down
        // }

        // visualList.Add(visual);
    }

    private IEnumerator AnimateCardToTarget(Transform cardTransform, Vector3 targetPos, Quaternion targetRot)
    {
        Vector3 startPos = cardTransform.position;
        Quaternion startRot = cardTransform.rotation;

        float elapsed = 0f;

        while (elapsed < _dealMoveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _dealMoveDuration);

            float eased = Mathf.SmoothStep(0f, 1f, t);

            Vector3 pos = Vector3.Lerp(startPos, targetPos, eased);

            pos.y += Mathf.Sin(eased * Mathf.PI) * _dealArcHeight;

            cardTransform.position = pos;
            cardTransform.rotation = Quaternion.Slerp(startRot, targetRot, eased);

            yield return null;
        }

        cardTransform.position = targetPos;
        cardTransform.rotation = targetRot;
    }

    /// <summary>
    /// Destroys all GameObjects in a list of CardVisuals and clears the list.
    /// </summary>
    private void DestroyCardVisuals(List<CardVisual> list)
    {
        foreach (CardVisual cv in list)
        {
            if (cv != null)
                Destroy(cv.gameObject);
        }
        list.Clear();
    }

    /// <summary>
    /// Removes pre-placed static card children from a root transform.
    /// These were baked into the BlackjackTable prefab for test purposes.
    /// </summary>
    private void ClearStaticCards(Transform root)
    {
        for (int i = root.childCount - 1; i >= 0; i--)
            Destroy(root.GetChild(i).gameObject);
    }

    /// <summary>
    /// Returns the time to wait after triggering a flip so the UI update
    /// happens after the animation finishes.
    /// Matches the FlipDuration constant in CardVisual.
    /// </summary>
    private float FlipWaitTime() => 0.5f; // slightly longer than CardVisual.FlipDuration
}

/// <summary>
/// Possible outcomes of a round. Used by UIManager to choose the display text.
/// </summary>
public enum GameResult
{
    PlayerWins,
    DealerWins,
    Push,
    Blackjack
}

