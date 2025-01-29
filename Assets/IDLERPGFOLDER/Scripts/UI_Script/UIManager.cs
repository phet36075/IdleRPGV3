using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private PlayerManager _PlayerManager;
    private CurrencyManager _currencyManager;
    private EnemySpawner _enemySpawner;
    public TextMeshProUGUI _txtPlayerHealth;
    public TextMeshProUGUI _txtStage;
    //public AIController _AIController;
    public TextMeshProUGUI _txtMoney;
    public TextMeshProUGUI _txtTotalToDefeated;
    public TextMeshProUGUI _txtEnemiesDefeated;
  //  public StageManager _StageManager;

   // public EnemySpawner _EnemySpawner;
    // Start is called before the first frame update
    void Start()
    {
        _currencyManager = FindObjectOfType<CurrencyManager>();
        _PlayerManager = FindObjectOfType<PlayerManager>();
        _enemySpawner = FindObjectOfType<EnemySpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        _txtMoney.text = _currencyManager.CurrentMoney.ToString();
        _txtPlayerHealth.text = _PlayerManager.currentHealth.ToString("0");
        _txtStage.text = _enemySpawner.GetStage().ToString();
        /* _txtEnemiesDefeated.text = _EnemySpawner.enemiesDefeated.ToString();
         _txtTotalToDefeated.text = _EnemySpawner.totalEnemiesToDefeat.ToString();*/
    }
}
