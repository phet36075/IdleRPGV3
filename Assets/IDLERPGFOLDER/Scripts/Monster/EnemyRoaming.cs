using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRoaming : MonoBehaviour
{
    public float roamRadius = 10f;

    public float roamDelay = 3f;
    public Animator animator;
    private NavMeshAgent navMeshAgent;

    private Vector3 startingPosition;
    
    public AudioClip walkingSound; // คลิปเสียงสำหรับการเดิน
    public AudioSource audioSource;

    [Header("Sound Settings")]
    [SerializeField] private float delayBetweenSounds = 0.5f; // ระยะเวลาดีเลย์ระหว่างการเล่นเสียง (วินาที)
    
    public bool canPlaySound = true; // ตัวแปรเช็คว่าสามารถเล่นเสียงได้หรือไม่
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        startingPosition = transform.position;
        RoamToNewPosition();
      
        
        // กำหนดค่า AudioSource สำหรับเสียงแบบ 3D
       // audioSource = gameObject.AddComponent<AudioSource>();
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
            if (navMeshAgent.enabled)
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
    void PlayWalkSound()
    {
        if (canPlaySound && !audioSource.isPlaying) // ถ้าสามารถเล่นเสียงได้และเสียงยังไม่เล่น
        {
            audioSource.PlayOneShot(walkingSound);
            canPlaySound = false; // ตั้งค่าให้ไม่สามารถเล่นเสียงได้ชั่วคราว
            StartCoroutine(SoundDelay()); // เริ่ม Coroutine สำหรับดีเลย์
        }
    }
    
    IEnumerator SoundDelay()
    {
        // รอเวลาตามที่กำหนดไว้ในตัวแปร delayBetweenSounds
        yield return new WaitForSeconds(delayBetweenSounds);
        canPlaySound = true; // อนุญาตให้เล่นเสียงได้อีกครั้งหลังจากรอครบตามเวลาที่กำหนด
    }
    
    void StopWalkSound()
    {
        if (audioSource.isPlaying) // ถ้าเสียงกำลังเล่นอยู่ ให้หยุดเสียง
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }
    
    void Update()
    {
        float speed = navMeshAgent.velocity.magnitude; // คำนวณความเร็ว
        animator.SetFloat("Speed", speed); // อัปเดตพารามิเตอร์ Speed ใน Animator
        
        if (speed > 0.1f)
        {
            //animator.SetBool("IsWalking", true);  // เริ่มเล่นอนิเมชั่นเดิน
            PlayWalkSound();
        }
        else
        {
            //animator.SetBool("IsWalking", false); // หยุดเล่นอนิเมชั่นเดิน
            StopWalkSound();
        }
    }
}
