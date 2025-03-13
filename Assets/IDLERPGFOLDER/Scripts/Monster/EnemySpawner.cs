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

// เพิ่ม enum สำหรับประเภทบอส
public enum BossType
{
    MiniBoss,
    MainBoss
}

// เพิ่ม enum สำหรับธาตุต่างๆ (เรียงตามลำดับที่กำหนด)
public enum BossElementType
{
    Earth,  // ดิน
    Water,  // น้ำ
    Wind,   // ลม
    Fire,   // ไฟ
    Dark,   // มืด
    Light   // แสง
}

// ปรับปรุง BossSet เพื่อเพิ่มข้อมูลแมพและธาตุ
[Serializable]
public class BossSet
{
    public string setName;
    public int[] bossIndices; // บอสในชุดนี้
    public int mapIndex; // แมพที่ใช้สำหรับบอสชุดนี้
    public BossType bossType; // ประเภทบอส (มินิบอส หรือ บอสใหญ่)
    public BossElementType elementType; // ธาตุของบอส
}

[Serializable]
public class StageSpawnPointSet
{
    public string setName;
    public int stageIndex; // ด่านที่จะใช้จุดเกิดนี้
    public List<Transform> spawnPoints; // จุดเกิดสำหรับด่านนี้
}

[Serializable]
public class StageMapRange
{
    public string rangeName; // ชื่อของช่วงด่าน
    public int startStage; // ด่านเริ่มต้นของช่วง
    public int endStage;   // ด่านสิ้นสุดของช่วง
    public int mapIndex;   // ดัชนีแมพที่จะใช้สำหรับช่วงด่านนี้
}

public class EnemySpawner : MonoBehaviour
{
    [Header("Element Groups")]
    [SerializeField] private ElementalGroup[] elementalGroups;

    [Header("Boss Sets")]
    [SerializeField] private BossSet[] bossSets;
    [SerializeField] private int defaultBossMapIndex = 5; // แมพบอสเริ่มต้นในกรณีที่ไม่พบบอสเซ็ต
    
    [Header("Stage Spawn Points")]
    [SerializeField] private StageSpawnPointSet[] stageSpawnPointSets;
    [SerializeField] private List<Transform> defaultSpawnPoints; // จุดเกิดเริ่มต้นถ้าไม่มีจุดเฉพาะ

    [Header("Map Selection")]
    [SerializeField] private StageMapRange[] stageMapRanges;
    [SerializeField] private int defaultMapIndex = 1; // แมพเริ่มต้นถ้าไม่มีการกำหนด range

    [Header("Difficulty Settings")]
    [SerializeField] private int stagesPerDifficultyIncrease = 10; // จำนวนด่านก่อนเพิ่มความยาก
    [SerializeField] private bool randomizeSetSelection = false; // สุ่มเลือกชุดหรือไม่
    [SerializeField] private bool useBossPattern = true; // ใช้รูปแบบมินิบอส -> บอสใหญ่ หรือไม่

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefab;

    private System.Random random = new System.Random();
    private int currentSet;
    
    private int GetCurrentSet()
    {
        if (randomizeSetSelection)
        {
            return random.Next(GetMaxAvailableSets());
        }
        
        return Mathf.Min((currentStage - 1) / stagesPerDifficultyIncrease, GetMaxAvailableSets() - 1);
    }

    private int GetMaxAvailableSets()
    {
        int maxSets = 0;
        foreach (var group in elementalGroups)
        {
            maxSets = Mathf.Max(maxSets, group.monsterSets?.Length ?? 0);
        }
        return maxSets;
    }

