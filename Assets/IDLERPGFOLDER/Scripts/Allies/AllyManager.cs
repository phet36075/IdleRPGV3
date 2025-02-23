using UnityEngine;

public class AllyManager : MonoBehaviour
{
    public AllyRangedCombat rangedAlly;    // Reference to ranged ally
    public AllyMeleeCombat meleeAlly;      // Reference to melee ally
    
    public void CallAllAllies()
    {
        // เรียกใช้ ally ทั้งสองตัว
        if (rangedAlly != null)
        {
            rangedAlly.CallAlliesToAttack();
        }
        
        if (meleeAlly != null)
        {
            //meleeAlly.StartAutonomousCombat();
        }
    }

    public void StopAllAllies()
    {
        // หยุดการโจมตีของ ally ทั้งสองตัว
        if (rangedAlly != null)
        {
            rangedAlly.AllyisAttacking = false;
            rangedAlly.CancelInvoke("ContinueAttacking");
        }
        
        if (meleeAlly != null)
        {
           // meleeAlly.AllyisAttacking = false;
          //  meleeAlly.CancelInvoke("ContinueAttacking");
        }
    }
}