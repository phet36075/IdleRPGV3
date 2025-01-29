using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRoaming : MonoBehaviour
{
    public float roamRadius = 10f;

    public float roamDelay = 3f;
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    private Vector3 startingPosition;
    
    public AudioClip walkingSound; // คลิปเสียงสำหรับการเดิน
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        startingPosition = transform.position;
        RoamToNewPosition();
        animator = GetComponent<Animator>();
        
        // กำหนดค่า AudioSource สำหรับเสียงแบบ 3D
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = walkingSound;
        audioSource.loop = true; // ให้เสียงเล่นวนซ้ำ
        audioSource.spatialBlend = 1.0f; // ตั้งค่า spatialBlend ให้เป็น 1 เพื่อใช้เสียงแบบ 3D
        audioSource.minDistance = 1f; // ระยะใกล้สุดที่เสียงจะดังชัดเจน
        audioSource.maxDistance = 15f; // ระยะไกลสุดที่เสียงจะเริ่มเบา
        audioSource.rolloffMode = AudioRolloffMode.Linear; // กำหนดโหมดการลดระดับเสียงตามระยะทาง
        
        
    }

    void RoamToNewPosition()
    {
        //Debug.Log("Roaming");
        // คำนวณตำแหน่งใหม่แบบสุ่มภายในรัศมี
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += startingPosition;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDirection, out navHit, roamRadius, -1))
        {
//            Debug.Log("Moving to new position: " + navHit.position);  // ตรวจสอบตำแหน่งเป้าหมาย
            navMeshAgent.SetDestination(navHit.position);
            
        }
        else
        {
     //       Debug.Log("No valid NavMesh position found.");
        }
        // ตั้งเวลาเพื่อเรียกการ Roam ใหม่หลังจากถึงจุดหมาย
        Invoke("RoamToNewPosition", roamDelay + Random.Range(0f, 3f));
    }
    

    // Update is called once per frame
    void Update()
    {
        float speed = navMeshAgent.velocity.magnitude; // คำนวณความเร็ว
        animator.SetFloat("Speed", speed); // อัปเดตพารามิเตอร์ Speed ใน Animator
        if (navMeshAgent.velocity.magnitude > 0.1f)
        {
            animator.SetBool("IsWalking", true);  // เริ่มเล่นอนิเมชั่นเดิน
            PlayWalkSound();
        }
        else
        {
            animator.SetBool("IsWalking", false); // หยุดเล่นอนิเมชั่นเดิน
            StopWalkSound();
        }
    }

    void PlayWalkSound()
    {
        if (!audioSource.isPlaying) // ถ้าเสียงยังไม่เล่น ให้เล่นเสียง
        {
            audioSource.Play();
        }
    }

    void StopWalkSound()
    {
        if (audioSource.isPlaying) // ถ้าเสียงกำลังเล่นอยู่ ให้หยุดเสียง
        {
            audioSource.Stop();
        }
    }
}
