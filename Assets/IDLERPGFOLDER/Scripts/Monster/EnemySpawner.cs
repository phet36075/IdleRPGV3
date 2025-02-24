using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[Serializable]
public class MonsterSet
{
    public string setName; // ชื่อชุด เช่น "Easy", "Normal", "Hard"
    public int[] enemyIndices; // ดัชนีมอนสเตอร์ในชุดนี้
}

[Serializable]
public class ElementalGroup
{
    public string elementName; // ชื่อธาตุ
    public MonsterSet[] monsterSets; // แต่ละธาตุมีหลายชุด
}

[Serializable]
public class BossSet
{
    public string setName;
    public int[] bossIndices; // บอสในชุดนี้
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Element Groups")]
    [SerializeField] private ElementalGroup[] elementalGroups;

    [Header("Boss Sets")]
    [SerializeField] private BossSet[] bossSets;

    [Header("Difficulty Settings")]
    [SerializeField] private int stagesPerDifficultyIncrease = 10; // จำนวนด่านก่อนเพิ่มความยาก
    [SerializeField] private bool randomizeSetSelection = false; // สุ่มเลือกชุดหรือไม่

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefab;

    private System.Random random = new System.Random();
    private int currentSet; // เพิ่มตัวแปรนี้
    private int GetCurrentSet()
    {
        if (randomizeSetSelection)
        {
            // สุ่มเลือกชุดจากที่มีทั้งหมด
            return random.Next(GetMaxAvailableSets());
        }
        
        // เลือกชุดตามความยากของด่าน
        return Mathf.Min((currentStage - 1) / stagesPerDifficultyIncrease, GetMaxAvailableSets() - 1);
    }

    private int GetMaxAvailableSets()
    {
        // หาจำนวนชุดสูงสุดที่มีในทุกธาตุ
        int maxSets = 0;
        foreach (var group in elementalGroups)
        {
            maxSets = Mathf.Max(maxSets, group.monsterSets?.Length ?? 0);
        }
        return maxSets;
    }

    private int GetEnemyIndexForStage()
    {
        int currentSet = GetCurrentSet();

        // ตรวจสอบด่านบอส
        if (currentStage % 5 == 0)
        {
            if (currentStage <= bossSets.Length * 5)
            {
                int bossSetIndex = (currentStage / 5 - 1) % bossSets.Length;
                var bossSet = bossSets[bossSetIndex];
                return bossSet.bossIndices[random.Next(bossSet.bossIndices.Length)];
            }
            else
            {
                var lastBossSet = bossSets[bossSets.Length - 1];
                return lastBossSet.bossIndices[random.Next(lastBossSet.bossIndices.Length)];
            }
        }

        // สร้าง pattern แบบตายตัวตามลำดับที่ต้องการ
        // 0=ไฟ, 1=ลม, 2=ดิน, 3=น้ำ, 4=แสง, 5=มืด
        int[] pattern = new int[] { 0, 0, 1, 2, 3, 0, 4, 5, 0, 1, 0, 2, 3, 4, 5, 0, 0, 1, 2, 3, 0, 4, 5, 0, 1, 0, 2, 3, 4, 5, 0 };
    
        int index = currentStage % 30;
        if (index == 0) index = 30; // กรณีหารลงตัวพอดี (ยกเว้นบอส)
    
        int elementIndex = pattern[index];

        // เลือกมอนสเตอร์จาก Set ของธาตุนั้น
        if (elementIndex < elementalGroups.Length)
        {
            var group = elementalGroups[elementIndex];
            if (group.monsterSets != null && currentSet < group.monsterSets.Length)
            {
                var monsterSet = group.monsterSets[currentSet];
                if (monsterSet.enemyIndices != null && monsterSet.enemyIndices.Length > 0)
                {
                    return monsterSet.enemyIndices[random.Next(monsterSet.enemyIndices.Length)];
                }
            }
        }

        Debug.LogWarning($"No enemies found for stage {currentStage}, element {elementIndex}, set {currentSet}");
        return 0;
    }

