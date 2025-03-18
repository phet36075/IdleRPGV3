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

[CreateAssetMenu(fileName = "New Item", menuName = "Create SO_Item", order = 4) ]
public class SO_Item : ScriptableObject
{
    public Sprite icon;
    public string id;
    public string itemName;
    public string description;
    public int maxStack;
    public ItemType itemType;
    [Header("In Game Object")]
    public GameObject gamePrefab;
    
    public virtual void Use()
    {
        
        Debug.Log($"Using {itemName}");
        
    }
}
