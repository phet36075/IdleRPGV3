using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChanger : MonoBehaviour
{
    public PlayerData PlayerData;
    [SerializeField] private EnemyData _enemyData;
    private PlayerManager _playerManager;
    public string nextLevelName;


    private void Start()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LoadNextLevel();
        }
    }

    private void LoadNextLevel()
    {
        _enemyData.BaseAttack += 10;
        _enemyData.maxhealth += 100;
        _enemyData.moneyDrop += 100;
        //PlayerData.stage += 1;
        PlayerData.currentHealth = _playerManager.currentHealth;
        SceneManager.LoadScene(nextLevelName);
    }
}
