using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySpawnFxMonster : MonoBehaviour
{
   public GameObject FX;

   private void Start()
   {
      Destroy(FX,0.5f);
   }
}