    // เพิ่มฟังก์ชันเพื่อหา BossSet ที่เหมาะสมสำหรับด่านบอส
    private BossSet GetBossSetForStage(int stage)
    {
        if (bossSets == null || bossSets.Length == 0)
        {
            Debug.LogWarning("No boss sets defined!");
            return null;
        }

        // กำหนดรูปแบบของบอส: มินิบอส -> บอสใหญ่ -> มินิบอส -> บอสใหญ่...
        BossType bossTypeForStage = (stage % 10 == 5) ? BossType.MiniBoss : BossType.MainBoss;

        // กำหนดรูปแบบของธาตุตามด่าน
        // ด่าน 5, 10: ธาตุดิน; ด่าน 15, 20: ธาตุน้ำ; ด่าน 25, 30: ธาตุลม; ด่าน 35, 40: ธาตุไฟ; ฯลฯ
        BossElementType elementForStage;
        int elementCycle = ((stage - 5) / 10) % 6; // วนรอบทุก 60 ด่าน (6 ธาตุ × 10 ด่าน)
        
        switch (elementCycle)
        {
            case 0: elementForStage = BossElementType.Earth; break; // ดิน
            case 1: elementForStage = BossElementType.Water; break; // น้ำ
            case 2: elementForStage = BossElementType.Wind; break;  // ลม
            case 3: elementForStage = BossElementType.Fire; break;  // ไฟ
            case 4: elementForStage = BossElementType.Dark; break;  // มืด
            case 5: elementForStage = BossElementType.Light; break; // แสง
            default: elementForStage = BossElementType.Earth; break;
        }

        // ค้นหา BossSet ที่ตรงกับประเภทและธาตุที่ต้องการ
        foreach (var bossSet in bossSets)
        {
            if (bossSet.bossType == bossTypeForStage && bossSet.elementType == elementForStage)
            {
                return bossSet;
            }
        }

        // ถ้าไม่พบ BossSet ที่ตรงกับทั้งประเภทและธาตุ ให้ค้นหาเฉพาะประเภท
        foreach (var bossSet in bossSets)
        {
            if (bossSet.bossType == bossTypeForStage)
            {
                return bossSet;
            }
        }

        // ถ้ายังไม่พบอีก ให้ใช้ BossSet แรก
        return bossSets[0];
    }

