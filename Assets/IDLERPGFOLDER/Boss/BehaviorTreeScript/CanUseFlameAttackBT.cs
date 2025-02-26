using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class CanUseFlameAttackBT : Conditional
{
    public SharedBool isCanUseFlameAttack = false;
    private bool possibleBool;
    public override void OnAwake()
    {
        
       
    }

    public override TaskStatus OnUpdate()
    {

      //  isCanUseFlameAttack.Value = possibleBool;
        

        if (isCanUseFlameAttack.Value == true)
        {
            isCanUseFlameAttack.Value = false;
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
