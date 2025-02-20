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
    
    [Header("Random Flight Settings")]
    [SerializeField] private float minX = -50f;
    [SerializeField] private float maxX = 50f;
    [SerializeField] private float minZ = -50f;
    [SerializeField] private float maxZ = 50f;
    [SerializeField] private int FlightCount = 0;
    [Header("Components")]
    [SerializeField] private Animator animator;
    private NavMeshAgent agent;
    private Transform player;
    [Header("Enemy Settings")]
    [SerializeField] private float detectionRange = 10f;  // ระยะการตรวจจับ Player
    [SerializeField] private float attackRange = 2f;      // ระยะโจมตี
    [SerializeField] private float attackCooldown = 2f;   // ระยะเวลารอระหว่างการโจมตี
    
    private float nextAttackTime;
    private bool isAttacking;

    private bool isFlyingTowardPlayer;
    // Animation parameter names
    private const string TAKEOFF_TRIGGER = "Takeoff";
    private const string FLAP_TRIGGER = "Flap";
    private const string GLIDE_TRIGGER = "Glide";
    private const string LAND_TRIGGER = "Land";

    // State machine
    private enum DragonState
    {
        GroundIdle,
        FlyAttack,
        Takeoff,
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
        ForceFly();
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
        
        // คำนวณระยะห่างระหว่างศัตรูกับ Player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // ถ้าอยู่ในระยะตรวจจับ
        if (distanceToPlayer <= detectionRange)
        {
            // ถ้าอยู่ในระยะโจมตี
            if (distanceToPlayer <= attackRange)
            {
                // หยุดเดิน
                agent.isStopped = true;
                
                // โจมตีถ้าหมดเวลา Cooldown
                if (Time.time >= nextAttackTime)
                {
                    //Attack();
                }
            }
            else
            {
                // เดินตาม Player
                float speed = agent.velocity.magnitude; // คำนวณความเร็ว
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
        
        
        
        if (ShouldStartFlying())
        {
            isFlyingTowardPlayer = true;
            StartTakeoff();
        }
    }

    private void UpdateTakeoff()
    {
        stateTimer += Time.deltaTime;
      //  float targetHeight = Mathf.Lerp(0, takeoffHeight, stateTimer / flapDuration);
        
        // ปรับ baseOffset ของ NavMeshAgent แทนการเปลี่ยน y position โดยตรง
        //agent.baseOffset = targetHeight;

        if (stateTimer >= flapDuration)
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
    // State Transition Methods
    private void StartTakeoff()
    {
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

    private void StartGliding()
    {
        currentState = DragonState.Gliding;
        animator.SetTrigger(GLIDE_TRIGGER);
        agent.speed = glideSpeed;
        GenerateNewTarget(); // สุ่มเป้าหมายแรกทันทีที่เริ่ม Gliding
    }
    private void StartFlyAttack()
    {
        currentState = DragonState.FlyAttack;
        agent.isStopped = true;
        animator.SetTrigger("FlyAttack");
        agent.speed = glideSpeed;
        //GenerateNewTarget(); // สุ่มเป้าหมายแรกทันทีที่เริ่ม Gliding
    }

    private void StartLanding()
    {
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

    private bool ShouldLand()
    {
        
        
        
        return FlightCount >= 4;
        // ใส่เงื่อนไขการลงจอดตามที่ต้องการ
        // return false;
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