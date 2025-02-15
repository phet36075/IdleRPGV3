using UnityEngine;

public class IndicatorFadeEffect : MonoBehaviour
{
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float maxAlpha = 0.6f;
    [SerializeField] private bool enablePulse = true;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseMinAlpha = 0.3f;
    
    // สำหรับ HDRP
    [SerializeField] private string colorPropertyName = "_UnlitColor"; // หรือ "_UnlitColor" ขึ้นอยู่กับ shader
    [SerializeField] private Color baseColor = Color.red; // สีพื้นฐาน
    [SerializeField] private float intensity = 1f; // ความเข้มของแสง (HDR)
    
    private Material indicatorMaterial;
    private float currentTime = 0f;
    private bool isFading = true;
    private static readonly int ColorProperty = Shader.PropertyToID("_UnlitColor"); // cache property ID

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // สร้าง material instance
            indicatorMaterial = new Material(renderer.material);
            renderer.material = indicatorMaterial;
            
            // ตั้งค่าสีเริ่มต้นด้วย alpha = 0
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
            // Fade In Effect
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, maxAlpha, currentTime / fadeInDuration);
            UpdateMaterialColor(alpha);

            if (currentTime >= fadeInDuration)
            {
                isFading = false;
            }
        }
        else if (enablePulse)
        {
            // Pulse Effect
            float pulseAlpha = Mathf.Lerp(pulseMinAlpha, maxAlpha, 
                (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f);
            UpdateMaterialColor(pulseAlpha);
        }
    }

    private void UpdateMaterialColor(float alpha)
    {
        Color color = baseColor * intensity; // คูณด้วย intensity สำหรับ HDR
        color.a = alpha;
        indicatorMaterial.SetColor(ColorProperty, color);
    }

    void OnValidate()
    {
        // อัพเดทค่าเมื่อมีการเปลี่ยนแปลงใน Inspector
        if (indicatorMaterial != null)
        {
            UpdateMaterialColor(isFading ? 0f : maxAlpha);
        }
    }
}