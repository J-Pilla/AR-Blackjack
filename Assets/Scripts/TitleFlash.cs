using UnityEngine;
using TMPro;

public class TitleFlash : MonoBehaviour
{
    public float speed = 2f;
    public float minBrightness = 0.7f;
    public float maxBrightness = 1.3f;

    private TextMeshProUGUI text;
    private Color originalColor;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        originalColor = text.color;
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) / 2f;
        float brightness = Mathf.Lerp(minBrightness, maxBrightness, t);

        text.color = originalColor * brightness;
    }
}