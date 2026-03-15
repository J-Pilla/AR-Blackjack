# AR Blackjack Game

## Game flow

- App Start: ARFoundation initialises the camera and begins plane detection.
- Place table: when a table is found, the position is recorded and a table prefab is instantiated at that position.
- Deal initial cards.
- Player turn.
- Dealer turn.
- Check result.
- New round || Reset.

## Game Components:

```
Assets/
    Scripts/
            GameManager
            UIManager:
            CardVisual: ## Yet to be created
            FaceChanger.cs
            Hand  ## Yet to be created
            Deck  ## Yet to be created
            ARService

```

## 1. FaceChanger.cs

The script is attached to a Card prefab, it finds the `Face` as a ` MeshRenderer` object among the card's children and replaces its `mainTexture` at runtime with the approprate card from the resources.

```c#
public class FaceChanger : MonoBehaviour
{
    private MeshRenderer faceRenderer;
    // THe script
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
}
```

---
