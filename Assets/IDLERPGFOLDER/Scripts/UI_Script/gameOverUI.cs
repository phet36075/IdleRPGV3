
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
    [SerializeField] private StageData stageData;
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
      
     // playerAnim.SetTrigger("Reset");
     // allyAnim.SetTrigger("Reset");
      _playerManager.ResetDie();
     // stageData.currentStage = _enemySpawner.currentStage;
     Vector3 newpos = new Vector3(-8, 2.1f, -6);
      _teleportPlayer.TeleportPlayer(newpos);
     // SceneManager.LoadScene(stageName);
     timeRemaining = 10;
    gameObject.SetActive(false);
      
  }
}
