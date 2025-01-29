using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerWeapon : MonoBehaviour
{
  
    private PlayerAttack _playerAttack;
    private PlayerManager _playerManager;
    private void Start()
    {
        _playerAttack = GetComponent<PlayerAttack>();
        _playerManager = FindObjectOfType<PlayerManager>();
    }
    
    
   
   
}
