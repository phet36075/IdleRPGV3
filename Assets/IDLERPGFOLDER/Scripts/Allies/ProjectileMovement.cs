using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    public float speed = 10f;
    public float baseDamage = 10f;
    public float explosionRadius = 3f;
    private Transform target;
    public GameObject hitEffectPrefab;
    public float damageVariation = 0.2f;  // กำหนดเปอร์เซ็นต์การแกว่งของดาเมจ (เช่น 0.2 = ±20%)
    public float distanceEnemy = 0.2f;

  //  private PlayerManager _playerManeger;
     void Start()
     {
         //_playerManeger = FindObjectOfType<PlayerManager>();
     }
    void Update()
    {
       // baseDamage = _playerManeger.playerData.weaponDamage;
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
          //  transform.position += direction * speed * Time.deltaTime;
          
          
            transform.LookAt(target);
            gameObject.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * speed);
            // ทำลายโปรเจกไทล์เมื่อถึงเป้าหมาย
            if (Vector3.Distance(transform.position, target.position) < distanceEnemy)
            {
                // HitTarget();
              //  Explode();
                // เพิ่มโค้ดเพื่อสร้างเอฟเฟกต์การโจมตีหรือลดพลังชีวิตของศัตรูที่นี่
            }
        }
        else
        {
            Destroy(gameObject);  // ทำลายโปรเจกไทล์ถ้าไม่มีเป้าหมาย
        }
        
    }
        
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    public void SetExplosionRadius(float newRadius)
    {
        explosionRadius = newRadius;
    }
    
    public void SetDamage(float newDamage)
    {
        baseDamage = newDamage;
    }
   
    
   public void Explode()
    {
       /* if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            //Destroy(effect, 1f); // ลบ effect หลังจาก 2 วินาที
            
        }*/
        int finalDamage = CalculateDamage();
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            if (nearbyObject.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = nearbyObject.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(finalDamage);
                }
            }
           
        }
        
        Debug.Log("Destroy Projectile");
        //tDestroy(gameObject);
        
    }

    int CalculateDamage()
    {
        float randomFactor = Random.Range(1f - damageVariation, 1f + damageVariation);
        int finalDamage = Mathf.RoundToInt(baseDamage * randomFactor);
        return finalDamage;
    }
   /* void HitTarget()
    {
        EnemyHealth enemyHealth = target.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamge(baseDamage);
        }
        
        // add effect here
       // Instantiate(explosionEFX, explosionLocation.position, Quaternion.identity);
        Destroy(gameObject);
    }*/
    
    }

