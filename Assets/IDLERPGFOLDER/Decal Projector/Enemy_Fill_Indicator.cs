using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   
public class Enemy_Fill_Indicator : MonoBehaviour
{
    public Image fillImage;
    public float fillDuration = 2f;
    public Vector3 hitboxSize = new Vector3(1f, 1f, 2f); // ขนาดของ hitbox
    public float hitboxDistance = 1f; // ระยะห่างจากตัวละคร
    public LayerMask hitboxLayer; // Layer ที่ต้องการตรวจสอบการชน
    public GameObject Indicator;
    private void Start()
    {
        if (fillImage == null)
        {
            fillImage = GetComponent<Image>();
        }

        StartCoroutine(FillAndDestroy());
    }

    private IEnumerator FillAndDestroy()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fillDuration)
        {
            elapsedTime += Time.deltaTime;
            fillImage.fillAmount = Mathf.Clamp01(elapsedTime / fillDuration);
            yield return null;
        }

        // Image is now filled, destroy the GameObject
        DoDMG();
        Indicator.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
       Destroy(gameObject);
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
