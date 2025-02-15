using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private StageSelectionManager stageSelectionManager;
    
    [Header("Stage Settings")]
    public StageData _StageData;
    public int currentStage = 1;
    public int MapIndex;
    
    [Header("Spawn Settings")]
    public GameObject[] enemyPrefab;
    public float spawnInterval = 2f;
    public float spawnRadius = 10f;
    public int maxEnemies = 10;
    public List<Transform> spawnPoints;
    
    [Header("Enemy Tracking")]
    public int enemiesSpawned = 0;
    public int enemiesDefeated = 0;
    
    [Header("UI Elements")]
    public GameObject NextButton;
    public GameObject WinUI;
    public GameObject BossUI;
    
    private TestTeleportPlayer _teleportPlayer;
    private StageManager _stageManager;
    private bool isClearing = false;

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
        if (Input.GetKeyDown(KeyCode.P))
        {
            ClearAllEnemies();
        }
    }

    private void StartSpawning()
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

    private GameObject CreateEnemy(Vector3 spawnPos)
    {
        UpdateStageSettings();
        int enemyIndex = GetEnemyIndexForStage();
        return Instantiate(enemyPrefab[enemyIndex], spawnPos, Quaternion.identity);
    }

    private int GetEnemyIndexForStage()
    {
        int stageModulo = currentStage % 5;
        if (stageModulo == 0) return 4; // Boss stages (5, 10, etc.)
        if (stageModulo == 1) return 0;
        if (stageModulo == 2) return 1;
        if (stageModulo == 3) return 5;
        return 2; // stageModulo == 4 or default case
    }

    private void UpdateStageSettings()
    {
        int stageModulo = currentStage % 5;
        MapIndex = (stageModulo == 0) ? 5 : stageModulo;
        maxEnemies = (stageModulo == 0) ? 1 : 5;
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
        if (currentStage <= stageSelectionManager.currentStage)
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
        Vector3 newpos = new Vector3(-8, 2.1f, -6);
        _teleportPlayer.TeleportPlayer(newpos);
    }
    public void GotoBoss()
    {
        TeleportPlayer();
        
        ClearAllEnemies();
        maxEnemies = 1;
        currentStage += 1;
        
        _stageManager.ChangeMap(MapIndex);
        StartSpawning();
        BossUI.SetActive(false);
    }

    public void NextStage()
    { 
        TeleportPlayer();
        
        currentStage += 1;
        UpdateStageSettings();
        _stageManager.ChangeMap(MapIndex);
        ResetEnemyCount();
        StartSpawning();
    }

    public void SetStage(int stageIndex)
    {
        TeleportPlayer();
        
        ClearAllEnemies();
        currentStage = stageIndex;
        _stageManager.ChangeMap(stageIndex);
        ResetEnemyCount();
        StartSpawning();
    }

    public int GetStage() => currentStage;

    public void ResetEnemies()
    {
        enemiesDefeated = 0;
        NextButton.SetActive(false);
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