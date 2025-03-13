using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public class CongratulationUI : MonoBehaviour
{
    public float timeRemaining = 10;
    public TextMeshProUGUI timeText;
   private EnemySpawner enemySpawner;
    // Start is called before the first frame update
    void Start()
    {
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
