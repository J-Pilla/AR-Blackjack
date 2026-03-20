using UnityEngine;

public class CardVisual : MonoBehaviour
{
    // fields
    [SerializeField] private MeshRenderer _faceRenderer;
    [SerializeField] private MeshRenderer _backRenderer;
    [SerializeField] private Card _card;
    [SerializeField] private bool _isFaceUp; // this field should be hidden in the inspector later, but for testing purposes it's shown for now.

    // properties
    public Card Card { get { return _card; } }

    // find 'face and back' renderers in children
    private void Awake()
    {
        // for testing
        _card = new(Suit.Hearts, Rank.Ace);
        _isFaceUp = true;
        LoadFaceTexture();
    }

    /// <summary>
    /// The Initialize method is responsible for setting up the card visual with the given card data and face-up state.
    /// </summary>
    /// <param name="card">the card</param>
    /// <param name="isFaceUp">the Face-up state</param>
    public void Initialize(Card card, bool isFaceUp = false)
    {
        _card = card;
        _isFaceUp = isFaceUp;
        LoadFaceTexture();
    }


    /// <summary>
    /// Loads the texture for the card's face and applies it to the face renderer.
    /// </summary>
    private void LoadFaceTexture()
    {
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
    // Card flip
    // A coroutine should be implemented to handle the card flip animation!

}
