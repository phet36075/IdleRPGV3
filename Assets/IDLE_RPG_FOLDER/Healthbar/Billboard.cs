using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cam;

    private void Start()
    {
       
    }

    void LateUpdate()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
       // Transform camTransform = camPos.transform;
        transform.LookAt(transform.position + cam.forward);
    }
}
