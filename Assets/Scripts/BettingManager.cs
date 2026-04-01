using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;

public class BettingManager : MonoBehaviour
{
    private int _betAmount = 0;
    private int _totalChips = 0;
    private float _chipHeight = .0125f;

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

    public event System.Action<int> OnBetPlaced; // int = bet amount

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject == _positiveOneHundred)
                {
                    SpawnChip(_oneHundredPrefab, _oneHundredSpawner, 100);
                }
                else if (hit.transform.gameObject == _negativeOneHundred)
                {
                    DestroyChip(_oneHundredSpawner, 100);
                }
                else if (hit.transform.gameObject == _positiveTen)
                {
                    SpawnChip(_tenPrefab, _tenSpawner, 10);
                }
                else if (hit.transform.gameObject == _negativeTen)
                {
                    DestroyChip(_tenSpawner, 10);
                }
                else if (hit.transform.gameObject == _positiveOne)
                {
                    SpawnChip(_onePrefab, _oneSpawner, 1);
                }
                else if (hit.transform.gameObject == _negativeOne)
                {
                    DestroyChip(_oneSpawner, 1);
                }
                else if (hit.transform.gameObject == _placeBetButton)
                {
                    PlaceBet(_betAmount);
                }
            }
        }
    }

    public void ResetBettingBoard()
    {
        foreach (Transform child in _oneSpawner.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in _tenSpawner.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in _oneHundredSpawner.transform)
        {
            Destroy(child.gameObject);
        }
        _betAmount = 0;
    }

    public void SetUpBet(int newChips)
    {
        ResetBettingBoard();

        _totalChips = newChips;
        _chipsText.SetText($"Total Chips \n{newChips}");
        _betText.SetText($"Bet \n{_betAmount}");
        SpawnChip(_onePrefab, _oneSpawner, 1);
    }

    public void SpawnChip(GameObject chipPrefab, GameObject spawnPoint, int betIncrement)
    {
        if(_betAmount + betIncrement <= _totalChips && spawnPoint.transform.childCount < 10)
        {
            _betAmount += betIncrement;
            _betText.SetText($"Bet \n{_betAmount}");
            Vector3 position = new Vector3(
                spawnPoint.transform.position.x,
                spawnPoint.transform.position.y + spawnPoint.transform.childCount * _chipHeight,
                spawnPoint.transform.position.z
            );
            GameObject newChip = Instantiate(chipPrefab, position, Quaternion.identity);
            newChip.transform.SetParent(spawnPoint.transform, true);
            newChip.transform.SetAsLastSibling();
            Debug.Log(spawnPoint.transform.childCount);
        }

    }

    public void DestroyChip(GameObject spawnPoint, int betDecrement)
    {
        if (_betAmount - betDecrement > 0 && spawnPoint.transform.childCount > 0)
        {
            Debug.Log(spawnPoint.transform.childCount);
            _betAmount -= betDecrement;
            _betText.SetText($"Bet \n{_betAmount}");
            Transform lastChild = spawnPoint.transform.GetChild(spawnPoint.transform.childCount - 1);
            Destroy(lastChild.gameObject);
        }
    }

    public int GetBet()
    {
        return _betAmount;
    }

    public void PlaceBet(int amount)
    {

        // Fire event
        OnBetPlaced?.Invoke(_betAmount);
    }
}
