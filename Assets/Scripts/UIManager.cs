using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls all Canvas UI elements.
/// </summary>
public class UIManager : MonoBehaviour
{
    /* inspector fields */
    [Header("Placement")]
    [SerializeField] private GameObject _placementHintPanel;
    [SerializeField] private TextMeshProUGUI _placementHintText;

    [Header("Game Panel")]
    [SerializeField] private GameObject _gamePanel;
    [SerializeField] private TextMeshProUGUI _dealerScoreText;
    [SerializeField] private TextMeshProUGUI _playerScoreText;
    [SerializeField] private Button _hitButton;
    [SerializeField] private Button _standButton;

    [Header("Result Panel")]
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private Button _newRoundButton;

    [Header("GameManager Reference")]
    [SerializeField] private GameManager _gameManager;



    /* unity lifecycle */
    private void Start()
    {
        // button callbacks
        _hitButton.onClick.AddListener(_gameManager.OnPlayerHit);
        _standButton.onClick.AddListener(_gameManager.OnPlayerStand);
        _newRoundButton.onClick.AddListener(_gameManager.StartNewRound);

        // at initial state, only the placement hint is visible
        SetPanelActive(_placementHintPanel, true);
        SetPanelActive(_gamePanel, false);
        SetPanelActive(_resultPanel, false);
    }

    /* Public API — called by GameManager */

    /// <summary>
    /// Shows the AR placement instruction screen.
    /// </summary>
    public void ShowPlacementHint()
    {
        if (_placementHintText != null)
            _placementHintText.text = "Point your camera at a flat surface\nand tap to place the table";

        SetPanelActive(_placementHintPanel, true);
        SetPanelActive(_gamePanel, false);
        SetPanelActive(_resultPanel, false);
    }

    /// <summary>
    /// Called while initial cards are being dealt. Hides action buttons.
    /// </summary>
    public void ShowDealingState()
    {
        SetPanelActive(_placementHintPanel, false);
        SetPanelActive(_gamePanel, true);
        SetPanelActive(_resultPanel, false);

        _dealerScoreText.text = "Dealer: [?]";
        _playerScoreText.text = "You: [?]";
        SetActionsInteractable(false);
    }

    /// <summary>
    /// Called when it's the player's turn. Shows scores and enables buttons.
    /// </summary>
    public void ShowPlayerTurn(int playerScore, int dealerVisibleCard)
    {
        _playerScoreText.text = $"You: {playerScore}";
        _dealerScoreText.text = $"Dealer: {dealerVisibleCard} + [?]";
        SetActionsInteractable(true);
    }

    /// <summary>
    /// Called when the dealer's turn begins.
    /// </summary>
    public void ShowDealerTurn()
    {
        SetActionsInteractable(false);
        _dealerScoreText.text = "Dealer is playing...";
    }

    /// <summary>
    /// Updates the dealer score label during the dealer's turn.
    /// </summary>
    public void UpdateDealerScore(int score)
    {
        _dealerScoreText.text = $"Dealer: {score}";
    }

    /// <summary>
    /// Updates the player score label after a hit.
    /// </summary>
    public void UpdatePlayerScore(int score)
    {
        _playerScoreText.text = $"You: {score}";
    }

    /// <summary>
    /// Displays the round result overlay and the "New Round" button.
    /// </summary>
    public void ShowResult(GameResult result, int playerScore, int dealerScore)
    {
        SetPanelActive(_resultPanel, true);
        SetActionsInteractable(false);
        _playerScoreText.color = result == GameResult.PlayerWins || result == GameResult.Blackjack ? Color.green : Color.red;
        _playerScoreText.text = $"You: {playerScore}";
        _dealerScoreText.text = $"Dealer: {dealerScore}";

        _resultText.text = result switch
        {
            GameResult.Blackjack => "Blackjack! You Win!",
            GameResult.PlayerWins => "You Win!",
            GameResult.DealerWins => "Dealer Wins",
            GameResult.Push => "It's a Tie",
            _ => string.Empty
        };

        _resultText.color = result switch
        {
            GameResult.PlayerWins => Color.green,
            GameResult.Blackjack => Color.green,
            GameResult.DealerWins => Color.red,
            GameResult.Push => Color.yellow,
            _ => Color.white
        };
    }

    /// <summary>
    /// Enables or disables the Hit and Stand buttons.
    /// Called by GameManager to prevent input during animations.
    /// </summary>
    public void SetActionsInteractable(bool interactable)
    {
        _hitButton.interactable = interactable;
        _standButton.interactable = interactable;
    }

    /* private helper methods */

    private void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null)
            panel.SetActive(active);
    }
}
