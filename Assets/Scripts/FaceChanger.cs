using UnityEngine;

/// <summary>
/// FaceChange is deprecated, use CardVisual instead.
/// </summary>
public class FaceChanger : MonoBehaviour
{
    private MeshRenderer faceRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.gameObject.name == "Face")
            {
                faceRenderer = renderer;
            }
        }

        string file = "card-clubs-1";
        /*
         * This script has a downside, that it will create a new material instance every time
         * it's called, which can lead to a lot of mterial instances during the game!
         * Instead, it's better to use MaterialPropertyBlock object and call SetTexture on it, 
         * then apply the block to the renderer with SetPropertyBlock.
         * <script>
             *  MaterialPropertyBlock block = new MaterialPropertyBlock();
             *  faceRenderer.GetPropertyBlock(block); // this is important, this copies existing properties to the block to avoid overwriting them
             *  block.SetTexture("_BaseMap", texture);
             *  faceRenderer.SetPropertyBlock(block);
         * </script>
        */
        faceRenderer.material.mainTexture = Resources.Load(file, typeof(Texture)) as Texture;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
