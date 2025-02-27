using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class IsHavingTarget : Conditional
{
    public SharedTransform target;
    public bool isHaving;
    public override TaskStatus OnUpdate()
    {
        if (isHaving)
        {
            if (target.Value == null)
            {
                return TaskStatus.Success;
            }
        }

        if (!isHaving)
        {
            if (target.Value != null)
            {
                return TaskStatus.Success;
            }
        }

        return TaskStatus.Failure;
    }
}
