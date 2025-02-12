using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WeaponSystem : MonoBehaviour
{
    public TextMeshProUGUI PressTabText;
    public Transform weaponBack; // ตำแหน่งดาบที่หลัง
    public Transform weaponHand; // ตำแหน่งดาบที่มือ
    public GameObject sword; // ตัวดาบ
    public Animator characterAnimator; // Animator ของตัวละคร
    
    private bool isDrawn = false; // สถานะว่าชักดาบออกมาหรือยัง

    public bool GetIsDrawn => isDrawn;
    // เพิ่มตัวแปรสำหรับระบบ idle
    public float idleTimeBeforeSheath = 10f; // เวลาที่จะรอก่อนเก็บดาบ (วินาที)
    private float lastMovementTime; // เวลาล่าสุดที่มีการเคลื่อนไหว
    private Vector3 lastPosition; // ตำแหน่งล่าสุดของตัวละคร
    private Quaternion lastRotation; // การหมุนล่าสุดของตัวละคร
    private PlayerAttack _playerAttack;
    
    // เพิ่มตัวแปรใหม่
    private bool isSheathing = false; // ป้องกันการ sheath ซ้ำ
    private float sheathCooldown = 1f; // ระยะเวลารอระหว่างการ sheath
    private float lastSheathTime; // เวลาล่าสุดที่ทำการ sheath
    void Start()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
        lastMovementTime = Time.time;
        lastSheathTime = Time.time;
        _playerAttack = GetComponent<PlayerAttack>();
    }
    
    void Update()
    {
        if (!isDrawn)
        {
            PressTabText.gameObject.SetActive(true);
        }
        else
        {
            PressTabText.gameObject.SetActive(false);
        }
        CheckMovement();
        CheckIdleTime();
        // ตัวอย่างการเช็คว่าอยู่ในโหมดต่อสู้หรือไม่
        if (Input.GetKeyDown(KeyCode.Tab)) // หรือปุ่มที่คุณต้องการ
        {
            if (!isDrawn)
            {
                ResetIdleTimer();
                DrawWeaponAnim();

            }
            else
            {
               
                SheathWeaponAnim();
            }
        }
    }
    void CheckMovement()
    {
        // ตรวจสอบการเคลื่อนที่หรือหมุน
        if (transform.position != lastPosition || transform.rotation != lastRotation)
        {
            // มีการเคลื่อนไหว อัพเดทเวลา
            lastMovementTime = Time.time;
        }

        // อัพเดทตำแหน่งและการหมุนล่าสุด
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    void CheckIdleTime()
    {
        if (isDrawn && !isSheathing) // เช็คว่าไม่ได้กำลัง sheath อยู่
        {
            float idleTime = Time.time - lastMovementTime;
            float timeSinceLastSheath = Time.time - lastSheathTime;

            // เช็คว่าผ่าน cooldown แล้ว
            if (idleTime >= idleTimeBeforeSheath && timeSinceLastSheath >= sheathCooldown )
            {
                StartCoroutine(SheathWeaponRoutine());
            }
        }
    }
    IEnumerator SheathWeaponRoutine()
    {
        isSheathing = true;
        SheathWeaponAnim();
        lastSheathTime = Time.time;
        
        // รอให้ animation จบ
        yield return new WaitForSeconds(sheathCooldown);
        
        isSheathing = false;
    }

    // เพิ่มฟังก์ชันรีเซ็ตเวลา idle สำหรับเรียกใช้เมื่อมีการต่อสู้หรือทำ action อื่นๆ
    public void ResetIdleTimer()
    {
        lastMovementTime = Time.time;
    }
    public void DrawWeapon()
    {
        if (!isDrawn)
        {
            // เริ่มเล่น animation ชักดาบ
           
        
            // ย้ายดาบจากหลังไปที่มือ
            sword.transform.parent = weaponHand;
            sword.transform.localPosition = Vector3.zero;
            sword.transform.localRotation = Quaternion.identity;
        
            isDrawn = true;
        }
    }

    public void DrawWeaponAnim()
    {
        if (!isDrawn && !isSheathing)
        {
            characterAnimator.SetTrigger("DrawWeapon");
                
        }
    }
    public void SheathWeaponAnim()
    {
        if (isDrawn)
        {
            
            characterAnimator.SetTrigger("SheathWeapon");
                
        }
    }
    public void SheathWeapon()
    {
        if (isDrawn)
        {
            // เริ่มเล่น animation เก็บดาบ
           
        
            // ย้ายดาบจากมือไปที่หลัง
            sword.transform.parent = weaponBack;
            sword.transform.localPosition = Vector3.zero;
            sword.transform.localRotation = Quaternion.identity;
        
            isDrawn = false;
        }
    }
    
}
