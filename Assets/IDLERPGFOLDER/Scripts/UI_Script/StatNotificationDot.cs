using UnityEngine;
using UnityEngine.UI;

public class StatNotificationDot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private GameObject notificationDot;
    [SerializeField] private Image dotImage;
    
    [Header("Settings")]
    [SerializeField] private Color dotColor = Color.red;
    [SerializeField] private float pulseMinScale = 0.8f;
    [SerializeField] private float pulseMaxScale = 1.2f;
    [SerializeField] private float pulseSpeed = 2f;
    
    private void Start()
    {
        // กำหนดค่าสีให้จุดแจ้งเตือน
        if (dotImage != null)
        {
            dotImage.color = dotColor;
        }
        
        // ซ่อนจุดแจ้งเตือนตอนเริ่มต้น
        notificationDot.SetActive(false);
        
        // ลงทะเบียน callback เมื่อมีการเปลี่ยนแปลงแต้มสเตตัส
        playerStats.OnStatPointsChanged += CheckStatPoints;
    }
    
    private void OnDestroy()
    {
        // ยกเลิกการลงทะเบียน callback
        if (playerStats != null)
        {
            playerStats.OnStatPointsChanged -= CheckStatPoints;
        }
    }
    
    private void Update()
    {
        if (notificationDot.activeSelf)
        {
            // สร้าง effect กระพริบ
            float pulse = Mathf.Lerp(pulseMinScale, pulseMaxScale, 
                (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
            notificationDot.transform.localScale = Vector3.one * pulse;
        }
    }
    
    private void CheckStatPoints(int availablePoints)
    {
        // แสดงหรือซ่อนจุดแจ้งเตือนตามจำนวนแต้มที่มี
        notificationDot.SetActive(availablePoints > 0);
    }
}