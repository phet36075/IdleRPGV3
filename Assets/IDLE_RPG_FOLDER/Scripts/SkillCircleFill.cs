using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SkillCircleFill : MonoBehaviour
{
    public DecalProjector decalProjector;
    public float fadeDuration = 2f;
    public float maxProjectionDepth = 5f;
    public float maxPivotZ = 2.6f;
    public Vector3 hitboxSize = new Vector3(1f, 1f, 2f); // ขนาดของ hitbox
    public float hitboxDistance = 1f; // ระยะห่างจากตัวละคร
    public LayerMask hitboxLayer; // Layer ที่ต้องการตรวจสอบการชน
    private void Update()
    {
      
    }

    void Start()
    {
        if (decalProjector == null)
        {
            decalProjector = GetComponent<DecalProjector>();
        }

        StartCoroutine(AdjustProjectionDepth());
        Destroy(gameObject,fadeDuration+1f);
    }
    System.Collections.IEnumerator AdjustProjectionDepth()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float depth = Mathf.Lerp(0f, maxProjectionDepth, elapsedTime / fadeDuration);
            float pivotz = Mathf.Lerp(0f, maxPivotZ, elapsedTime / fadeDuration);
            decalProjector.size = new Vector3(decalProjector.size.x, decalProjector.size.y, depth);
            decalProjector.pivot = new Vector3(decalProjector.pivot.x, decalProjector.pivot.y, pivotz);
            
           
            yield return null;
        }
        
       // yield return new WaitForSeconds(fadeDuration);
        DoDMG();
    }

    void DoDMG()
    {
        Vector3 hitboxCenter = transform.position + transform.forward * (hitboxDistance + hitboxSize.z / 2f);
        Collider[] hitColliders = Physics.OverlapBox(hitboxCenter, hitboxSize / 2f, transform.rotation, hitboxLayer);
        foreach (var hitCollider in hitColliders)
        {
            PlayerManager playerHealth = hitCollider.GetComponent<PlayerManager>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10,10);
                Debug.Log("HIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIT");
            }
            
        }
    }
    private void OnDrawGizmos()
    {
        //Draw Hit Box Cube last hit
        Gizmos.color = Color.yellow;
        Vector3 hitboxCenter = transform.position + transform.forward * (hitboxDistance + hitboxSize.z / 2f);
        Gizmos.matrix = Matrix4x4.TRS(hitboxCenter, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, hitboxSize);
    }
}
