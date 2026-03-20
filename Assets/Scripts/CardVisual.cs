using UnityEngine;

public class CardVisual : MonoBehaviour
{

    /*
     * I am using camelCase for the public fields, and for the private fields I am using _camelCase
     */
    public Card card;
    public bool isFaceUp; // this field should be hidden in the inspector later, but for testing purposes it's shown for now.

    // private refs
    private MeshRenderer _faceRenderer;
    private MeshRenderer _backRenderer;


    // find 'face and back' renderers in children
    private void Awake()
    {
        foreach(MeshRenderer face in GetComponentsInChildren<MeshRenderer>())
        {
            if(face.gameObject.name == "Face")
            {
                _faceRenderer = face;
            }
            else if(face.gameObject.name == "Back")
            {
                _backRenderer = face;
            }
        }

        if (_faceRenderer == null) Debug.LogError("CardVisual: No child named 'Face' found.", this);
        if (_backRenderer == null) Debug.LogError("CardVisual: No child named 'Back' found.", this);

        // for testing
        card = new Card(Suit.Hearts, Rank.Ace);
        isFaceUp = true;
        LoadFaceTexture();
    }

    /// <summary>
    /// The Initialize method is responsible for setting up the card visual with the given card data and face-up state.
    /// </summary>
    /// <param name="card">the card</param>
    /// <param name="isFaceUp">the Face-up state</param>
    public void Initialize(Card card, bool isFaceUp = false)
    {
        this.card = card;
        this.isFaceUp = isFaceUp;
        LoadFaceTexture();
    }


    /// <summary>
    /// Loads the texture for the card's face and applies it to the face renderer.
    /// </summary>
    private void LoadFaceTexture()
    {
        string path = card.ResourcePath;
        Texture texture = Resources.Load<Texture>(path);

        //Debug.Log($"CardVisual: Loading texture for card '{Card}' from path '{path}'. Texture found: {texture != null}");

        if (texture == null)
        {
            Debug.LogError($"CardVisual: Could not load texture at path '{path}' for card '{card}'.");
            return;
        }
        var block = new MaterialPropertyBlock();
        _faceRenderer.GetPropertyBlock(block);
        block.SetTexture("_BaseMap", texture);
        block.SetTexture("_MainTex", texture); // fallback for stadard shader
        _faceRenderer.SetPropertyBlock(block);
    }
    // Card flip
    // A coroutine should be implemented to handle the card flip animation!

}
