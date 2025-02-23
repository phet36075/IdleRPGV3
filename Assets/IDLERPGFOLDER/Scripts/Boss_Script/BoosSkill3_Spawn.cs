using UnityEngine;
using System.Collections;
public class BoosSkill3_Spawn : MonoBehaviour
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
}
