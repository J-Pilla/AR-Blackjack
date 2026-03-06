using UnityEngine;

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
        faceRenderer.material.mainTexture = Resources.Load(file, typeof(Texture)) as Texture;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
