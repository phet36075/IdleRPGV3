using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BossAttackCircle : MonoBehaviour
{
    public float fillDuration = 5f;    // ระยะเวลาในการเติมเต็มวงกลม (วินาที)
    public float damageAmount = 10f;   // จำนวนความเสียหายที่จะให้
    public string playerTag = "Player"; // แท็กของผู้เล่นหรือตัวละครที่จะรับความเสียหาย

    private Material circleMaterial;
    private float fillAmount = 0f;
    private bool isActivated = false;

    void Start()
    {
        // ดึง Material จาก Renderer ของวัตถุนี้
        circleMaterial = GetComponent<Renderer>().material;
    }

    void Update()
    {
        if (isActivated)
        {
            // เพิ่มค่า fill amount
            fillAmount += Time.deltaTime / fillDuration;
            circleMaterial.SetFloat("_FillAmount", fillAmount);

            // ตรวจสอบว่าเต็มหรือยัง
            if (fillAmount >= 1f)
            {
                DealDamage();
                ResetCircle();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Player HI");
            isActivated = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            ResetCircle();
        }
    }

    void DealDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, transform.localScale.x / 2f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(playerTag))
            {
                // สมมติว่าผู้เล่นมี component ชื่อ PlayerHealth
                PlayerManager playerHealth = hitCollider.GetComponent<PlayerManager>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageAmount,10);
                }
            }
        }
    }

    void ResetCircle()
    {
        isActivated = false;
        fillAmount = 0f;
        circleMaterial.SetFloat("_FillAmount", fillAmount);
    }
}
