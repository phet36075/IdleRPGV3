using System.Collections;
using UnityEngine;

public class Destroy10Sec : MonoBehaviour
{
    public float SecToDestory = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartDestroy());
    }

    IEnumerator StartDestroy()
    {
        yield return new WaitForSeconds(SecToDestory);
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
