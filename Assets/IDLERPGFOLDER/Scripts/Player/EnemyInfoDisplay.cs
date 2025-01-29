using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class EnemyInfoDisplay : MonoBehaviour
{
    public Camera mainCamera; 
    public GameObject infoPanel; 
    public TextMeshProUGUI enemyNameText; 
    public TextMeshProUGUI enemyHealthText;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        infoPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
       
        EnemyHealth enemyStats = enemyHealthText.GetComponent<EnemyHealth>();

        if (enemyStats != null)
        {
            // อัพเดทข้อความใน UI
            enemyNameText.text = "Name: " + enemyStats.EnemyData.enemyName;
            enemyHealthText.text = "Health: " + enemyStats.currentHealth.ToString();
            
            // แสดง Panel ข้อมูล
           
        }
        
        
        
        if (Input.GetMouseButtonDown(0)) // ตรวจจับการคลิกซ้าย
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Enemy")) // ตรวจสอบว่าเป็นศัตรูหรือไม่
                {
                   
                    infoPanel.SetActive(true);
                    
                  // EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();

                }
                else
                {
                    HideEnemyInfo();
                }
            }
        }

       
    }
    
   public void ShowEnemyInfo(GameObject enemy)
    {
        // สมมติว่า Enemy มีสคริปต์ที่ชื่อว่า "EnemyStats" ที่มีข้อมูล
        EnemyHealth enemyStats = enemy.GetComponent<EnemyHealth>();

        if (enemyStats != null)
        {
            // อัพเดทข้อความใน UI
            enemyNameText.text = "Name: " + enemyStats.EnemyData.enemyName;
            enemyHealthText.text = "Health: " + enemyStats.currentHealth.ToString();
            
            // แสดง Panel ข้อมูล
            infoPanel.SetActive(true);
        }
    }

    public void ShowEnemyInfo2()
    {
       
            infoPanel.SetActive(true);
        
    }
    
    
   public void HideEnemyInfo()
    {
            infoPanel.SetActive(false);
    }
    
    
}
