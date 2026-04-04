using UnityEngine;
using TMPro;

public class TitleFlash : MonoBehaviour
{
    [SerializeField] private float _speed = 11f;
    [SerializeField] private float _minBrightness = 0.7f;
    [SerializeField] private float _maxBrightness = 1.3f;

    private TextMeshProUGUI text;
    private Color originalColor;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        originalColor = text.color;
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * _speed) + 1f) / 2f;
        float brightness = Mathf.Lerp(_minBrightness, _maxBrightness, t);

        text.color = originalColor * brightness;
    }
}