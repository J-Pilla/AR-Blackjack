using System.Collections;
using UnityEngine;

/// <summary>
/// lives on every instantiated Card prefab.
/// </summary>
public class CardVisual : MonoBehaviour
{
    // fields
    [Header("References")]
    [SerializeField] private MeshRenderer _faceRenderer;
    [SerializeField] private MeshRenderer _backRenderer;


    // 
    private Card _card;
    private bool _isFaceUp;
    private bool _isFlipping;

    // CONSTANTS
    private const float FLIP_DURATION = 0.4f; // seconds for the full flip

    // properties
    public Card Card { get => _card; }
    public bool IsFaceUp { get => _isFaceUp; }

    // Unity lifecycle
    private void Awake()
    {
        if (_faceRenderer == null || _backRenderer == null)
        {
            Debug.LogError($"CardVisual on '{gameObject.name}': face or back renderer is not assigned.", this);
        }
    }



    // Public API

    /// <summary>
    /// called by the GameManager immediately after spawning a card.
    /// </summary>
    public void Initialize(Card card, bool isFaceUp = false)
    {
        _card = card;
        LoadFaceTexture();
        // TODO: set back texture if needed
    }

    /// <summary>
    /// Flips the card face up with an animation. Public API for GameManager to call when dealing or revealing cards.
    /// </summary>
    public void FlipFaceUp ()
    {
        if (_isFaceUp || _isFlipping)
        {
            return;
        }
        StartCoroutine(FlipCoroutine(true));
    }



    /* Private methods */

    /// <summary>
    /// Loads the texture for the card's face and applies it to the face renderer.h
    /// uses a MaterialPropertyBlock to avoid creating new material instances
    /// </summary>
    private void LoadFaceTexture()
    {
        if (_card == null)
        {
            Debug.LogError("CardVisual.LoadFaceTexture: card is null."); // should never happen if Initialize is called correctly
            return;
        }


        string path = _card.ResourcePath;
        Texture texture = Resources.Load<Texture>(path);

        //Debug.Log($"CardVisual: Loading texture for card '{Card}' from path '{path}'. Texture found: {texture != null}");

        if (texture == null)
        {
            Debug.LogError($"CardVisual: Could not load texture at path '{path}' for card '{_card}'.");
            return;
        }
        MaterialPropertyBlock block = new();
        _faceRenderer.GetPropertyBlock(block);
        block.SetTexture("_BaseMap", texture);
        block.SetTexture("_MainTex", texture); // fallback for stadard shader
        _faceRenderer.SetPropertyBlock(block);
    }

    /// <summary>
    /// Immediately sets the face-up state without animation if animate is false.
    /// </summary>
    private void SetFaceUp(bool faceUp, bool animate)
    {
        if (_isFlipping) // just to be safe, but ideally the GameManager should never call this while a flip is in progress
        {
            Debug.LogWarning($"CardVisual on '{gameObject.name}': Attempted to set face up to {faceUp} while already flipping. Ignoring.", this);
            return;
        }

        if (animate)
        {
            StartCoroutine(FlipCoroutine(faceUp));
            return;
        }
        _isFaceUp = faceUp;
        _faceRenderer.gameObject.SetActive(faceUp);
        _backRenderer.gameObject.SetActive(!faceUp);
    }



    /// <summary>
    /// Animates a Y-axis rotation:
    ///   Phase 1 (0  to 90).
    ///   Phase 2 (90 to 0)  – rotate back and show new side.
    /// </summary>
    private IEnumerator FlipCoroutine(bool targetFaceUp)
    {
        _isFlipping = true;

        float halfDuration = FLIP_DURATION / 2f; // Phase 1: rotate from 0 to 90
        float elapsed = 0f;
        Quaternion startRotation = transform.localRotation;


        // Phase 1 (0 to 90)
        Quaternion edgeRotation = Quaternion.Euler(
            startRotation.eulerAngles.x,
            90f,
            startRotation.eulerAngles.z);

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(startRotation, edgeRotation, elapsed / halfDuration);
            yield return null;
        }

        transform.localRotation = edgeRotation;

        // Swap faces at the midpoint
        _isFaceUp = targetFaceUp;
        _faceRenderer.gameObject.SetActive(_isFaceUp);
        _backRenderer.gameObject.SetActive(!_isFaceUp);

        // Phase 2 (90 back to 0)
        Quaternion endRotation = Quaternion.Euler(
            startRotation.eulerAngles.x,
            0f,
            startRotation.eulerAngles.z);

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(edgeRotation, endRotation, elapsed / halfDuration);
            yield return null;
        }
        transform.localRotation = endRotation;

        _isFlipping = false;
    }
}