    private int GetEnemyIndexForStage()
    {
        currentSet = GetCurrentSet();

        // ตรวจสอบด่านบอส
        if (currentStage % 5 == 0)
        {
            BossSet bossSet = GetBossSetForStage(currentStage);
            if (bossSet != null && bossSet.bossIndices != null && bossSet.bossIndices.Length > 0)
            {
                return bossSet.bossIndices[random.Next(bossSet.bossIndices.Length)];
            }
            else
            {
                Debug.LogWarning($"No boss found for stage {currentStage}, using first enemy");
                return 0;
            }
        }

        // ด่านปกติ (ไม่ใช่บอส)
        int elementIndex = (currentStage - 1) % elementalGroups.Length;

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
                    string typeStr = bossSets[i].bossType.ToString();
                    string elementStr = bossSets[i].elementType.ToString();
                    bossSets[i].setName = $"{elementStr} {typeStr} Set";
                }
            }
        }
        
        if (stageSpawnPointSets != null)
        {
            for (int i = 0; i < stageSpawnPointSets.Length; i++)
            {
                if (string.IsNullOrEmpty(stageSpawnPointSets[i].setName))
                {
                    stageSpawnPointSets[i].setName = $"Spawn Set for Stage {stageSpawnPointSets[i].stageIndex}";
                }
            }
        }

        if (stageMapRanges != null)
        {
            for (int i = 0; i < stageMapRanges.Length; i++)
            {
                if (string.IsNullOrEmpty(stageMapRanges[i].rangeName))
                {
                    stageMapRanges[i].rangeName = $"Map Range {i + 1} (Stage {stageMapRanges[i].startStage}-{stageMapRanges[i].endStage})";
                }
                
                if (stageMapRanges[i].startStage > stageMapRanges[i].endStage)
                {
                    Debug.LogWarning($"Stage range {stageMapRanges[i].rangeName} has startStage > endStage. Auto-fixing.");
                    int temp = stageMapRanges[i].startStage;
                    stageMapRanges[i].startStage = stageMapRanges[i].endStage;
                    stageMapRanges[i].endStage = temp;
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
                string enemyInfo;
                
                if (currentStage % 5 == 0)
                {
                    BossSet bossSet = GetBossSetForStage(currentStage);
                    string bossType = bossSet?.bossType.ToString() ?? "Unknown";
                    string elementType = bossSet?.elementType.ToString() ?? "Unknown";
                    enemyInfo = $"Stage {currentStage}: Spawned {elementType} {bossType} (Index: {enemyIndex}) on Map {MapIndex}";
                }
                else
                {
                    int elementIndex = (currentStage - 1) % elementalGroups.Length;
                    string elementName = elementIndex < elementalGroups.Length ? elementalGroups[elementIndex].elementName : "Unknown";
                    enemyInfo = $"Stage {currentStage}: Spawned {elementName} enemy from Set {currentSet} (Index: {enemyIndex}) on Map {MapIndex}";
                }
                
                Debug.Log(enemyInfo);
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
    public int currentStage = 1;
    public int MapIndex;
    
    [Header("Spawn Settings")]
    public float spawnInterval = 2f;
    public float spawnRadius = 10f;
    public int maxEnemies = 10;
    
    [Header("Enemy Tracking")]
    public int enemiesSpawned = 0;
    public int enemiesDefeated = 0;
    
    [Header("UI Elements")]
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
        // ตรวจสอบว่ามีจุดเกิดเฉพาะสำหรับด่านนี้หรือไม่
        if (stageSpawnPointSets != null)
        {
            foreach (var spawnSet in stageSpawnPointSets)
            {
                if (spawnSet.stageIndex == currentStage && spawnSet.spawnPoints != null && spawnSet.spawnPoints.Count > 0)
                {
                    Transform selectedPoint = spawnSet.spawnPoints[Random.Range(0, spawnSet.spawnPoints.Count)];
                    if (selectedPoint != null)
                    {
                        if (debug)
                        {
                            Debug.Log($"Using custom spawn point for stage {currentStage} from set {spawnSet.setName}");
                        }
                        return selectedPoint.position;
                    }
                }
            }
        }
        
        // ตรวจสอบตาม MapIndex ถ้าไม่มีการตั้งค่าเฉพาะสำหรับ Stage
        if (stageSpawnPointSets != null)
        {
            foreach (var spawnSet in stageSpawnPointSets)
            {
                // ใช้ stageIndex ติดลบเพื่อระบุว่าเป็นการอ้างอิงถึง MapIndex แทน Stage
                if (spawnSet.stageIndex == -MapIndex && spawnSet.spawnPoints != null && spawnSet.spawnPoints.Count > 0)
                {
                    Transform selectedPoint = spawnSet.spawnPoints[Random.Range(0, spawnSet.spawnPoints.Count)];
                    if (selectedPoint != null)
                    {
                        if (debug)
                        {
                            Debug.Log($"Using custom spawn point for MapIndex {MapIndex} from set {spawnSet.setName}");
                        }
                        return selectedPoint.position;
                    }
                }
            }
        }
        
        // ถ้าเป็นบอส ใช้จุดเกิดพิเศษ
        if (currentStage % 5 == 0)
        {
            // ตรวจสอบว่ามีจุดเกิดเฉพาะสำหรับบอสหรือไม่
            foreach (var spawnSet in stageSpawnPointSets)
            {
                if (spawnSet.stageIndex == -999 && spawnSet.spawnPoints != null && spawnSet.spawnPoints.Count > 0) // ใช้ค่าพิเศษ -999 สำหรับบอส
                {
                    Transform selectedPoint = spawnSet.spawnPoints[Random.Range(0, spawnSet.spawnPoints.Count)];
                    if (selectedPoint != null)
                    {
                        if (debug)
                        {
                            Debug.Log($"Using boss spawn point for stage {currentStage}");
                        }
                        return selectedPoint.position;
                    }
                }
            }
        }
        
        // ถ้าไม่มีจุดเกิดเฉพาะ ใช้จุดเกิดเริ่มต้น
        if (defaultSpawnPoints != null && defaultSpawnPoints.Count > 0)
        {
            return defaultSpawnPoints[Random.Range(0, defaultSpawnPoints.Count)].position;
        }
        
        // ถ้าไม่มีจุดเกิดเริ่มต้น ใช้การสุ่มรอบตำแหน่งปัจจุบัน
        Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
        return new Vector3(randomPos.x, 0, randomPos.y) + transform.position;
    }

    // ปรับปรุงฟังก์ชัน UpdateStageSettings เพื่อใช้แมพตาม BossSet
    private void UpdateStageSettings()
    {
        currentSet = GetCurrentSet();
    
        // ถ้าเป็นด่านที่หาร 5 ลงตัว จะเป็นด่านบอส
        if (currentStage % 5 == 0)
        {
            BossSet bossSet = GetBossSetForStage(currentStage);
            if (bossSet != null)
            {
                MapIndex = bossSet.mapIndex; // ใช้แมพตามที่กำหนดใน BossSet
                if (debug)
                {
                    Debug.Log($"Using map {MapIndex} for {bossSet.elementType} {bossSet.bossType} at stage {currentStage}");
                }
            }
            else
            {
                MapIndex = defaultBossMapIndex; // ใช้แมพบอสเริ่มต้น
            }
            
            maxEnemies = 1; // บอสมี 1 ตัว
            return;
        }

        // หาแมพที่ตรงกับช่วงด่านปัจจุบัน
        MapIndex = GetMapIndexForCurrentStage();
    
        maxEnemies = maxEnemiesForStage; // ด่านปกติมีศัตรูตามที่กำหนด
    }

    private int GetMapIndexForCurrentStage()
    {
        if (stageMapRanges != null && stageMapRanges.Length > 0)
        {
            foreach (var range in stageMapRanges)
            {
                if (currentStage >= range.startStage && currentStage <= range.endStage)
                {
                    return range.mapIndex;
                }
            }
        }
        
        return defaultMapIndex;
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
        // ตรวจสอบแมพพิเศษ
        if (MapIndex == 6)
        {
            Vector3 newpos6 = new Vector3(2.9f, 2.1f, 42.4f);
            _teleportPlayer.TeleportPlayer(newpos6);
            return;
        }
       
        Vector3 newpos = new Vector3(-8, 2.1f, -6);
        _teleportPlayer.TeleportPlayer(newpos);
    }

    public void GotoBoss()
    {
        ClearAllEnemies();
        TeleportPlayer();
        
        currentStage += 1;
        UpdateStageSettings();
        _stageManager.ChangeMap(MapIndex);
        volumeProfileChanger.ChangeSceneSetup(MapIndex);
        ResetEnemyCount();
        StartSpawning();
        BossUI.SetActive(false);
    }

    public void NextStage()
    { 
        ClearAllEnemies();
        if (currentStage == 5)
        {
            Vector3 newpos6 = new Vector3(2.9f, 2.1f, 42.4f);
            _teleportPlayer.TeleportPlayer(newpos6);
        }
        else
        {
            TeleportPlayer();
        }
    
        currentStage += 1;
        UpdateStageSettings();
        _stageManager.ChangeMap(MapIndex);
        volumeProfileChanger.ChangeSceneSetup(MapIndex);
        ResetEnemyCount();
        StartSpawning();
        BossUI.SetActive(false);
    }

    public void SetStage(int stageIndex)
    {
        currentStage = stageIndex;
        UpdateStageSettings(); // อัพเดทค่า MapIndex ตามด่านที่เลือก
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