using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossSkill3 : MonoBehaviour
{
    public float WaitTime = 3f;
    public GameObject Indicator;
    public GameObject bossSkill3Prefab;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitBeforeSpawnSkill());
    }

    IEnumerator WaitBeforeSpawnSkill()
    {
        yield return new WaitForSeconds(WaitTime);
        bossSkill3Prefab.gameObject.SetActive(true);
        Indicator.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);

    }
    // Update is called once per frame
    void Update()
    {
       
    }
    
    
    
}
