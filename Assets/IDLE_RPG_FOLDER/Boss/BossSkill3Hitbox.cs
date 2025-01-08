using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkill3Hitbox : MonoBehaviour
{
    public Vector3 hitboxSize = new Vector3(1f, 1f, 2f); // ขนาดของ hitbox
    public float hitboxDistance = 1f; // ระยะห่างจากตัวละคร
    public LayerMask hitboxLayer; // Layer ที่ต้องการตรวจสอบการชน

    private MaterialAlphaTransition _MaterialAlphaTransition;
 //   public float IndicatorTime = 2;
    // Start is called before the first frame update
    void Start()
    {
        _MaterialAlphaTransition = FindObjectOfType<MaterialAlphaTransition>();
        StartCoroutine("DamagePlayerInSecs");
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DamagePlayerInSecs()
    {
        yield return new WaitForSeconds(_MaterialAlphaTransition.transitionDuration-0.1f);
        DoDMGToPlayer();
    }

    void DoDMGToPlayer()
    {
        Vector3 hitboxCenter = transform.position + transform.forward * (hitboxDistance + hitboxSize.z / 2f);
        // ตรวจสอบการชนกัน
        Collider[] hitColliders = Physics.OverlapBox(hitboxCenter, hitboxSize / 2f, transform.rotation, hitboxLayer);
        // Vector3 skillCenter = transform.position + transform.forward * skillRange;
        //   Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * 2f, damageRadius);
        foreach (var hitCollider in hitColliders)
        {
            PlayerManager playerStats = hitCollider.GetComponent<PlayerManager>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(10,10);
            }
            
        }
    }
    private void OnDrawGizmos()
    {
        //Draw Hit Box Cube last hit
        Gizmos.color = Color.red;
        Vector3 hitboxCenter = transform.position + transform.forward * (hitboxDistance + hitboxSize.z / 2f);
        Gizmos.matrix = Matrix4x4.TRS(hitboxCenter, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, hitboxSize);
    }
}
