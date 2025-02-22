using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DragonMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float takeoffHeight = 10f;
    [SerializeField] private float flapDuration = 1.5f;
    [SerializeField] private float defaultSpeed = 3.5f;
    [SerializeField] private float glideSpeed = 15f;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float takeOffDuration = 3f;
    
    [Header("Random Flight Settings")]
    [SerializeField] private float minX = -50f;
    [SerializeField] private float maxX = 50f;
    [SerializeField] private float minZ = -50f;
    [SerializeField] private float maxZ = 50f;
    [SerializeField] private int FlightCount = 0;
    [Header("Components")]
    [SerializeField] private Animator animator;

    [SerializeField] private Collider dragonHitbox;
    private NavMeshAgent agent;
    private Transform player;
    [SerializeField] private EnemyHealth enemyHealth;
    
    [Header("Enemy Settings")]
    [SerializeField] private float detectionRange = 10f;  // ระยะการตรวจจับ Player
    [SerializeField] public float attackRange = 2f;      // ระยะโจมตี
    [SerializeField] private float attackCooldown = 2f;   // ระยะเวลารอระหว่างการโจมตี
    private bool screamFirstTime;
    private float nextAttackTime;
    private bool isAttacking;
    private int attackCount;
    private bool isFlyingTowardPlayer;
    // Animation parameter names
    private const string TAKEOFF_TRIGGER = "Takeoff";
    private const string FLAP_TRIGGER = "Flap";
    private const string GLIDE_TRIGGER = "Glide";
    private const string LAND_TRIGGER = "Land";


    private float lastTimeAttack;
    
    // State machine
    private enum DragonState
    {
        GroundIdle,
        FlyAttack,
        Takeoff,
        FlameAttack,
        Flapping,
        Gliding,
        Landing
    }
    private DragonState currentState;

    // State tracking
    private float stateTimer;
    private int flapCount;
    private Vector3 currentTarget;
    private const float TARGET_REACHED_THRESHOLD = 10f;

    private void Start()
    {
        currentState = DragonState.GroundIdle;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
     //   ForceFly();
        // ตั้งค่า NavMeshAgent
        agent.baseOffset = 0;
       
        agent.angularSpeed = rotationSpeed * 100;
        agent.acceleration = 8;
        agent.stoppingDistance = TARGET_REACHED_THRESHOLD;
    }

    private void Update()
    {
        switch (currentState)
        {
            case DragonState.GroundIdle:
                UpdateGroundIdle();
                break;
            case DragonState.FlyAttack:
                UpdateFlyAttack();
                break;
            case DragonState.Takeoff:
                UpdateTakeoff();
                break;
            case DragonState.FlameAttack:
                UpdateFlameAttack();
                break;
            case DragonState.Flapping:
                UpdateFlapping();
                break;
            case DragonState.Gliding:
                UpdateGliding();
                break;
            case DragonState.Landing:
                UpdateLanding();
                break;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            ForceFly();
        }
    }

    private void UpdateGroundIdle()
    {
        if (player == null) return;
        float speed = agent.velocity.magnitude;
        // คำนวณระยะห่างระหว่างศัตรูกับ Player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // ถ้าอยู่ในระยะตรวจจับ
        if (distanceToPlayer <= detectionRange)
        {
           
            animator.SetFloat("Speed", speed);
            // ถ้าอยู่ในระยะโจมตี
            if (distanceToPlayer <= attackRange)
            {
                if (!screamFirstTime)
                {
                    animator.SetTrigger("Scream");
                    screamFirstTime = true;
                }
                    
                // หยุดเดิน
                agent.isStopped = true;
                
                // โจมตีถ้าหมดเวลา Cooldown
                if (Time.time >= nextAttackTime)
                {
                    Attack();
                    StartCoroutine(FacePlayer());
                }
            }
            else
            {
                // เดินตาม Player
                // คำนวณความเร็ว
                animator.SetFloat("Speed", speed);
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
        }
        else
        {
            // อยู่นอกระยะตรวจจับ หยุดเดิน
            agent.isStopped = true;
        }


        if (ShouldStartFlameAttack())
        {
            animator.SetTrigger("FlameAttack");
            attackCount = 0;
        }
        
        // if (ShouldStartFlying())
        // {
        //     isFlyingTowardPlayer = true;
        //     StartTakeoff();
        // }

        if (ShouldStartTakeOff())
        {
          
            StartTakeoff();
        }
    }

    private void Attack()
    {
        nextAttackTime = Time.time + attackCooldown;
        attackCount++;
        animator.SetTrigger("ClawAttack");
    }
    IEnumerator FacePlayer()
    {
        if (player != null)
        {
            // คำนวณทิศทางไปยัง player
            Vector3 direction = player.position - transform.position;
            direction.y = 0; // ไม่หมุนในแกน y (ขึ้น-ลง)
        
            if (direction != Vector3.zero)
            {
                // คำนวณ rotation เป้าหมาย
                Quaternion targetRotation = Quaternion.LookRotation(direction);
            
                // หมุนช้าๆ จนกว่าจะถึงเป้าหมาย
                float angle = Quaternion.Angle(transform.rotation, targetRotation);
            
                // ทำการหมุนจนกว่าจะหันไปหา player เกือบสมบูรณ์ (น้อยกว่า 2 องศา)
                while (angle > 2f)
                {
                    // ค่อยๆ หมุนด้วย Slerp
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation, 
                        targetRotation, 
                        rotationSpeed * Time.deltaTime
                    );
                
                    // คำนวณมุมที่เหลืออยู่
                    angle = Quaternion.Angle(transform.rotation, targetRotation);
                
                    // รอจนกว่าจะถึงเฟรมถัดไป
                    yield return null;
                }
            
                // หมุนให้ตรงกับเป้าหมายเมื่อใกล้ถึง
                transform.rotation = targetRotation;
            }
        }
    }
    
    
    private void UpdateTakeoff()
    {
        stateTimer += Time.deltaTime;
      //  float targetHeight = Mathf.Lerp(0, takeoffHeight, stateTimer / flapDuration);
        
        // ปรับ baseOffset ของ NavMeshAgent แทนการเปลี่ยน y position โดยตรง
        //agent.baseOffset = targetHeight;
    
        if (stateTimer >= takeOffDuration)
        {
            StartFlapping();
        }
    }

    private void UpdateFlapping()
    {
        stateTimer += Time.deltaTime;
        if (stateTimer >= flapDuration)
        {
            stateTimer = 0;
            flapCount++;
            animator.SetTrigger(FLAP_TRIGGER);

            if (flapCount >= 2)
            {
                if (isFlyingTowardPlayer)
                {
                    StartFlyAttack();
                }
                else
                {
                    StartGliding();
                }
                
               //
            }
            
        }
    }

    private void UpdateGliding()
    {
        stateTimer += Time.deltaTime;
        // ถ้ายังไม่มีเป้าหมายหรือถึงเป้าหมายแล้ว ให้สุ่มเป้าหมายใหม่
        if (currentTarget == Vector3.zero || 
            Vector3.Distance(transform.position, currentTarget) < TARGET_REACHED_THRESHOLD)
        {
            GenerateNewTarget();
        }

        // ตรวจสอบว่าควรจะลงจอดหรือไม่
        if (ShouldLand())
        {
            StartLanding();
        }
    }

    private void UpdateLanding()
    {
        stateTimer += Time.deltaTime;
        float landingProgress = stateTimer / flapDuration;
        
        // ค่อยๆ ลดความสูงลง
       // agent.baseOffset = Mathf.Lerp(takeoffHeight, 0, landingProgress);

        if (landingProgress >= 2)
        {
            ReturnToIdle();
        }
    }

    private void GenerateNewTarget()
    {
        FlightCount++;
        float randomX = Random.Range(minX, maxX);
        float randomZ = Random.Range(minZ, maxZ);
        
        // สร้างตำแหน่งเป้าหมายใหม่
        currentTarget = new Vector3(randomX, 0, randomZ);
        
        // สั่งให้ NavMeshAgent เคลื่อนที่ไปยังเป้าหมาย
        agent.SetDestination(currentTarget);
    }

    private void UpdateFlyAttack()
    {
        // สั่งให้ NavMeshAgent เคลื่อนที่ไปยังเป้าหมาย
        agent.isStopped = false;
        agent.SetDestination(player.transform.position);
        
        if (ShouldLandFlyAttack())
        {
            StartLanding();
        }
    }

    private void UpdateFlameAttack()
    {
        
    }
    
    // State Transition Methods
    private void StartTakeoff()
    {
        
        //animator.ResetTrigger();
        if (!isFlyingTowardPlayer)
        {
            animator.SetTrigger("Scream2");
        }

        dragonHitbox.enabled = false;
        currentState = DragonState.Takeoff;
        stateTimer = 0;
        animator.SetTrigger(TAKEOFF_TRIGGER);
        
    }

    private void StartFlapping()
    {
        currentState = DragonState.Flapping;
        stateTimer = 0;
        flapCount = 0;
        animator.SetTrigger(FLAP_TRIGGER);
    }

    private void StartFlameAttack()
    {
        
    }
    private void StartGliding()
    {
        stateTimer = 0;
        agent.ResetPath();
        currentState = DragonState.Gliding;
        animator.SetTrigger(GLIDE_TRIGGER);
        agent.speed = glideSpeed;
        GenerateNewTarget(); // สุ่มเป้าหมายแรกทันทีที่เริ่ม Gliding
    }
    private void StartFlyAttack()
    {
        agent.ResetPath();
        currentState = DragonState.FlyAttack;
        agent.isStopped = true;
        animator.SetTrigger("FlyAttack");
        agent.speed = glideSpeed;
        //GenerateNewTarget(); // สุ่มเป้าหมายแรกทันทีที่เริ่ม Gliding
    }

    private void StartLanding()
    {
        dragonHitbox.enabled = true;
        agent.isStopped = true;
        FlightCount = 0;
        currentState = DragonState.Landing;
        stateTimer = 0;
        isFlyingTowardPlayer = false;
        animator.SetTrigger(LAND_TRIGGER);
        
       
        // หยุด NavMeshAgent
        agent.ResetPath();
    }
    

    private void ReturnToIdle()
    {
        currentState = DragonState.GroundIdle;
        agent.speed = defaultSpeed;
        stateTimer = 0;
        currentTarget = Vector3.zero;
    }

    private bool ShouldStartFlying()
    {
        Transform playerTransform = GameObject.FindWithTag("Player").transform;
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        return distanceToPlayer > 20f;  // เริ่มบินเมื่อผู้เล่นเข้าใกล้ในระยะ 20 หน่วย
       
        // ใส่เงื่อนไขการเริ่มบินตามที่ต้องการ
        //return false;
    }

    private bool ShouldStartFlameAttack()
    {
        return attackCount >= 4;
    }
    
    private bool ShouldLand()
    {
        if (stateTimer >= 8)
        {
            return true;
        }
        return FlightCount >= 4;
        // ใส่เงื่อนไขการลงจอดตามที่ต้องการ
        // return false;
    }
    
    

    private bool hasTriggered70 = false;
    private bool hasTriggered30 = false;

    private bool ShouldStartTakeOff()
    {
        float currentHealth = enemyHealth.GetCurrentHealth();
        float maxHealth = enemyHealth.GetCurrentMaxHealth();
        bool isFreeze = enemyHealth.IsFreeze();
        if (currentHealth < maxHealth * 0.70f && !hasTriggered70 && !isFreeze)
        {
            hasTriggered70 = true; // ป้องกันการเรียกซ้ำ
            return true;
        }

        if (currentHealth < maxHealth * 0.35f && !hasTriggered30 && !isFreeze)
        {
            hasTriggered30 = true; // ป้องกันการเรียกซ้ำ
            return true;
        }

        return false; // ไม่เข้าเงื่อนไข
    }
    private bool ShouldLandFlyAttack()
    {
       
        Transform playerTransform = GameObject.FindWithTag("Player").transform;
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        return distanceToPlayer < 10f;  // เริ่มบินเมื่อผู้เล่นเข้าใกล้ในระยะ 20 หน่วย
    }
    public void ForceFly()
    {
        if(currentState == DragonState.GroundIdle)
        {
            StartTakeoff();
        }
    }

    public void ForceLand()
    {
        if(currentState == DragonState.Gliding)
        {
            StartLanding();
        }
    }
}