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
    public int basePen;
    public int baseHealthRegen;
    public int baseDamage;
    public int baseCritChance;
    public int baseMana;
    public int baseManaRegen;
    public int baseCritDamage;
    [Header("Random Stat Ranges")]
    // สำหรับกำหนดช่วงการสุ่ม
    public StatRange healthRange = new StatRange(0, 10);
    public StatRange defenseRange = new StatRange(0, 5);
    public StatRange penRange = new StatRange(0, 5);
    public StatRange healthRegenRange = new StatRange(0, 5);
    public StatRange damageRange = new StatRange(0, 5);
    public StatRange critChanceRange = new StatRange(0, 5);
    public StatRange manaRange = new StatRange(0, 5);
    public StatRange manaRegenRange = new StatRange(0, 5);
    public StatRange critDamageRange = new StatRange(0, 5);
    // สำหรับค่าที่ถูกสุ่มแล้ว
    [HideInInspector] public int bonusHealth;
    [HideInInspector] public int bonusDefense;
    [HideInInspector] public int bonusPen;
    [HideInInspector] public int bonusHealthRegen;
    [HideInInspector] public int bonusDamage;
    [HideInInspector] public int bonusCritChance;
    [HideInInspector] public int bonusMana;
    [HideInInspector] public int bonusManaRegen;
    [HideInInspector] public int bonusCritDamage;
    public virtual void Use()
    {
        
        Debug.Log($"Using {itemName}");
        
    }
    
    // เมธอดสำหรับสร้างไอเท็มใหม่ที่มีการสุ่ม stats
  public SO_Item CreateInstance()
{
    SO_Item newItem = Instantiate(this);
    newItem.GenerateRandomRarity();
    newItem.GenerateRandomStats();
    newItem.ApplyRarityBonus();
    newItem.UpdateDescription();
    return newItem;
}

// เมธอดสำหรับสุ่ม rarity
public void GenerateRandomRarity()
{
    // สุ่ม rarity ตามโอกาสที่กำหนด
    float randomValue = Random.value; // สุ่มค่าระหว่าง 0-1
    
    if (randomValue < 0.5f)
    {
        rarity = ItemRarity.Common;
    }
    else if (randomValue < 0.75f)
    {
        rarity = ItemRarity.Uncommon;
    }
    else if (randomValue < 0.9f)
    {
        rarity = ItemRarity.Rare;
    }
    else if (randomValue < 0.98f)
    {
        rarity = ItemRarity.Epic;
    }
    else
    {
        rarity = ItemRarity.Legendary;
    }
}

// เมธอดสำหรับสุ่ม stats 2 อย่างจากทั้งหมด (ซ้ำได้)
public void GenerateRandomStats()
{
    // รีเซ็ตค่าโบนัสทั้งหมดก่อน
    bonusHealth = 0;
    bonusDefense = 0;
    bonusPen = 0;
    bonusHealthRegen = 0;
    bonusDamage = 0;
    bonusCritChance = 0;
    bonusMana = 0;
    bonusManaRegen = 0;
    bonusCritDamage = 0;
    
    // สร้าง array ของฟังก์ชั่นที่จะใช้สุ่ม stats แต่ละแบบ
    System.Action[] statGenerators = new System.Action[]
    {
        () => bonusHealth = baseHealth + healthRange.GetRandomValue(),
        () => bonusDefense = baseDefense + defenseRange.GetRandomValue(),
        () => bonusPen = basePen + penRange.GetRandomValue(),
        () => bonusHealthRegen = baseHealthRegen + healthRegenRange.GetRandomValue(),
        () => bonusDamage = baseDamage + damageRange.GetRandomValue(),
        () => bonusCritChance = baseCritChance + critChanceRange.GetRandomValue(),
        () => bonusMana = baseMana + manaRange.GetRandomValue(),
        () => bonusManaRegen = baseManaRegen + manaRegenRange.GetRandomValue(),
        () => bonusCritDamage = baseCritDamage + critDamageRange.GetRandomValue()
    };
    
    // สุ่มเลือก stats 2 อย่าง (ซ้ำได้)
    for (int i = 0; i < 2; i++)
    {
        int randomIndex = Random.Range(0, statGenerators.Length);
        statGenerators[randomIndex].Invoke();
    }
}

