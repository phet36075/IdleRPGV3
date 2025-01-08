using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPotionSO : SO_Item
{
    public int healAmount = 50;

    public override void Use()
    {
        base.Use();
        Debug.Log($"Healing for {healAmount} HP");
        // เพิ่มโค้ดสำหรับการรักษา HP ของตัวละคร
    }
}
