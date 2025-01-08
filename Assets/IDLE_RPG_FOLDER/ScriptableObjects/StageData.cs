using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StageData",menuName = "Game/StageData")]
public class StageData : ScriptableObject
{
    public int currentStage = 1;
    [SerializeField] private int DefaultStage = 1;

    private void OnEnable()
    {
        currentStage = DefaultStage;
    }
}
