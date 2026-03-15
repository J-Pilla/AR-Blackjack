using UnityEngine;

public class CardVisual : MonoBehaviour
{
    public Card Card; // maybe this should be hidden in the inspector, because it will be set by the game manager
    public bool IsFaceUp; // 

    // private refs
    private MeshRenderer _faceRenderer;
    private MeshRenderer _backRenderer;


    // find face and back renderers in children
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
        Card = new Card(Suit.Hearts, Rank.Ace);
        IsFaceUp = true;
        LoadFaceTexture();
    }


    //public void Initialize(Card card, bool isFaceUp = false)
    //{
    //    Card = new Card(Suit.Hearts, Rank.Ace);
    //    IsFaceUp = isFaceUp;
    //    LoadFaceTexture();
    //}


    /// <summary>
    /// Loads the texture for the card's face and applies it to the face renderer.
    /// </summary>
    private void LoadFaceTexture()
    {
        string path = Card.ResourcePath();
        Texture texture = Resources.Load<Texture>(path);

        //Debug.Log($"CardVisual: Loading texture for card '{Card}' from path '{path}'. Texture found: {texture != null}");

        if (texture == null)
        {
            Debug.LogError($"CardVisual: Could not load texture at path '{path}' for card '{Card}'.");
            return;
        }
        var block = new MaterialPropertyBlock();
        _faceRenderer.GetPropertyBlock(block);
        block.SetTexture("_BaseMap", texture);
        block.SetTexture("_MainTex", texture); // fallback for stadard shader
        _faceRenderer.SetPropertyBlock(block);
    }
}
