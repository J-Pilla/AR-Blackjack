using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls all Canvas UI elements.
/// </summary>
public class UIManager : MonoBehaviour
{
    private GameManager _gameManager;

    [Header("Hit And Stand Buttons")]
    [SerializeField] private GameObject _hitButton;
    [SerializeField] private GameObject _standButton;

    [Header("Score Texts")]
    [SerializeField] private TextMeshPro _playerScoreText;
    [SerializeField] private TextMeshPro _dealerScoreText;

    [Header("Results")]
    [SerializeField] private TextMeshPro _newResultText;
    [SerializeField] private GameObject _newNewRoundButton;
    [SerializeField] private GameObject _newResultDisplay;

    [Header("Game Over")]
    [SerializeField] private GameObject _newGameButton;
    [SerializeField] private GameObject _gameOverMainMenuButton;

    private bool _hitStandButtonsActive;

    /* unity lifecycle */
    private void Start()
    {
        _gameManager = FindAnyObjectByType<GameManager>();

    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleTapInput();
    }

    private void HandleTapInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if(_gameManager._phase != GameManager.GamePhase.GameOver)
            {
                if(_hitStandButtonsActive)
                {
                    if (hit.transform.gameObject == _hitButton)
                        _gameManager.OnPlayerHit();
                    else if (hit.transform.gameObject == _standButton)
                        _gameManager.OnPlayerStand();
                }
                if (hit.transform.gameObject == _newNewRoundButton)
                    _gameManager.StartNewRound();
            }
            else
            {
                if (hit.transform.gameObject == _newGameButton)
                    _gameManager.NewGame();
                else if (hit.transform.gameObject == _gameOverMainMenuButton)
                    SceneManager.LoadScene(0);
            }   
        }
    }

    /* Public API — called by GameManager */
    /// <summary>
    /// Called while initial cards are being dealt. Hides action buttons.
    /// </summary>
    public void ShowDealingState()
    {
        SetOptionsToPlay(true);

        _dealerScoreText.text = "?";
        _playerScoreText.text = "?";
        SetActionsInteractable(false);
    }

    /// <summary>
    /// Called when it's the player's turn. Shows scores and enables buttons.
    /// </summary>
    public void ShowPlayerTurn(int playerScore, int dealerVisibleCard)
    {
        _dealerScoreText.text = $"{dealerVisibleCard}";
        _playerScoreText.text = $"{playerScore}";
        SetActionsInteractable(true);
    }

    /// <summary>
    /// Called when the dealer's turn begins.
    /// </summary>
    public void ShowDealerTurn()
    {
        SetActionsInteractable(false);
    }

    /// <summary>
    /// Updates the dealer score label during the dealer's turn.
    /// </summary>
    public void UpdateDealerScore(int score)
    {
        _dealerScoreText.text = $"{score}";
        
    }

    /// <summary>
    /// Updates the player score label after a hit.
    /// </summary>
    public void UpdatePlayerScore(int score)
    {
        _playerScoreText.text = $"{score}";
    }

    /// <summary>
    /// Displays the round result overlay and the "New Round" button.
    /// </summary>
    public void ShowResult(GameResult result, int playerScore, int dealerScore)
    {
        SetActionsInteractable(false);

        UpdateDealerScore(dealerScore);
        UpdatePlayerScore(playerScore);

        _newResultText.text = result switch
        {
            GameResult.Blackjack => "Blackjack! You Win!",
            GameResult.PlayerWins => "You Win!",
            GameResult.DealerWins => "Dealer Wins",
            GameResult.Push => "It's a Tie",
            _ => string.Empty
        };

        _newResultText.color = result switch
        {
            GameResult.PlayerWins => Color.green,
            GameResult.Blackjack => Color.green,
            GameResult.DealerWins => Color.red,
            GameResult.Push => Color.yellow,
            _ => Color.white
        };

        SetOptionsToPlay(false);
    }

    /// <summary>
    /// Enables or disables the Hit and Stand buttons.
    /// Called by GameManager to prevent input during animations.
    /// </summary>
    public void SetActionsInteractable(bool interactable)
    {
        _hitStandButtonsActive = interactable;

        Color objectColor = interactable ? Color.white : Color.gray;

        _hitButton.GetComponent<Renderer>().material.SetColor("_BaseColor", objectColor);
        _standButton.GetComponent<Renderer>().material.SetColor("_BaseColor", objectColor);
    }

    /* private helper methods */
    private void SetOptionsToPlay(bool isGameOptions)
    {
        _hitButton.SetActive(isGameOptions);
        _standButton.SetActive(isGameOptions);

        _newResultDisplay.SetActive(!isGameOptions);
        _newNewRoundButton.SetActive(!isGameOptions);
    }
}
