using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossSkill1 : MonoBehaviour
{
    public GameObject objectPrefab; // Prefab ของวัตถุที่จะสร้าง
    public float distanceInFront = 2f; // ระยะห่างด้านหน้าตัวละคร

    public GameObject objectToAttach; // กำหนด prefab ที่จะติดกับผู้เล่นในหน้า Inspector
    public Vector3 attachmentOffset = Vector3.zero; // ระยะห่างจากตัวผู้เล่น
  //  public Vector3 attachmentScale = Vector3.one; // ขนาดของ object ที่จะติด

    private GameObject attachedObject;
    private Transform enemyTransform;
    
    public void SpawnObjectInFront()
    {
        // สร้างวัตถุที่ตำแหน่งด้านหน้าตัวละคร
        Vector3 spawnPosition = transform.position + transform.forward * distanceInFront;
        
        // สร้างวัตถุและกำหนดให้เป็นลูกของตัวละคร
        // คำนวณตำแหน่งและทิศทางสำหรับ effect
        
        Quaternion effectRotation = transform.rotation;
        GameObject spawnedObject = Instantiate(objectPrefab, spawnPosition,effectRotation);
        
        // หันหน้าวัตถุไปทางเดียวกับตัวละคร (ถ้าต้องการ)
        spawnedObject.transform.forward = transform.forward;
    }
    
    void Start()
    {
        // หา Transform ของผู้เล่น (สมมติว่าสคริปต์นี้ติดอยู่กับ Player object)
        enemyTransform = transform;

        // สร้างและติด object กับผู้เล่นทันทีเมื่อเริ่มเกม
        //AttachObject();
    }

    void Update()
    {
        if (attachedObject != null)
        {
            // อัปเดตตำแหน่งของ attached object ให้ติดกับผู้เล่นตลอดเวลา
            attachedObject.transform.position = enemyTransform.position + attachmentOffset;
        }
    }

    public void AttachObject()
    {
        // สร้าง object และกำหนดตำแหน่งเริ่มต้น
        Quaternion auraRotation = Quaternion.Euler(-90, 0, 0);
        attachedObject = Instantiate(objectToAttach, enemyTransform.position + attachmentOffset,auraRotation);

        // ปรับ scale ของ object
      //  attachedObject.transform.localScale = attachmentScale;
    }

    public void DestroyAuraThis()
    {
        Destroy(attachedObject);
    }
}
