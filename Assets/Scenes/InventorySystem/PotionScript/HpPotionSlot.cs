using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HpPotionSlot : MonoBehaviour, IPointerClickHandler
{
    public Inventory inventory; 
    public InventorySlot inventorySlot; 
    public GameObject potionImage; 
    public TextMeshProUGUI stackText;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PlayerProperty playerProperty;
    public Slider healthThresholdSlider;
    void Update()
    {
        if (playerManager.GetCurrentHealth() <= playerProperty.maxHealth * healthThresholdSlider.value && healthThresholdSlider.value != 1)
        {
            foreach (InventorySlot slot in inventory.inventorySlots)
            {
                if (slot.item != null && slot.item.itemName == "Healing Potion" && slot.stack > 0)
                {
                    slot.UseItem();
                    inventory.HideItemDetail();
                    break;
                }
            }
        }
        
        
        if (inventory != null && inventory.inventorySlots != null)
        {
            int PotionStack = 0; // เก็บ stack ของ Healing Potion
            bool hasPotion = false;

            foreach (InventorySlot slot in inventory.inventorySlots)
            {
                if (slot.item != null && slot.item.itemName == "Healing Potion" && slot.stack > 0)
                {
                    hasPotion = true;
                    PotionStack = slot.stack; // อัปเดต stack
                    break;
                }
            }

            potionImage.SetActive(hasPotion);

            if (stackText != null)
            {
                if (hasPotion && PotionStack > 0)
                {
                    stackText.text = PotionStack.ToString(); // แสดง stack
                    stackText.gameObject.SetActive(true);
                }
                else
                {
                    stackText.gameObject.SetActive(false); 
                }
            }
        }
        else
        {
            potionImage.SetActive(false);
            if (stackText != null)
            {
                stackText.gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && inventory != null && inventory.inventorySlots != null)
        {
            foreach (InventorySlot slot in inventory.inventorySlots)
            {
                if (slot.item != null && slot.item.itemName == "Healing Potion" && slot.stack > 0)
                {
                    slot.UseItem();
                    inventory.HideItemDetail();
                    break;
                }
            }
        }
    }
}