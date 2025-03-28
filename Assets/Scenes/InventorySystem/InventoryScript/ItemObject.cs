using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class ItemObject : MonoBehaviour
{
    public SO_Item item;
    public int amount = 1;
    public TextMeshProUGUI amountText;
    private ShowDropItem showDropItem;

    public void Start()
    {
        showDropItem = FindAnyObjectByType<ShowDropItem>();
    }

    public void SetAmount(int newAmount)
    {
        amount = newAmount;
        amountText.text = amount.ToString();
    }

    public void RandomAmount()
    {
        amount = Random.Range(1, item.maxStack + 1);
        amountText.text = amount.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Add Item
            other.GetComponent<ItemPicker>().inventory.AddItem(item, amount);
            showDropItem.ShowDrop(item,amount);
            Destroy(gameObject);
        }
    }

}