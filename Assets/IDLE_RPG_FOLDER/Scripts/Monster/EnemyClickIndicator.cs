using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClickIndicator : MonoBehaviour
{
    public GameObject indicatorPrefab; // Prefab ของ Indicator
    private GameObject currentIndicator;
    public Transform cam;
    public AIController playerAIController;
    void Update()
    {
        if (currentIndicator != null)
        {
            currentIndicator.transform.LookAt(currentIndicator.transform.position + cam.forward);
        }
       
        if (Input.GetMouseButtonDown(0)) // ตรวจจับการคลิกซ้ายของเมาส์
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                
                // ตรวจสอบว่าเราคลิกโดนศัตรูหรือไม่
                if (hit.collider.CompareTag("Enemy") ||  hit.collider.CompareTag("Boss")) // สมมติว่า Tag ของศัตรูคือ "Enemy" 
                {
                    Transform enemyTransform = hit.transform;
                    playerAIController.SetTarget(enemyTransform);
                    
                    // ถ้ามี Indicator อยู่แล้วให้ทำลาย
                    if (currentIndicator != null)
                    {
                        Destroy(currentIndicator);
                    }

                    // สร้าง Indicator ใหม่ที่ตำแหน่งของศัตรู
                    currentIndicator = Instantiate(indicatorPrefab, hit.transform.position, Quaternion.identity);

                    // กำหนดให้ Indicator ติดตามศัตรู
                    currentIndicator.transform.SetParent(hit.transform);
                   // currentIndicator.transform.LookAt(currentIndicator.transform.position + cam.forward);
                }
                
                else
                {
                    Destroy(currentIndicator);
                }
            }
        }
    }
}
