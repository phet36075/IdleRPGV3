using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

#region Data Structures
[Serializable]
public class MonsterSet
{
    public string setName;
    public int[] enemyIndices;
}

[Serializable]
public class ElementalGroup
{
    public string elementName;
    public MonsterSet[] monsterSets;
}

public enum BossType { MiniBoss, MainBoss }

public enum BossElementType { Earth, Water, Wind, Fire, Dark, Light }

[Serializable]
public class BossSet
{
    public string setName;
    public int[] bossIndices;
    public int mapIndex;
    public BossType bossType;
    public BossElementType elementType;
}

[Serializable]
public class StageSpawnPointSet
{
    public string setName;
    public int stageIndex; // ใช้ค่าติดลบเพื่อระบุ MapIndex แทน Stage, -999 สำหรับบอส
    public List<Transform> spawnPoints;
}

[Serializable]
public class StageMapRange
{
    public string rangeName;
    public int startStage;
    public int endStage;
    public int mapIndex;
}

[Serializable]
public class MapTeleportPosition
{
    public string positionName;
    public int mapIndex;
    public Vector3 position;
}
#endregion

public class EnemySpawner : MonoBehaviour
{
    #region Inspector Fields
    [Header("Element Groups")]
    [SerializeField] private ElementalGroup[] elementalGroups;

    [Header("Boss Sets")]
    [SerializeField] private BossSet[] bossSets;
    [SerializeField] private int defaultBossMapIndex = 5;
    
    [Header("Spawn Points")]
    [SerializeField] private StageSpawnPointSet[] stageSpawnPointSets;
    [SerializeField] private List<Transform> defaultSpawnPoints;

    [Header("Teleport Positions")]
    [SerializeField] private MapTeleportPosition[] mapTeleportPositions;
    [SerializeField] private Vector3 defaultTeleportPosition = new Vector3(-8, 2.1f, -6);
    
    [Header("Map Selection")]
    [SerializeField] private StageMapRange[] stageMapRanges;
    [SerializeField] private int defaultMapIndex = 1;

    [Header("Difficulty Settings")]
    [SerializeField] private int stagesPerDifficultyIncrease = 10;
    [SerializeField] private bool randomizeSetSelection = false;
    [SerializeField] private bool useBossPattern = true;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefab;
    
    [Header("Stage Settings")]
    public int currentStage = 1;
    public int MapIndex;
    
    [Header("Spawn Settings")]
    public float spawnInterval = 2f;
    public float spawnRadius = 10f;
    public int maxEnemies = 10;
    [SerializeField] private int maxEnemiesForStage;
    
    [Header("Enemy Tracking")]
    public int enemiesSpawned = 0;
    public int enemiesDefeated = 0;
    
    [Header("UI Elements")]
    public GameObject WinUI;
    public GameObject BossUI;

    [Header("Dependencies")]
    [SerializeField] private StageSelectionManager stageSelectionManager;
    [SerializeField] private VolumeProfileChanger volumeProfileChanger;
    
    [Header("Debug")]
    [SerializeField] private bool debug = false;
    #endregion

