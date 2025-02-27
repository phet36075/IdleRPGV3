using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class PlayerSeek : Action
{
    public SharedFloat speed = 10;
  
    public SharedFloat angularSpeed = 120;
    
    public SharedFloat arriveDistance = 0.2f;
    public SharedTransform target;
    protected UnityEngine.AI.NavMeshAgent navMeshAgent;
 
    public string targetTag;
    
    public override void OnAwake()
    {
      
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }
    public override void OnStart()
    {
        navMeshAgent.speed = speed.Value;
        navMeshAgent.angularSpeed = angularSpeed.Value;
        navMeshAgent.isStopped = false;
       
    }
    public override TaskStatus OnUpdate()
    {
        if (target != null)
        {
            target.Value = GameObject.FindGameObjectWithTag(targetTag).transform;
        }
        RotateTowardsTarget();
        navMeshAgent.SetDestination(target.Value.position);
        if (HasArrived())
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }
    private bool HasArrived()
    {
        // The path hasn't been computed yet if the path is pending.
        float remainingDistance;
        if (navMeshAgent.pathPending) {
            remainingDistance = float.PositiveInfinity;
        } else {
            remainingDistance = navMeshAgent.remainingDistance;
        }

        return remainingDistance <= arriveDistance.Value;
    }
    
    void RotateTowardsTarget()
    {
        Vector3 direction = (target.Value.position - transform.position).normalized;
        //Debug.Log("Direction: " + direction);  // ตรวจสอบทิศทางการหมุน
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        //Debug.Log("Target Rotation: " + lookRotation.eulerAngles);  // ตรวจสอบการหมุนที่ควรจะเป็น
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3);
    }
}
