using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TargetIndicator : MonoBehaviour
{
    public GameObject indicator;

    public void ShowIndicator(Transform transform1)
    {
        indicator = Instantiate(indicator, transform1.transform.position, Quaternion.identity);
        indicator.SetActive(true);
        
    }

    private void Update()
    {
        
    }

    public void HideIndicator()
    {
      Destroy(indicator);
    }
}
