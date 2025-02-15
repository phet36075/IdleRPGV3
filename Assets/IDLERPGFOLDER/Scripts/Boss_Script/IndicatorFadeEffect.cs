using UnityEngine;

public class IndicatorFadeEffect : MonoBehaviour
{
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.3f;  // เพิ่มระยะเวลา fade out
    [SerializeField] private float maxAlpha = 0.6f;
    [SerializeField] private string colorPropertyName = "_BaseColor";
    [SerializeField] private Color baseColor = Color.red;
    [SerializeField] private float intensity = 1f;
    
    private Material indicatorMaterial;
    private float currentTime = 0f;
    private bool isFading = true;
    private bool isFadingOut = false;
    private static readonly int ColorProperty = Shader.PropertyToID("_BaseColor");
    private System.Action onFadeOutComplete; // callback เมื่อ fade out เสร็จ

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            indicatorMaterial = new Material(renderer.material);
            renderer.material = indicatorMaterial;
            
            Color startColor = baseColor * intensity;
            startColor.a = 0f;
            indicatorMaterial.SetColor(ColorProperty, startColor);
        }
    }

    void Update()
    {
        if (indicatorMaterial == null) return;

        if (isFading)
        {
            // Fade In
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, maxAlpha, currentTime / fadeInDuration);
            UpdateMaterialColor(alpha);

            if (currentTime >= fadeInDuration)
            {
                isFading = false;
                currentTime = 0f;  // รีเซ็ตเวลาสำหรับ fade out
            }
        }
        else if (isFadingOut)
        {
            // Fade Out
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(maxAlpha, 0f, currentTime / fadeOutDuration);
            UpdateMaterialColor(alpha);

            if (currentTime >= fadeOutDuration)
            {
                isFadingOut = false;
                if (onFadeOutComplete != null)
                {
                    onFadeOutComplete.Invoke();
                }
                Destroy(gameObject);  // ทำลาย indicator เมื่อ fade out เสร็จ
            }
        }
    }

    private void UpdateMaterialColor(float alpha)
    {
        Color color = baseColor * intensity;
        color.a = alpha;
        indicatorMaterial.SetColor(ColorProperty, color);
    }

    public void StartFadeOut(System.Action onComplete = null)
    {
        isFading = false;
        isFadingOut = true;
        currentTime = 0f;
        onFadeOutComplete = onComplete;
    }
}