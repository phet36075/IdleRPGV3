using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using Unity.VisualScripting;
using UnityEngine;
public enum ItemType
{
    Default,
    Weapon,
    Hat,
    Armor,
    Boot,
    Ring,
    Consumable,
    Skill
    
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
[System.Serializable]
public class StatRange
{
    public int minValue;
    public int maxValue;

    public StatRange(int min, int max)
    {
        minValue = min;
        maxValue = max;
    }

    public int GetRandomValue()
    {
        return Random.Range(minValue, maxValue + 1);
    }
}
[CreateAssetMenu(fileName = "New Item", menuName = "Create SO_Item", order = 4) ]
public class SO_Item : ScriptableObject
{
    public Sprite icon;
    public string id;
    public string itemName;
    [TextArea(1, 10)]
    public string description;
    public int maxStack;
    public ItemType itemType;
    public ItemRarity rarity = ItemRarity.Common;
    
    [Header("In Game Object")]
    public GameObject gamePrefab;

    [Header("Base Stats")]
    // สำหรับกำหนดค่าเริ่มต้น
    public int baseHealth;
    public int baseDefense;
    [Header("Random Stat Ranges")]
    // สำหรับกำหนดช่วงการสุ่ม
    public StatRange healthRange = new StatRange(0, 10);
    public StatRange defenseRange = new StatRange(0, 5);

    // สำหรับค่าที่ถูกสุ่มแล้ว
    [HideInInspector] public int bonusHealth;
    [HideInInspector] public int bonusDefense;
    public virtual void Use()
    {
        
        Debug.Log($"Using {itemName}");
        
    }
    
    // เมธอดสำหรับสร้างไอเท็มใหม่ที่มีการสุ่ม stats
    public SO_Item CreateInstance()
    {
        SO_Item newItem = Instantiate(this);
        newItem.GenerateRandomStats();
        return newItem;
    }

    // เมธอดสำหรับสุ่ม stats
    public void GenerateRandomStats()
    {
        bonusHealth = baseHealth + healthRange.GetRandomValue();
        bonusDefense = baseDefense + defenseRange.GetRandomValue();
        
        // เพิ่มค่าตามความหายาก
        ApplyRarityBonus();
    }
    public void UpdateDescription()
    {
        description = "Health: " + baseHealth.ToString() + " + " + bonusHealth.ToString();

        // You can customize the format of the description here
        // For example, you could include other stats:
        // description = $"Health: {baseHealth} | Defense: {baseDefense}";
    }
    // เพิ่มโบนัสตาม rarity
    private void ApplyRarityBonus()
    {
        float rarityMultiplier = 1.0f;
        
        switch (rarity)
        {
            case ItemRarity.Common:
                rarityMultiplier = 1.0f;
                break;
            case ItemRarity.Uncommon:
                rarityMultiplier = 1.2f;
                break;
            case ItemRarity.Rare:
                rarityMultiplier = 1.5f;
                break;
            case ItemRarity.Epic:
                rarityMultiplier = 2.0f;
                break;
            case ItemRarity.Legendary:
                rarityMultiplier = 3.0f;
                break;
        }

        bonusHealth = Mathf.RoundToInt(bonusHealth * rarityMultiplier);
        bonusDefense = Mathf.RoundToInt(bonusDefense * rarityMultiplier);
    }
}