// เมธอดสำหรับเพิ่มค่าตามความหายาก
public void ApplyRarityBonus()
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

    // ปรับค่าโบนัสตามความหายากเฉพาะที่มีการสุ่ม (ค่ามากกว่า 0)
    if (bonusHealth > 0)
        bonusHealth = Mathf.RoundToInt(bonusHealth * rarityMultiplier);
    
    if (bonusDefense > 0)
        bonusDefense = Mathf.RoundToInt(bonusDefense * rarityMultiplier);
    
    if (bonusPen > 0)
        bonusPen = Mathf.RoundToInt(bonusPen * rarityMultiplier);
        
    if (bonusHealthRegen > 0)
        bonusHealthRegen = Mathf.RoundToInt(bonusHealthRegen * rarityMultiplier);
        
    if (bonusDamage > 0)
        bonusDamage = Mathf.RoundToInt(bonusDamage * rarityMultiplier);
        
    if (bonusCritChance > 0)
        bonusCritChance = Mathf.RoundToInt(bonusCritChance * rarityMultiplier);
        
    if (bonusMana > 0)
        bonusMana = Mathf.RoundToInt(bonusMana * rarityMultiplier);
        
    if (bonusManaRegen > 0)
        bonusManaRegen = Mathf.RoundToInt(bonusManaRegen * rarityMultiplier);
    
    if (bonusCritDamage > 0)
        bonusCritDamage = Mathf.RoundToInt(bonusCritDamage * rarityMultiplier);
}

public void UpdateDescription()
{
    // เริ่มด้วยการแสดง rarity
    string rarityColor = GetRarityColorHex();
    string desc = "<color=" + rarityColor + ">" + rarity.ToString() + "</color>\n";
    
    // แสดงข้อมูล stats ที่ถูกสุ่ม
    if (bonusHealth > 0)
    {
        desc += "Health: " + baseHealth.ToString() + " + " + bonusHealth.ToString() + "\n";
    }
    
    if (bonusDefense > 0)
    {
        desc += "Defense: " + baseDefense.ToString() + " + " + bonusDefense.ToString() + "\n";
    }
    
    if (bonusPen > 0)
    {
        desc += "Pen: " + basePen.ToString() + " + " + bonusPen.ToString() + "\n";
    }
    
    if (bonusHealthRegen > 0)
    {
        desc += "Health Regen: " + baseHealthRegen.ToString() + " + " + bonusHealthRegen.ToString() + "\n";
    }
    
    if (bonusDamage > 0)
    {
        desc += "Damage: " + baseDamage.ToString() + " + " + bonusDamage.ToString() + "\n";
    }
    
    if (bonusCritChance > 0)
    {
        desc += "Crit Chance: " + baseCritChance.ToString() + "% + " + bonusCritChance.ToString() + "%\n";
    }
    
    if (bonusMana > 0)
    {
        desc += "Mana: " + baseMana.ToString() + " + " + bonusMana.ToString() + "\n";
    }
    
    if (bonusManaRegen > 0)
    {
        desc += "Mana Regen: " + baseManaRegen.ToString() + " + " + bonusManaRegen.ToString() + "\n";
    }
    
    if (bonusCritDamage > 0)
    {
        desc += "Crit Damage: " + baseCritDamage.ToString() + " + " + bonusCritDamage.ToString() + "\n";
    }
    
    // ตัดเครื่องหมาย \n ตัวสุดท้ายออกถ้ามี
    if (desc.EndsWith("\n"))
    {
        desc = desc.Substring(0, desc.Length - 1);
    }
    
    description = desc;
}

// เมธอดช่วยสำหรับเลือกสีตาม rarity
private string GetRarityColorHex()
{
    switch (rarity)
    {
        case ItemRarity.Common:
            return "#FFFFFF"; // ขาว
        case ItemRarity.Uncommon:
            return "#00FF00"; // เขียว
        case ItemRarity.Rare:
            return "#0080FF"; // ฟ้า
        case ItemRarity.Epic:
            return "#8000FF"; // ม่วง
        case ItemRarity.Legendary:
            return "#FF8000"; // ส้ม
        default:
            return "#FFFFFF";
    }
}
}
