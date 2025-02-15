using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class CongratulationUI : MonoBehaviour
{
    public float timeRemaining = 10;
    public TextMeshProUGUI timeText;

   
    
    public Transform NextDoorLocation;
    public GameObject PortalEffect;
    //public Animator playerAnimator;
   private AIController _aiController;

   private PlayerController _playerController;
   private TestTeleportPlayer _teleportPlayer;
   private EnemySpawner enemySpawner;
    //public NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        _teleportPlayer = FindObjectOfType<TestTeleportPlayer>();
        _aiController = FindObjectOfType<AIController>();
        _playerController = FindObjectOfType<PlayerController>();
         enemySpawner = FindObjectOfType<EnemySpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining -= Time.deltaTime;
        timeText.text = timeRemaining.ToString("#");

        if (timeRemaining <= 0)
        {
            
           GoNextStage();
          
        }
    }

    public void GoNextStage()
    {
        timeRemaining = 10;
        enemySpawner.NextStage();
        gameObject.SetActive(false);
    }

    
}