    // ฟังก์ชันตรวจสอบความถูกต้อง
    private void OnValidate()
    {
        if (elementalGroups != null)
        {
            for (int i = 0; i < elementalGroups.Length; i++)
            {
                if (string.IsNullOrEmpty(elementalGroups[i].elementName))
                {
                    elementalGroups[i].elementName = $"Element {i + 1}";
                }

                if (elementalGroups[i].monsterSets != null)
                {
                    for (int j = 0; j < elementalGroups[i].monsterSets.Length; j++)
                    {
                        if (string.IsNullOrEmpty(elementalGroups[i].monsterSets[j].setName))
                        {
                            elementalGroups[i].monsterSets[j].setName = $"Set {j + 1}";
                        }
                    }
                }
            }
        }

        if (bossSets != null)
        {
            for (int i = 0; i < bossSets.Length; i++)
            {
                if (string.IsNullOrEmpty(bossSets[i].setName))
                {
                    bossSets[i].setName = $"Boss Set {i + 1}";
                }
            }
        }
    }
    private GameObject CreateEnemy(Vector3 spawnPos)
    {
        UpdateStageSettings();
        int enemyIndex = GetEnemyIndexForStage();

        if (enemyIndex >= 0 && enemyIndex < enemyPrefab.Length)
        {
            GameObject enemy = Instantiate(enemyPrefab[enemyIndex], spawnPos, Quaternion.identity);
    
            if (debug)
            {
                // แก้การคำนวณ pattern และชื่อธาตุ
                int pattern = ((currentStage - 1) % 6) + 1;
                string elementName = "Unknown";
            
                // แปลง pattern เป็นชื่อธาตุ
                switch (pattern)
                {
                    case 1: elementName = "Fire"; break;
                    case 2: elementName = "Wind"; break;
                    case 3: elementName = "Earth"; break;
                    case 4: elementName = "Water"; break;
                    case 5: elementName = "Light"; break;
                    case 6: elementName = "Dark"; break;
                }

                string setName = "Regular Set " + currentSet;
        
                if (currentStage % 5 == 0)
                {
                    int bossSetIndex = (currentStage / 5 - 1) % bossSets.Length;
                    setName = "Boss Set " + bossSets[bossSetIndex].setName;
                }
        
                Debug.Log($"Stage {currentStage}: Spawned {elementName} enemy from {setName} (Index: {enemyIndex})");
            }
    
            return enemy;
        }

        Debug.LogError($"Invalid enemy index: {enemyIndex}");
        return null;
    }
    
    [Header("Debug")]
    [SerializeField] private bool debug = false; // เพิ่มตัวเลือกสำหรับ debug
    
    [SerializeField] private StageSelectionManager stageSelectionManager;
    
    [Header("Stage Settings")]
    public StageData _StageData;
    public int currentStage = 1;
    public int MapIndex;
    
    [Header("Spawn Settings")]
    // public GameObject[] enemyPrefab;
    public float spawnInterval = 2f;
    public float spawnRadius = 10f;
    public int maxEnemies = 10;
    public List<Transform> spawnPoints;
    
    [Header("Enemy Tracking")]
    public int enemiesSpawned = 0;
    public int enemiesDefeated = 0;
    
    [Header("UI Elements")]
    // public GameObject NextButton;
    public GameObject WinUI;
    public GameObject BossUI;
    
    private TestTeleportPlayer _teleportPlayer;
    private StageManager _stageManager;
    private bool isClearing = false;
    [SerializeField] private VolumeProfileChanger volumeProfileChanger;

    [SerializeField] private int maxEnemiesForStage;
    private void Start()
    {
        InitializeComponents();
        StartSpawning();
    }

