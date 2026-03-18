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
            GameManager: Yet to be created
            UIManager: Yet to be created
            CardVisual
            FaceChanger.cs ***Deprecated***
            Hand  ## Yet to be created
            Deck  ## Yet to be created
            ARService ## Yet to be created

```

## 1. FaceChanger.cs **_Deprecated_**

The script is attached to a Card prefab, it finds the `Face` as a `MeshRenderer` object among the card's children and replaces its `mainTexture` at runtime with the approprate card from the resources.

> Important note: This script has a flaw; whenever `faceRenderer.material.mainTexture` is accessed, unity creates a new copy of the material.
> That's why this functionality is re-implemented in a different way in the `CardVisual.cs` script.

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

## 2. CardVisual.cs

Lives on every instanctiated Card `prefab`.

### Responsibilities:

- This script replaces the texture mapping functionality found in `FaceChanger.cs`.
- Load the correct face texture from the `Resources` at runtime.
- Expose a `Flip()` method that animates a Y-axis rotation to flip the card up and down
- other stuff...
