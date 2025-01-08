using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DestroyOnAnimationEnd : MonoBehaviour
{
  public void DestryoParent()
    {
        GameObject parent = gameObject.transform.parent.gameObject;
        Destroy(parent);
    }
}
