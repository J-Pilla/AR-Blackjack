using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Handles AR plane detection and table placement.
/// </summary>
public class AutoPlaceBoard : MonoBehaviour
{
    /* inspector fields */

    [Header("Prefabs")]
    [SerializeField] private GameObject _blackjackTablePrefab;
    [SerializeField] private GameObject _placementIndicatorPrefab; // a simple ring/disc to show aim point

    [Header("Placement")]
    [Tooltip("Height offset so the board sits on top of the detected surface.")]
    [SerializeField] private float heightOffset = 0.01f;

    /* private state */

    private ARPlaneManager _planeManager;
    private ARRaycastManager _raycastManager;
    private GameObject _indicator;
    private GameObject _spawnedBoard;
    private bool _isPlaced;
    private readonly List<ARRaycastHit> _hits = new();

    /* events */

    /// <summary>
    /// Fired once when the board is successfully placed.
    /// GameManager subscribes to this to begin dealing.
    /// </summary>
    public event System.Action<GameObject> OnBoardPlaced;


    /* Unity lifecycle */
    private void Start()
    {
        _planeManager = GetComponent<ARPlaneManager>();
        _raycastManager = GetComponent<ARRaycastManager>();

        // Spawn the placement indicator but keep it hidden until a plane is found
        _indicator = Instantiate(_placementIndicatorPrefab);
        _indicator.SetActive(false);
    }

    private void Update()
    {
        if (_isPlaced) return;

        UpdateIndicator();

        if (Input.GetMouseButtonDown(0))
            HandleTapInput();
    }


    /* private methods */

    /// <summary>
    /// Casts a ray from the screen centre and moves the placement indicator
    /// to the hit point on a detected AR plane.
    /// </summary>
    private void UpdateIndicator()
    {
        Vector2 screenCentre = new(Screen.width / 2f, Screen.height / 2f);

        if (_raycastManager.Raycast(screenCentre, _hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = _hits[0].pose;
            Vector3 position = hitPose.position;
            position.y += heightOffset;

            // Face the indicator toward the camera on the horizontal plane
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0f;
            forward.Normalize();

            if (_indicator != null)
            {
                _indicator.SetActive(true);
                _indicator.transform.SetPositionAndRotation(
                    position,
                    Quaternion.LookRotation(forward));
            }
        }
        else if (_indicator != null)
            _indicator.SetActive(false);
    }

    /// <summary>
    /// Detects a screen tap (touch on mobile, mouse click in Unity Editor)
    /// and places the board when the indicator is visible.
    /// </summary>
    private void HandleTapInput()
    {
        if (_indicator == null || !_indicator.activeSelf) return;

        PlaceBoard(_indicator.transform.position, _indicator.transform.rotation);
    }

    /// <summary>
    /// Instantiates the board at the confirmed position and hides the indicator,
    /// and fires the OnBoardPlaced event so other systems can react.
    /// </summary>
    private void PlaceBoard(Vector3 position, Quaternion rotation)
    {
        _isPlaced = true;

        if (_indicator != null)
            _indicator.SetActive(false);

        _spawnedBoard = Instantiate(_blackjackTablePrefab, position, rotation);

        // Stop detecting planes once the board is placed to save resources
        _planeManager.enabled = false;
        foreach (ARPlane plane in _planeManager.trackables)
            plane.gameObject.SetActive(false);

        OnBoardPlaced?.Invoke(_spawnedBoard);
    }


    /* public APIs */

    /// <summary>
    /// Allows GameManager to re-enable placement for a full scene reset.
    /// </summary>
    public void ResetPlacement()
    {
        if (_spawnedBoard != null)
            Destroy(_spawnedBoard);

        _isPlaced = false;
        _planeManager.enabled = true;
    }
}
