using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class FlyFlameBT : Action
{
    [Header("Random Flight Settings")]
    [SerializeField] private float minX = -50f;
    [SerializeField] private float maxX = 50f;
    [SerializeField] private float minZ = -50f;
    [SerializeField] private float maxZ = 50f;
    [SerializeField] private int FlightCount = 0;
    protected UnityEngine.AI.NavMeshAgent navMeshAgent;
    private float stateTimer;
    private Vector3 currentTarget;
    private const float TARGET_REACHED_THRESHOLD = 10f;
    public SharedFloat speed = 5;
    public SharedFloat angularSpeed = 80;
    public override void OnAwake()
    {
       
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
       
    }

    public override void OnStart()
    {
        navMeshAgent.speed = speed.Value;
        navMeshAgent.angularSpeed = angularSpeed.Value;
        navMeshAgent.ResetPath();
    }
    public override TaskStatus OnUpdate()
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
            FlightCount = 0;
            stateTimer = 0;
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }
    
    
    private void GenerateNewTarget()
    {
        FlightCount++;
        float randomX = Random.Range(minX, maxX);
        float randomZ = Random.Range(minZ, maxZ);
        
        // สร้างตำแหน่งเป้าหมายใหม่
        currentTarget = new Vector3(randomX, 0, randomZ);
        
        // สั่งให้ NavMeshAgent เคลื่อนที่ไปยังเป้าหมาย
        navMeshAgent.SetDestination(currentTarget);
    }
    private bool ShouldLand()
    {
        if (stateTimer >= 12)
        {
            return true;
        }
        return FlightCount >= 4;
        // ใส่เงื่อนไขการลงจอดตามที่ต้องการ
        // return false;
    }
}
