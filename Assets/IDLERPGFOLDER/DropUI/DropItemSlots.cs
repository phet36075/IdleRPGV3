using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropItemSlots : MonoBehaviour
{
    public TextMeshProUGUI itemName;
    public Image itemIcon;
    public void HandleStatsSlots(SO_Item item)
    {
        itemIcon.sprite = item.icon;
        itemName.text = item.itemName;
    }
}