    #region Private Fields
    private System.Random random = new System.Random();
    private int currentSet;
    private TestTeleportPlayer _teleportPlayer;
    private StageManager _stageManager;
    private bool isClearing = false;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        InitializeComponents();
        StartSpawning();
    }

    private void OnValidate()
    {
        ValidateElementalGroups();
        ValidateBossSets();
        ValidateSpawnPointSets();
        ValidateMapRanges();
        ValidateTeleportPositions();
    }
    #endregion

    #region Initialization
    private void InitializeComponents()
    {
        _teleportPlayer = FindObjectOfType<TestTeleportPlayer>();
        _stageManager = FindObjectOfType<StageManager>();
    }
    #endregion

    #region Validation
    private void ValidateElementalGroups()
    {
        if (elementalGroups == null) return;
        
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

    private void ValidateBossSets()
    {
        if (bossSets == null) return;
        
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

    private void ValidateSpawnPointSets()
    {
        if (stageSpawnPointSets == null) return;
        
        for (int i = 0; i < stageSpawnPointSets.Length; i++)
        {
            if (string.IsNullOrEmpty(stageSpawnPointSets[i].setName))
            {
                stageSpawnPointSets[i].setName = $"Spawn Set for Stage {stageSpawnPointSets[i].stageIndex}";
            }
        }
    }

    private void ValidateMapRanges()
    {
        if (stageMapRanges == null) return;
        
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
    
    // Add validation for teleport positions
    private void ValidateTeleportPositions()
    {
        if (mapTeleportPositions == null) return;
        
        for (int i = 0; i < mapTeleportPositions.Length; i++)
        {
            if (string.IsNullOrEmpty(mapTeleportPositions[i].positionName))
            {
                mapTeleportPositions[i].positionName = $"Map {mapTeleportPositions[i].mapIndex} Position";
            }
        }
    }
    
    #endregion

    #region Difficulty Management
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
    #endregion

    #region Boss Management
    private BossSet GetBossSetForStage(int stage)
    {
        if (bossSets == null || bossSets.Length == 0)
        {
            Debug.LogWarning("No boss sets defined!");
            return null;
        }

        // กำหนดรูปแบบของบอส: มินิบอส -> บอสใหญ่
        BossType bossTypeForStage = (stage % 10 == 5) ? BossType.MiniBoss : BossType.MainBoss;

        // กำหนดธาตุตามด่าน (วนรอบทุก 60 ด่าน)
        int elementCycle = ((stage - 5) / 10) % 6;
        BossElementType elementForStage = GetElementTypeForCycle(elementCycle);

        // ค้นหา BossSet ที่ตรงกับประเภทและธาตุ
        foreach (var bossSet in bossSets)
        {
            if (bossSet.bossType == bossTypeForStage && bossSet.elementType == elementForStage)
            {
                return bossSet;
            }
        }

        // ถ้าไม่พบ ค้นหาเฉพาะประเภท
        foreach (var bossSet in bossSets)
        {
            if (bossSet.bossType == bossTypeForStage)
            {
                return bossSet;
            }
        }

        // ถ้ายังไม่พบ ใช้ BossSet แรก
        return bossSets[0];
    }

    private BossElementType GetElementTypeForCycle(int elementCycle)
    {
        switch (elementCycle)
        {
            case 0: return BossElementType.Earth;
            case 1: return BossElementType.Water;
            case 2: return BossElementType.Wind;
            case 3: return BossElementType.Fire;
            case 4: return BossElementType.Dark;
            case 5: return BossElementType.Light;
            default: return BossElementType.Earth;
        }
    }
    #endregion

    #region Enemy Selection
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

        // ด่านปกติ
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
    #endregion

    #region Spawning
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

    private GameObject CreateEnemy(Vector3 spawnPos)
    {
        UpdateStageSettings();
        int enemyIndex = GetEnemyIndexForStage();

        if (enemyIndex >= 0 && enemyIndex < enemyPrefab.Length)
        {
            GameObject enemy = Instantiate(enemyPrefab[enemyIndex], spawnPos, Quaternion.identity);
    
            if (debug)
            {
                LogSpawnInfo(enemyIndex);
            }
    
            return enemy;
        }

        Debug.LogError($"Invalid enemy index: {enemyIndex}");
        return null;
    }

    private void LogSpawnInfo(int enemyIndex)
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

    private Vector3 GetSpawnPosition()
    {
        // ตรวจสอบจุดเกิดเฉพาะสำหรับด่าน
        foreach (var spawnSet in stageSpawnPointSets ?? new StageSpawnPointSet[0])
        {
            // จุดเกิดเฉพาะสำหรับด่าน
            if (spawnSet.stageIndex == currentStage && HasValidSpawnPoints(spawnSet))
            {
                return GetRandomSpawnPoint(spawnSet, $"custom spawn point for stage {currentStage}");
            }
            
            // จุดเกิดเฉพาะสำหรับแมพ
            if (spawnSet.stageIndex == -MapIndex && HasValidSpawnPoints(spawnSet))
            {
                return GetRandomSpawnPoint(spawnSet, $"custom spawn point for MapIndex {MapIndex}");
            }
            
            // จุดเกิดเฉพาะสำหรับบอส
            if (currentStage % 5 == 0 && spawnSet.stageIndex == -999 && HasValidSpawnPoints(spawnSet))
            {
                return GetRandomSpawnPoint(spawnSet, $"boss spawn point for stage {currentStage}");
            }
        }
        
        // ใช้จุดเกิดเริ่มต้น
        if (defaultSpawnPoints != null && defaultSpawnPoints.Count > 0)
        {
            return defaultSpawnPoints[Random.Range(0, defaultSpawnPoints.Count)].position;
        }
        
        // สุ่มรอบตำแหน่งปัจจุบัน
        Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
        return new Vector3(randomPos.x, 0, randomPos.y) + transform.position;
    }

    private bool HasValidSpawnPoints(StageSpawnPointSet spawnSet)
    {
        return spawnSet.spawnPoints != null && spawnSet.spawnPoints.Count > 0;
    }

    private Vector3 GetRandomSpawnPoint(StageSpawnPointSet spawnSet, string debugMessage)
    {
        Transform selectedPoint = spawnSet.spawnPoints[Random.Range(0, spawnSet.spawnPoints.Count)];
        if (selectedPoint != null)
        {
            if (debug)
            {
                Debug.Log($"Using {debugMessage} from set {spawnSet.setName}");
            }
            return selectedPoint.position;
        }
        
        // Fallback to default position if something went wrong
        return transform.position;
    }
    #endregion

    #region Stage Management
    private void UpdateStageSettings()
    {
        currentSet = GetCurrentSet();
    
        // ตรวจสอบด่านบอส
        if (currentStage % 5 == 0)
        {
            SetBossStageSettings();
            return;
        }

        // หาแมพที่ตรงกับช่วงด่านปัจจุบัน
        MapIndex = GetMapIndexForCurrentStage();
        maxEnemies = maxEnemiesForStage;
    }

    private void SetBossStageSettings()
    {
        BossSet bossSet = GetBossSetForStage(currentStage);
        if (bossSet != null)
        {
            MapIndex = bossSet.mapIndex;
            if (debug)
            {
                Debug.Log($"Using map {MapIndex} for {bossSet.elementType} {bossSet.bossType} at stage {currentStage}");
            }
        }
        else
        {
            MapIndex = defaultBossMapIndex;
        }
        
        maxEnemies = 1; // บอสมี 1 ตัว
    }

    private int GetMapIndexForCurrentStage()
    {
        if (stageMapRanges != null)
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
    #endregion

    #region Player Movement
    public void TeleportPlayer()
    {
        // Find the teleport position for the current map index
        Vector3 teleportPosition = GetTeleportPositionForMap(MapIndex);
        _teleportPlayer.TeleportPlayer(teleportPosition);
    }
    
    private Vector3 GetTeleportPositionForMap(int mapIndex)
    {
        // Check if we have a specific position defined for this map
        if (mapTeleportPositions != null)
        {
            foreach (var teleportPos in mapTeleportPositions)
            {
                if (teleportPos.mapIndex == mapIndex)
                {
                    if (debug)
                    {
                        Debug.Log($"Using teleport position for map {mapIndex}: {teleportPos.position}");
                    }
                    return teleportPos.position;
                }
            }
        }
        
        // If no position found for the current map, use the default
        if (debug)
        {
            Debug.Log($"No teleport position found for map {mapIndex}, using default position");
        }
        return defaultTeleportPosition;
    }
    
    #endregion

    #region Public Methods
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

    // Replace the existing NextStage method to use the new TeleportPlayer
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
        BossUI.SetActive(false);
    }

    // Replace the existing GotoBoss method to use the new TeleportPlayer
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

    public void SetStage(int stageIndex)
    {
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
    }
    #endregion

    #region Helper Methods
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
    #endregion
}

public class EnemyDefeatedNotifier : MonoBehaviour
{
    public EnemySpawner spawner;

    private void OnDestroy()
    {
        spawner?.EnemyDefeated();
    }
}