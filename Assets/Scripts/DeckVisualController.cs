using UnityEngine;

/// <summary>
/// Controls the visible size of the deck in the scene.
/// Attach this to your DeckVisual object or a parent object that represents the stack.
/// </summary>
public class DeckVisualController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _stackVisual;

    [Header("Deck Thickness")]
    [Tooltip("Thickness when the deck is full.")]
    [SerializeField] private float _maxThickness = 0.12f;

    [Tooltip("Thickness when the deck is almost empty.")]
    [SerializeField] private float _minThickness = 0.01f;

    [Tooltip("Which local axis represents thickness? Usually Y for upright cube, sometimes Z if rotated.")]
    [SerializeField] private DeckThicknessAxis _thicknessAxis = DeckThicknessAxis.Y;

    private Vector3 _baseScale;

    private void Awake()
    {
        // CHANGE: defaults to this transform if none assigned
        // WHY: makes setup easier in Unity
        if (_stackVisual == null)
            _stackVisual = transform;

        _baseScale = _stackVisual.localScale;
    }

    private void Start()
    {
        Refresh();
    }

    /// <summary>
    /// Recalculates the visual stack size based on cards remaining in the shoe.
    /// </summary>
    public void Refresh()
    {
        float ratio = Mathf.Clamp01(Deck.RemainingRatio);

        Vector3 scale = _baseScale;
        float thickness = Mathf.Lerp(_minThickness, _maxThickness, ratio);

        switch (_thicknessAxis)
        {
            case DeckThicknessAxis.X:
                scale.x = thickness;
                break;
            case DeckThicknessAxis.Y:
                scale.y = thickness;
                break;
            case DeckThicknessAxis.Z:
                scale.z = thickness;
                break;
        }

        _stackVisual.localScale = scale;
    }

    /// <summary>
    /// Call this every time a card is drawn.
    /// </summary>
    public void OnCardDrawn()
    {
        Refresh();
    }

    /// <summary>
    /// Call this after a shuffle / new shoe.
    /// </summary>
    public void OnDeckReset()
    {
        Refresh();
    }
}

public enum DeckThicknessAxis
{
    X,
    Y,
    Z
}
