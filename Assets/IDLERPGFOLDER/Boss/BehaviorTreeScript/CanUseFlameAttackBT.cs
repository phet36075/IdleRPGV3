using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class CanUseFlameAttackBT : Conditional
{
    public SharedBool isCanUseFlameAttack = false;
    private bool possibleBool;

    public override TaskStatus OnUpdate()
    {
        
        if (isCanUseFlameAttack.Value == true)
        {
            isCanUseFlameAttack.Value = false;
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
  
}
