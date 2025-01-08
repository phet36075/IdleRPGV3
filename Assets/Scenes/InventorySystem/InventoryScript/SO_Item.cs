using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Create new Item", order = 4) ]
public class SO_Item : ScriptableObject
{
    public Sprite icon;
    public string id;
    public string itemName;
    public string description;
    public int maxStack;

    [Header("In Game Object")]
    public GameObject gamePrefab;
    
    public virtual void Use()
    {
        Debug.Log($"Using {itemName}");
        
    }
}
