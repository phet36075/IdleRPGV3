using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class CheckRange : Conditional
{
    protected UnityEngine.AI.NavMeshAgent navMeshAgent;
    
    public SharedFloat arriveDistance = 0.2f;
    public SharedTransform target;
    public override void OnAwake()
    {
        
    }

    public override void OnStart()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }
   
    // Update is called once per frame
    public override TaskStatus OnUpdate()
    {
      
        if (HasArrived())
        {
            navMeshAgent.isStopped = true;
             return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
    
    private bool HasArrived()
    {
        // The path hasn't been computed yet if the path is pending.
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.Value.position);
            return distanceToTarget <= arriveDistance.Value;
        }

        return false;
    }
}
