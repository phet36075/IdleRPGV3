using UnityEngine;

public class StatusEffectTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StatusEffectUI statusEffectUI;
    
    [Header("Test Status Effects")]
    [SerializeField] private Sprite poisonIcon;
    [SerializeField] private Sprite stunIcon;
    [SerializeField] private Sprite burnIcon;

    void Update()
    {
        // กด 1 เพื่อเพิ่ม Poison effect
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            statusEffectUI.AddStatusEffect("Poison", poisonIcon, 5f);
        }
        
        // กด 2 เพื่อเพิ่ม Stun effect
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            statusEffectUI.AddStatusEffect("Holy", stunIcon, 3f);
        }
        
        // กด 3 เพื่อเพิ่ม Burn effect
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            statusEffectUI.AddStatusEffect("Burn", burnIcon, 4f);
        }
    }
}