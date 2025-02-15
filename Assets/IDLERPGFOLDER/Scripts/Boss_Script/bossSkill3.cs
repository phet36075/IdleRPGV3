using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossSkill3 : BaseBossSkill
{
    public GameObject BossSkill3_effectPrefab;  // แยก effect ออกมาเป็น prefab ต่างหาก
    [SerializeField] private Vector3 hitboxOffset = Vector3.forward;
    
    // Start is called before the first frame update
   
    public  void BossSkill3_OnEffectSpawn()
    {
        Vector3 spawnPosition = transform.position + transform.rotation * hitboxOffset;
        GameObject spawnedEffect = Instantiate(BossSkill3_effectPrefab, spawnPosition, transform.rotation);
        
        Destroy(spawnedEffect, 3f);
    }
    
    public void BossSkill3_OnSkillEnd()
    {
        base.OnSkillEnd();
    }
    
}
