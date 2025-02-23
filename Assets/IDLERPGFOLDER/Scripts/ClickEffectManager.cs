using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ClickEffectManager : MonoBehaviour
{
    [SerializeField] private GameObject particleEffectPrefab;
    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private float effectDuration = 1f;
    
    private void Start()
    {
        if (targetCanvas == null)
        {
            targetCanvas = FindObjectOfType<Canvas>();
        }
        
        // ตรวจสอบว่า Canvas เป็น Overlay
        if (targetCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            Debug.LogWarning("Canvas ควรตั้งค่าเป็น Screen Space - Overlay");
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)||Input.GetMouseButtonDown(1) )
        {
            CreateUIParticleEffect(Input.mousePosition);
        }
    }

    private void CreateUIParticleEffect(Vector2 position)
    {
        // สร้าง Container สำหรับ Particle
        GameObject container = new GameObject("ParticleContainer");
        RectTransform rectTransform = container.AddComponent<RectTransform>();
        
        // ตั้งค่า Container
        rectTransform.SetParent(targetCanvas.transform, false);
        rectTransform.position = position;
        // สร้าง Particle Effect ในฐานะลูกของ Container
        GameObject effect = Instantiate(particleEffectPrefab, Vector3.zero, Quaternion.identity);
        effect.transform.SetParent(container.transform, false);
        
        // ลบ Effect เมื่อครบเวลา
        Destroy(container, effectDuration);
    }
}
