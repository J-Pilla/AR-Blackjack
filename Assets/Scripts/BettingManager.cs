using System.Collections;
using TMPro;
using UnityEngine;

public class BettingManager : MonoBehaviour
{
    private int _betAmount = 0;
    private int _totalChips = 0;
    private float _chipHeight = .003125f;

    [Header("Betting Buttons")]
    [SerializeField] private GameObject _negativeOneHundred;
    [SerializeField] private GameObject _negativeTen;
    [SerializeField] private GameObject _negativeOne;

    [SerializeField] private GameObject _positiveOneHundred;
    [SerializeField] private GameObject _positiveTen;
    [SerializeField] private GameObject _positiveOne;

    [SerializeField] private GameObject _placeBetButton;

    [Header("Chip Spawners")]
    [SerializeField] private GameObject _oneHundredSpawner;
    [SerializeField] private GameObject _tenSpawner;
    [SerializeField] private GameObject _oneSpawner;

    [Header("Chip Prefabs")]
    [SerializeField] private GameObject _oneHundredPrefab;
    [SerializeField] private GameObject _tenPrefab;
    [SerializeField] private GameObject _onePrefab;

    [Header("Text Fields")]
    [SerializeField] private TextMeshPro _betText;
    [SerializeField] private TextMeshPro _chipsText;

    public int BetAmount => _betAmount;

    public event System.Action<int> OnBetPlaced; // int = bet amount

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleTapInput();
    }

    private void HandleTapInput()
    {
        // Control betting actions and ability to place the bet when finished
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject == _positiveOneHundred)
                SpawnChip(_oneHundredPrefab, _oneHundredSpawner, 100);
            else if (hit.transform.gameObject == _negativeOneHundred)
                DestroyChip(_oneHundredSpawner, 100);
            else if (hit.transform.gameObject == _positiveTen)
                SpawnChip(_tenPrefab, _tenSpawner, 10);
            else if (hit.transform.gameObject == _negativeTen)
                DestroyChip(_tenSpawner, 10);
            else if (hit.transform.gameObject == _positiveOne)
                SpawnChip(_onePrefab, _oneSpawner, 1);
            else if (hit.transform.gameObject == _negativeOne)
                DestroyChip(_oneSpawner, 1);
            else if (hit.transform.gameObject == _placeBetButton)
                PlaceBet(_betAmount);
        }
    }

    /* Betting Actions */
    /// <summary>
    /// Resets the betting board and betting details before initializing the betting phase
    /// </summary>
    public IEnumerator ResetBettingBoard()
    {
        // Clear out the chip spawners
        foreach (Transform child in _oneSpawner.transform)
            Destroy(child.gameObject);

        foreach (Transform child in _tenSpawner.transform)
            Destroy(child.gameObject);

        foreach (Transform child in _oneHundredSpawner.transform)
            Destroy(child.gameObject);

        _betAmount = 0;
        yield return null;
    }

    /// <summary>
    /// Sets up a bet, initializes the current bet to 1 chip as a bet cannot be 0
    /// </summary>
    public IEnumerator SetUpBet(int newChips)
    {
        yield return StartCoroutine(ResetBettingBoard());

        _totalChips = newChips;
        _chipsText.SetText($"Total Chips \n{newChips}");

        // Initialize the bet to 1 chip 
        SpawnChip(_onePrefab, _oneSpawner, 1);
    }

    /// <summary>
    /// Increments the bet spawning a chip at the spawnPoint and increments the bet amount by the chip value
    /// </summary>
    public void SpawnChip(GameObject chipPrefab, GameObject spawnPoint, int betIncrement)
    {
        if(_betAmount + betIncrement <= _totalChips && spawnPoint.transform.childCount < 9)
        {
            _betAmount += betIncrement;
            _betText.SetText($"Bet \n{_betAmount}");

            // Spawn a chip at the spawnPoint and set the new chip to be the last child of the spawnPoint
            Vector3 position = new Vector3(
                spawnPoint.transform.position.x,
                spawnPoint.transform.position.y + spawnPoint.transform.childCount * _chipHeight,
                spawnPoint.transform.position.z
            );
            GameObject newChip = Instantiate(chipPrefab, position, Quaternion.identity);
            newChip.transform.SetParent(spawnPoint.transform, true);
            newChip.transform.SetAsLastSibling();
        }

    }

    /// <summary>
    /// Decrements the bet by removing a chip from the spawnPoint and decrements the bet amount by the chip value
    /// </summary>
    public void DestroyChip(GameObject spawnPoint, int betDecrement)
    {
        if (_betAmount - betDecrement > 0 && spawnPoint.transform.childCount > 0)
        {
            _betAmount -= betDecrement;
            _betText.SetText($"Bet \n{_betAmount}");

            // Destroy last child (top chip)
            Transform lastChild = spawnPoint.transform.GetChild(spawnPoint.transform.childCount - 1);
            Destroy(lastChild.gameObject);
        }
    }

    /// <summary>
    /// Fire the event and let the game manager know it has been fired
    /// </summary>
    public void PlaceBet(int amount) => OnBetPlaced?.Invoke(_betAmount);
}
