using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSpawn : MonoBehaviour
{
    public float WaitMeteorTime = 3;
    public float DestroyTime = 2.5f;
    public GameObject MeteorFxPrefab;
    private BossBehavior _bossData;
    
    
    public float radius = 1.15f;
    public LayerMask layerMask = -1;
    public Color gizmoColor = Color.red;
    

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    // Start is called before the first frame update
    void Start()
    {
        _bossData = FindObjectOfType<BossBehavior>();
        StartCoroutine(WaitForMeteorSpawn());
    }
    
    IEnumerator WaitForMeteorSpawn()
    {
        yield return new WaitForSeconds(WaitMeteorTime);
        MeteorFxPrefab.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, layerMask);
        
        foreach (var hitCollider in hitColliders)
        {
            PlayerManager playerStats = hitCollider.GetComponent<PlayerManager>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(10,_bossData._BossData.armorPenetration);
            }
        }
        yield return new WaitForSeconds(DestroyTime);
        Destroy(gameObject);
    }
    
    
}
