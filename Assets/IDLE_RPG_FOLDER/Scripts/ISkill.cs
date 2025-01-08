using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    void UseSkill();
    bool IsOnCooldown();
    float GetCooldownTime();
    float GetCooldownPercentage();
    float GetRemainingCooldownTime();
}