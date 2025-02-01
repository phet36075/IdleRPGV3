using System.Collections;
using UnityEngine;
public class Skill1 : MonoBehaviour,ISkill
{
    [Header("--------------------Damage--------------------")]
    public float skillDmgFirst2Hit = 1.5f;
    
    public float skillDmgLastHitAttack = 0.6f;
    public Animator animator;
    public float firstDamageRadius = 2f;
    public float firstDamageDistance = 2f;
    public float cooldownTime = 5f;
    public GameObject skillEffectPrefab;
    public GameObject skillEffectPrefabSlash;

   
    private bool isOnCooldown = false;
    private float lastUseTime = -Mathf.Infinity;
    private PlayerController _aiController;
    public int numberOfHits = 5;
    public float timeBetweenHits = 0.2f;
  //  private float actualCooldownStartTime;
    private float skillDuration;
    
    
    /*public Vector3 hitboxSize = new Vector3(1f, 1f, 2f); // ขนาดของ hitbox
    public float hitboxDistance = 1f; // ระยะห่างจากตัวละคร
    public LayerMask hitboxLayer; // Layer ที่ต้องการตรวจสอบการชน*/

    private PlayerManager playerManager;
    private float attackDamage;
    private DamageData damageData;

    public Vector3 AoEMods;
    public float AoEModsMultiplier = 1f;
    private float defaultAoEModsMultiplier = 1f;
    private void Start()
    {
        AoEModsMultiplier = defaultAoEModsMultiplier;
        playerManager = GetComponent<PlayerManager>();
        attackDamage = playerManager.CalculatePlayerAttackDamage();
        damageData = new DamageData(attackDamage, playerManager.playerData.armorPenetration , playerManager.playerData.elementType);
        
      
        
        _aiController = FindObjectOfType<PlayerController>();
        skillDuration = 3; 
    }

    public void UseSkill()
    {
        // Check Element
        damageData = new DamageData(attackDamage, playerManager.playerData.armorPenetration , playerManager.playerData.elementType);
        if (damageData.elementType == ElementType.Wind)
        {
            AoEMods = new Vector3(2f, 2f, 2f);
          
        }
        else
        {
            AoEModsMultiplier = defaultAoEModsMultiplier;
            AoEMods = new Vector3(1f, 1f, 1f);
           
        }
        
        if (!isOnCooldown)
        {
            lastUseTime = Time.time;
            StartCoroutine(Cooldown());
            animator.SetTrigger("UseSkill");
        }
        Debug.Log("YOUR ON COOLDOWN" + (cooldownTime - (Time.time - lastUseTime)));
        
    }
    private IEnumerator Cooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        isOnCooldown = false;
    }

   /* void UpdateDamage()
    {
        attackDamage = playerManager.CalculatePlayerAttackDamage(skillDmgFirst2Hit);
        damageData = new DamageData(attackDamage, playerManager.playerData.armorPenetration , playerManager.playerData.elementType);
    }

    void UpdateDamageSecondHit()
    {
        attackDamage = playerManager.CalculatePlayerAttackDamage(skillDmgLastHitAttack);
        damageData = new DamageData(attackDamage, playerManager.playerData.armorPenetration , playerManager.playerData.elementType);
    }*/

    public void PerformLastHit()
    {
       // StartCoroutine(LastHit());
       LastHit();
    }

    private void LastHit()
    {
        Vector3 effectPosition = transform.position + transform.forward * 2f;
        Quaternion effectRotation = transform.rotation;
        // แสดง effect
        GameObject effect = Instantiate(skillEffectPrefab, effectPosition, effectRotation);
        
        // ปรับขนาด effect
        effect.transform.localScale = AoEMods; // ปรับขนาดเป็น 2 เท่า
        Destroy(effect, 2f); // ลบ effect หลังจาก 2 วินาที
       
        // ปล่อยดาเมจหลายที
     /*   for (int i = 0; i < numberOfHits; i++)
        {
            PerformSingleHit();
               yield return new WaitForSeconds(timeBetweenHits);
        } */
    }
    
   

  /*  void PerformSingleHit()
    {
        Vector3 hitboxCenter = transform.position + transform.forward * (hitboxDistance  * AoEModsMultiplier + hitboxSize.z / 2f);

        // ตรวจสอบการชนกัน
        Collider[] hitColliders = Physics.OverlapBox(hitboxCenter, (hitboxSize / 2f), transform.rotation, hitboxLayer);
       
        foreach (var hitCollider in hitColliders)
        {
            IDamageable target = hitCollider.GetComponent<IDamageable>();
            if (target != null)
            {
               // PlayerManager playerManager = GetComponent<PlayerManager>();
              //  float attackDamage = playerManager.CalculatePlayerAttackDamage();
               
               // DamageData damageData = new DamageData(attackDamage, playerManager.playerData.armorPenetration , playerManager.playerData.elementType); 
             //  UpdateDamageSecondHit();
                target.TakeDamage(damageData);
                
            }
        }
    }*/

    /*public void ShowEffect1()
    {
        
    }*/

    public void DoAnimDamage()
    {
        Vector3 effectPosition = transform.position + transform.forward * 2f; // ระยะห่างจาก GameObject ไปทางด้านหน้า 
        Quaternion effectRotation = transform.rotation;
        GameObject effect = Instantiate(skillEffectPrefabSlash, effectPosition, effectRotation);
        // ปรับขนาด effect
        effect.transform.localScale = AoEMods; // ปรับขนาดเป็น 2 เท่า
        Destroy(effect, 0.2f);
        
        /*Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * firstDamageDistance, firstDamageRadius * AoEModsMultiplier); // ระยะห่างจาก GameObject ไปทางด้านหน้า และ รัศมีของวงกลม
        foreach (var hitCollider in hitColliders)
        {
            IDamageable target = hitCollider.GetComponent<IDamageable>();
            if (target != null)
            {
                UpdateDamage();
                target.TakeDamage(damageData); 
            }
            
        }*/
    }

    public void EnableNav()
    {
        //_aiController.GetComponent<NavMeshAgent>().enabled = true;
        _aiController.isAIActive = true;
    }

    public void DisableNav()
    {
        //_aiController.GetComponent<NavMeshAgent>().enabled = false;
        _aiController.isAIActive = false;
    }
    public float GetCooldownPercentage()
    {
        if (!isOnCooldown)
        {
            return 0f;
        }
        float elapsedTime = Time.time - lastUseTime;
        return 1f - Mathf.Clamp01(elapsedTime / cooldownTime);
    }
    public float GetRemainingCooldownTime()
    {
        if (!isOnCooldown)
        {
            return 0f;
        }

        float timeSinceUse = Time.time - lastUseTime;
      
        
            // สกิลยังทำงานอยู่
            return cooldownTime  - timeSinceUse;
        
       
    }
   
    
    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }

    public float GetCooldownTime()
    {
        return cooldownTime;
    }
   /* private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Vector3 sphereCenter = transform.position + transform.forward * firstDamageDistance;
        Gizmos.DrawSphere(sphereCenter, firstDamageRadius);
        
        
        //Draw Hit Box Cube last hit
        Gizmos.color = Color.red;
        Vector3 hitboxCenter = transform.position + transform.forward * (hitboxDistance + hitboxSize.z / 2f);
        Gizmos.matrix = Matrix4x4.TRS(hitboxCenter, transform.rotation, Vector3.one);
        Gizmos.DrawCube(Vector3.zero, hitboxSize);
    }*/
}

