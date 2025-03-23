using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropItemSlots : MonoBehaviour
{
    public TextMeshProUGUI itemName,amountText;
    public Image itemIcon;
    
    public void HandleStatsSlots(SO_Item item,int amount = 1)
    {
        itemIcon.sprite = item.icon;
        itemName.text = item.itemName;
        amountText.text = "x " + amount;
    }
}
