using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyItemDrop : MonoBehaviour
{
    [Header("Item Templates")]
    public SO_Item[] itemTemplatesArmor; // ใส่ SO_Item ต้นแบบที่สร้างไว้ใน editor

    [Header("Generated Items")]
    public List<SO_Item> generatedItems = new List<SO_Item>();
    private PlayerManager _playerManager;
    private ShowDropItem showDropItem;
    public void Start()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
        showDropItem = FindAnyObjectByType<ShowDropItem>();
    }

    public void DropItem()
    {
        GenerateRandomItems(1);
        _playerManager.GetComponent<ItemPicker>().inventory.AddItem(generatedItems[0],1);
        showDropItem.ShowDrop(generatedItems[0],1);
        
        generatedItems.Clear();
    }
    
    public void GenerateRandomItems(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // สุ่มเลือก template
            int randomIndex = Random.Range(0, itemTemplatesArmor.Length);
            
            // สร้างไอเท็มใหม่จาก template และสุ่ม stats
            SO_Item newItem = itemTemplatesArmor[randomIndex].CreateInstance();
            
            // เพิ่มเข้าลิสต์
            generatedItems.Add(newItem);
            
            // แสดงผลลัพธ์
            Debug.Log($"Generated {newItem.itemName} - Health: {newItem.bonusHealth}, Defense: {newItem.bonusDefense}");
            
            newItem.UpdateDescription();
        }
    }
}