    private void InitializeComponents()
    {
        _teleportPlayer = FindObjectOfType<TestTeleportPlayer>();
        _stageManager = FindObjectOfType<StageManager>();
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.P))
        // {
        //     ClearAllEnemies();
        // }
    }

    public void StartSpawning()
    {
        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
    }

    public void SpawnEnemy()
    {
        if (enemiesSpawned >= maxEnemies)
        {
            CancelInvoke(nameof(SpawnEnemy));
            return;
        }

        Vector3 spawnPos = GetSpawnPosition();
        GameObject enemy = CreateEnemy(spawnPos);
        enemy.AddComponent<EnemyDefeatedNotifier>().spawner = this;
        enemiesSpawned++;
    }

    private Vector3 GetSpawnPosition()
    {
        if (spawnPoints.Count > 0)
        {
            return spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        }
        
        Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
        return new Vector3(randomPos.x, 0, randomPos.y) + transform.position;
    }

    

    private void UpdateStageSettings()
    {
        currentSet = GetCurrentSet();
    
        // ถ้าเป็นด่านที่หาร 5 ลงตัว จะเป็นด่านบอส
        if (currentStage % 5 == 0)
        {
            MapIndex = 5; // บอส
            maxEnemies = 1; // บอสมี 1 ตัว
            return;
        }

        // สร้าง pattern แบบตายตัวตามลำดับที่ต้องการ
        // index 0 ไม่มีความหมาย เพราะเราจะใช้ currentStage % 30 เพื่อหา index
        int[] pattern = new int[] { 0, 1, 2, 3, 4, 0, 6, 7, 1, 2, 0, 3, 4, 6, 7, 0, 1, 2, 3, 4, 0, 6, 7, 1, 2, 0, 3, 4, 6, 7, 0 };
    
        int index = currentStage % 30;
        if (index == 0) index = 30; // กรณีหารลงตัวพอดี (ยกเว้นบอส)
    
        MapIndex = pattern[index];
    
        maxEnemies = maxEnemiesForStage; // ด่านปกติมีศัตรูตามที่กำหนด
    }

    public void ClearAllEnemies()
    {
        isClearing = true;
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in allEnemies)
        {
            Destroy(enemy);
        }
        ResetEnemyCount();
        StartCoroutine(WaitAndResetClear());
    }

    private void ResetEnemyCount()
    {
        enemiesSpawned = 0;
        enemiesDefeated = 0;
    }

    private IEnumerator WaitAndResetClear()
    {
        yield return new WaitForSeconds(5f);
        isClearing = false;
    }

    public void EnemyDefeated()
    {
        if (isClearing) return;
        
        enemiesDefeated++;
        if (enemiesDefeated >= maxEnemies)
        {
            HandleStageCompletion();
        }
    }

    private void HandleStageCompletion()
    {
        BeforeBossStageCheck();
        if (currentStage < stageSelectionManager.currentStage)
        {
            ResetEnemyCount();
            StartSpawning();
        }
        
        if (currentStage >= stageSelectionManager.currentStage)
        {
            CompleteCurrentStage();
        }
    }

    private void BeforeBossStageCheck()
    {
        if ((currentStage - 4) % 5 == 0)
        {
            BossUI.SetActive(true);
        }
    }

    private void CompleteCurrentStage()
    {
        stageSelectionManager.CompleteStage(currentStage);
        BossUI.SetActive(false);
        WinUI.SetActive(true);
    }

    public void TeleportPlayer()
    {
        if (MapIndex == 6)
        {
            Vector3 newpos6 = new Vector3(2.9f, 2.1f, 42.4f);
            _teleportPlayer.TeleportPlayer(newpos6);
        }
       
        Vector3 newpos = new Vector3(-8, 2.1f, -6);
        _teleportPlayer.TeleportPlayer(newpos);
    }
    public void GotoBoss()
    {
        TeleportPlayer();
        
        ClearAllEnemies();
        maxEnemies = 1;
        currentStage += 1;
        UpdateStageSettings();
        _stageManager.ChangeMap(MapIndex);
        StartSpawning();
        BossUI.SetActive(false);
    }

    public void NextStage()
    { 
        ClearAllEnemies();
        TeleportPlayer();
        
        currentStage += 1;
        UpdateStageSettings();
        _stageManager.ChangeMap(MapIndex);
        volumeProfileChanger.ChangeSceneSetup(MapIndex);
        ResetEnemyCount();
        StartSpawning();
    }

    public void SetStage(int stageIndex)
    {
        MapIndex = stageIndex;
       
        currentStage = stageIndex;
        UpdateStageSettings();
        TeleportPlayer();
        ClearAllEnemies();
        
        _stageManager.ChangeMap(MapIndex);
        volumeProfileChanger.ChangeSceneSetup(MapIndex);
        ResetEnemyCount();
        StartSpawning();
    }

    public int GetStage() => currentStage;

    public void ResetEnemies()
    {
        enemiesDefeated = 0;
//        NextButton.SetActive(false);
    }
}

public class EnemyDefeatedNotifier : MonoBehaviour
{
    public EnemySpawner spawner;

    private void OnDestroy()
    {
        spawner?.EnemyDefeated();
    }
}