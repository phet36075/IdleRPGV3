using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    private TestTeleportPlayer _teleportPlayer;
    private StageManager _stageManager;
    public StageData _StageData;
    public GameObject[] enemyPrefab;  // ตัว prefab ของศัตรู
    public float spawnInterval = 2f;  // ระยะเวลาระหว่างการเกิดของศัตรูแต่ละตัว
    public float spawnRadius = 10f;  // รัศมีของพื้นที่ที่ศัตรูจะเกิด
    public int maxEnemies = 10;  // จำนวนศัตรูสูงสุดที่จะเกิด
    public List<Transform> spawnPoints;  // ลิสต์ของตำแหน่งที่จะให้ศัตรูเกิด
    private EnemyHealth _enemyHealth;
    public int enemiesSpawned = 0;  // จำนวนศัตรูที่เกิดแล้ว
    public int enemiesDefeated = 0;  // จำนวนศัตรูที่ถูกกำจัดแล้ว

    public GameObject NextButton;
 
    public GameObject WinUI;
    private GameObject enemy;
    public int currentStage = 1;
    public GameObject BossUI;

    public int MapIndex;
    public bool isClearing = false; // เพิ่มตัวแปรควบคุมการเคลียร์
    void Start()
    {
        _enemyHealth = FindObjectOfType<EnemyHealth>();
        _teleportPlayer = FindObjectOfType<TestTeleportPlayer>();
        _stageManager = FindObjectOfType<StageManager>();
        currentStage = _StageData.currentStage;
        // เริ่มการเรียกฟังก์ชัน SpawnEnemy ซ้ำๆ ทุก spawnInterval วินาที
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
    }
    void Update()
    {
        // เพิ่มการตรวจสอบการกดปุ่ม P
        if (Input.GetKeyDown(KeyCode.P))
        {
            ClearAllEnemies();
        }
    }
    public void ClearAllEnemies()
    {
        isClearing = true; // ตั้งค่าเป็น true ก่อนเริ่มเคลียร์

        // หาศัตรูทั้งหมดในฉาก
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        // ทำลายศัตรูทุกตัว
        foreach (GameObject enemy in allEnemies)
        {
            Destroy(enemy);
        }

        // รีเซ็ตตัวแปรที่เกี่ยวข้อง
        enemiesSpawned = 0;
        enemiesDefeated = 0;
        StartCoroutine(WaitAndResetClear());

        Debug.Log("All enemies cleared!");
    }

    IEnumerator WaitAndResetClear()
    {
        yield return new WaitForSeconds(5f);
        isClearing = false;
    }
    public int GetStage()
    {
        return currentStage;
    }

    public void NextStage()
    {
        currentStage += 1;
      /*  if (currentStage > 5)
        {*/
            _stageManager.ChangeMap(MapIndex);
      /*  }
        else if(currentStage <=5 )
        {
            _stageManager.ChangeMap(currentStage -1);
        }*/
        enemiesDefeated = 0;
        enemiesSpawned = 0;
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);

      

    }

   
    public void GotoBoss()
    {
       /* if (currentStage > 5)
        {*/
            _stageManager.ChangeMap(MapIndex);
      /*  }else if(currentStage <=5 )
        {
            _stageManager.ChangeMap(currentStage -1);
        }*/
        Vector3 newpos = new Vector3(-8, 2.1f, -6);
        _teleportPlayer.TeleportPlayer(newpos);
        ClearAllEnemies();
        enemiesDefeated = 0;
        enemiesSpawned = 0;
        maxEnemies = 1;
      
       
       
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
        currentStage += 1;
        BossUI.SetActive(false);
    }
    public void SpawnEnemy()
    {
        // ตรวจสอบว่าจำนวนศัตรูที่เกิดแล้วยังไม่เกินค่าสูงสุด
        if (enemiesSpawned < maxEnemies)
        {
            Vector3 spawnPos;

            if (spawnPoints.Count > 0)
            {
                // สุ่มเลือกตำแหน่งจาก spawnPoints
                spawnPos = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
            }
            else
            {
                // ถ้าไม่มี spawnPoints ให้สุ่มตำแหน่งภายในวงกลมเหมือนเดิม
                Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
                spawnPos = new Vector3(randomPos.x, 0, randomPos.y) + transform.position;
            }


           
           
           if ((currentStage - 1) % 5 == 0) // ด่าน 1 6 11 16
            {
                enemy = Instantiate(enemyPrefab[0], spawnPos, Quaternion.identity);
                maxEnemies = 5;
                MapIndex = 1;
            }
            else if ((currentStage - 2) % 5 == 0) // ด่าน 2 7 12 17
            {
                enemy = Instantiate(enemyPrefab[1], spawnPos, Quaternion.identity);
                maxEnemies = 5;
                MapIndex = 2;
            }
            else if ((currentStage - 3) % 5 == 0) // ด่าน 3 8 13 18
            {
                enemy = Instantiate(enemyPrefab[5], spawnPos, Quaternion.identity);
                maxEnemies = 5;
                MapIndex = 3;
            }
            else 
            if ((currentStage - 4) % 5 == 0) // ด่าน 4,9 , 14 19
            {
                enemy = Instantiate(enemyPrefab[2], spawnPos, Quaternion.identity);
                maxEnemies = 5;
                MapIndex = 4;
            }
            else if (currentStage % 5 == 0) // ด่าน 5 10
            {
                enemy = Instantiate(enemyPrefab[4], spawnPos, Quaternion.identity);
                maxEnemies = 1;
                MapIndex = 5;
            }
            else
            {
                enemy = Instantiate(enemyPrefab[2], spawnPos, Quaternion.identity);
                maxEnemies = 5;
                MapIndex = 5;
            }
               
            
           
            
            // สร้างศัตรูที่ตำแหน่งที่เลือกได้
            
            //Destroy(SpawnFX,0.5f);
            // เพิ่มจำนวนศัตรูที่เกิดแล้ว
            enemiesSpawned++;

            // เพิ่ม EnemyDefeatedNotifier component ให้กับศัตรู
            enemy.AddComponent<EnemyDefeatedNotifier>().spawner = this;
        }
        else
        {
            // หยุดการ spawn เมื่อครบจำนวนที่กำหนด
            CancelInvoke("SpawnEnemy");
        }
    }

    public void EnemyDefeated()
    {
        if (!isClearing) // เช็คว่าไม่ได้อยู่ในระหว่างการเคลียร์
        {
            enemiesDefeated++;
            // ... (ส่วนอื่นๆ ของ EnemyDefeated ยังคงเหมือนเดิม)
        }
       // isClearing = false; // ตั้งค่ากลับเป็น false หลังจากเคลียร์เสร็จ
        // ตรวจสอบว่าได้กำจัดศัตรูครบตามจำนวนที่กำหนดหรือยัง
        if (enemiesDefeated >= maxEnemies)
        {
            if ((currentStage - 4) % 5 == 0)
            {
                Debug.Log("All enemies defeated! LOOPPPPPPPP");
                BossUI.SetActive(true);
                enemiesDefeated = 0;
                enemiesSpawned = 0;
                InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
            }
           
            else
            {
                BossUI.SetActive(false);
                Debug.Log("All enemies defeated!");
                WinUI.gameObject.SetActive(true);
            }

           
            
            // ทำอะไรก็ตามที่คุณต้องการเมื่อกำจัดศัตรูครบจำนวน เช่น แสดงข้อความชนะ หรือโหลดด่านต่อไป
        }
    }

    public void ResetEnemies()
    {
        enemiesDefeated = 0;
        NextButton.SetActive(false);
    }
}

// Component เพิ่มเติมสำหรับแจ้งเตือนเมื่อศัตรูถูกกำจัด
public class EnemyDefeatedNotifier : MonoBehaviour
{
    public EnemySpawner spawner;


    private void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.EnemyDefeated();
        }
    }
    
   
}
