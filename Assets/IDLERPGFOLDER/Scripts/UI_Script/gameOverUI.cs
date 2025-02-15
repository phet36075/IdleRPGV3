
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public float timeRemaining = 10;
    public TextMeshProUGUI timeText;
    public Animator playerAnim;
    public Animator allyAnim;
    private PlayerManager _playerManager;
    private EnemySpawner _enemySpawner;
    public string stageName;
    private TestTeleportPlayer _teleportPlayer;
  
    // Start is called before the first frame update
    void Start()
    {
        _teleportPlayer = FindObjectOfType<TestTeleportPlayer>();
        _playerManager = FindObjectOfType<PlayerManager>();
        _enemySpawner = FindObjectOfType<EnemySpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining -= Time.deltaTime;
        timeText.text = timeRemaining.ToString("#");

        if (timeRemaining <= 0)
        {
            RetryStage();
        }
    }

  private void RetryStage()
  {
      
      _playerManager.currentHealth = _playerManager.playerData.maxHealth;
      
      _playerManager.UpdateHealthBar();
      _playerManager.ResetDie();
      _enemySpawner.ClearAllEnemies();
      _enemySpawner.StartSpawning();
     Vector3 newpos = new Vector3(-8, 2.1f, -6);
      _teleportPlayer.TeleportPlayer(newpos);
     timeRemaining = 10;
    gameObject.SetActive(false);
      
  }
}
