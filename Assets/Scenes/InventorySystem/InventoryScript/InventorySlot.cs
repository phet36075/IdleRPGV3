using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class InventorySlot : MonoBehaviour, IDropHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("Inventory Detail")]
    public Inventory inventory;

    [Header("Slot Detail")]

    public SO_Item item;
    public int stack;

    [Header("UI")]
    public Color emptyColor;
    public Color itemColor;
    public Image icon;
    public TextMeshProUGUI stackText;

    [Header("Drag and Drop")]
    public int siblingIndex;
    public RectTransform draggable;
    Canvas canvas;
    CanvasGroup canvasGroup;
    private PlayerManager playerManager;

    //All in one button




    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        siblingIndex = transform.GetSiblingIndex();
        playerManager = FindObjectOfType<PlayerManager>();

        //5 Equipment
        if (inventory.myEqImage != null)
        {
            EventTrigger trigger = inventory.myEqImage.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = inventory.myEqImage.gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) => { OnEqImageClick(); });
            trigger.triggers.Add(entry);
        }
        if (inventory.myEqImageHat != null)
        {
            EventTrigger trigger = inventory.myEqImageHat.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = inventory.myEqImageHat.gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) => { OnEqImageClickHat(); });
            trigger.triggers.Add(entry);
        }

        if (inventory.myEqImageArmor != null)
        {
            EventTrigger trigger = inventory.myEqImageArmor.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = inventory.myEqImageArmor.gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) => { OnEqImageClickArmor(); });
            trigger.triggers.Add(entry);
        }

        if (inventory.myEqImageBoot != null)
        {
            EventTrigger trigger = inventory.myEqImageBoot.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = inventory.myEqImageBoot.gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) => { OnEqImageClickBoot(); });
            trigger.triggers.Add(entry);
        }

        if (inventory.myEqImageRing != null)
        {
            EventTrigger trigger = inventory.myEqImageRing.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = inventory.myEqImageRing.gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) => { OnEqImageClickRing(); });
            trigger.triggers.Add(entry);
        }

    }

    #region Drag and Drop Methods
    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
        inventory.SetLayoutControlChild(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        draggable.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
        draggable.anchoredPosition = Vector2.zero;
        transform.SetSiblingIndex(siblingIndex);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            InventorySlot slot = eventData.pointerDrag.GetComponent<InventorySlot>();
            if (slot != null)
            {
                if (slot.item == item)
                {
                    //merge
                    MergeThisSlot(slot);
                }
                else
                {
                    //swap
                    SwapSlot(slot);
                }
            }
        }



    }

    public void OnPointerClick(PointerEventData eventData)
    {

        
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (item == inventory.EMPTY_ITEM)
                return;

            inventory.currentSelectedSlot = this;

            inventory.ShowItemDetail(this);

            Debug.Log($"Item clicked: {item.itemName}");
            Debug.Log($"Item ID: {item.id}");
            Debug.Log($"Item Description: {item.description}");

            // อัปเดตปุ่ม Equip/Unequip
            UpdateEquipButton();
        }
    }


    //5 equipment
    public void OnEqImageClick()
    {

        if (inventory.equippedItemSlot != null) // ตรวจสอบว่ามีอาวุธติดตั้งอยู่หรือไม่
        {
            inventory.currentSelectedSlot = inventory.equippedItemSlot; // กำหนดให้เป็น Slot เดียวกันกับที่ติดตั้งอาวุธ
            inventory.ShowItemDetail(inventory.equippedItemSlot); // แสดงรายละเอียดของไอเทม
            inventory.equippedItemSlot.UpdateEquipButton(); // อัปเดตปุ่ม Equip/Unequip
        }
        

    }
    
    public void OnEqImageClickHat()
    {

        if (inventory.equippedHatSlot != null) // ตรวจสอบว่ามีอาวุธติดตั้งอยู่หรือไม่
        {
            inventory.currentSelectedSlot = inventory.equippedHatSlot; // กำหนดให้เป็น Slot เดียวกันกับที่ติดตั้ง
            inventory.ShowItemDetail(inventory.equippedHatSlot); // แสดงรายละเอียดของไอเทม
            inventory.equippedHatSlot.UpdateEquipButton(); // อัปเดตปุ่ม Equip/Unequip
        }


    }

    public void OnEqImageClickArmor()
    {

        if (inventory.equippedArmorSlot != null) // ตรวจสอบว่ามีอาวุธติดตั้งอยู่หรือไม่
        {
            inventory.currentSelectedSlot = inventory.equippedArmorSlot; // กำหนดให้เป็น Slot เดียวกันกับที่ติดตั้ง
            inventory.ShowItemDetail(inventory.equippedArmorSlot); // แสดงรายละเอียดของไอเทม
            inventory.equippedArmorSlot.UpdateEquipButton(); // อัปเดตปุ่ม Equip/Unequip
        }


    }

    public void OnEqImageClickBoot()
    {

        if (inventory.equippedBootSlot != null) // ตรวจสอบว่ามีอาวุธติดตั้งอยู่หรือไม่
        {
            inventory.currentSelectedSlot = inventory.equippedBootSlot; // กำหนดให้เป็น Slot เดียวกันกับที่ติดตั้ง
            inventory.ShowItemDetail(inventory.equippedBootSlot); // แสดงรายละเอียดของไอเทม
            inventory.equippedBootSlot.UpdateEquipButton(); // อัปเดตปุ่ม Equip/Unequip
        }


    }

    public void OnEqImageClickRing()
    {

        if (inventory.equippedRingSlot != null) // ตรวจสอบว่ามีอาวุธติดตั้งอยู่หรือไม่
        {
            inventory.currentSelectedSlot = inventory.equippedRingSlot; // กำหนดให้เป็น Slot เดียวกันกับที่ติดตั้ง
            inventory.ShowItemDetail(inventory.equippedRingSlot); // แสดงรายละเอียดของไอเทม
            inventory.equippedRingSlot.UpdateEquipButton(); // อัปเดตปุ่ม Equip/Unequip
        }


    }

    public void UpdateEquipButton()
    {
        /*
        if (inventory.equipButton == null || inventory.equipButton == null)
            return;

        if (inventory.IsItemEquipped(this))
        {
            inventory.equipButtonText.text = "Unequip";
            inventory.equipButton.onClick.RemoveAllListeners();
            //inventory.equipButton.onClick.AddListener(inventory.UnequipItem);
            inventory.equipButton.onClick.AddListener(Unequip);
        }
        else
        {
            inventory.equipButtonText.text = "Equip";
            inventory.equipButton.onClick.RemoveAllListeners();
            inventory.equipButton.onClick.AddListener(UseItem);
        }
        */
        if (inventory.equipButton == null || inventory.equipButtonText == null)
            return;

        bool isEquipped = false;

        // เช็คว่ากำลังเลือกไอเทมที่ติดตั้งอยู่หรือไม่
        if (item.itemType == ItemType.Weapon)
            isEquipped = (inventory.equippedItemSlot == this);
        else if (item.itemType == ItemType.Hat)
            isEquipped = (inventory.equippedHatSlot == this);
        else if (item.itemType == ItemType.Armor)
            isEquipped = (inventory.equippedArmorSlot == this);
        else if (item.itemType == ItemType.Boot)
            isEquipped = (inventory.equippedBootSlot == this);
        else if (item.itemType == ItemType.Ring)
            isEquipped = (inventory.equippedRingSlot == this);

        // อัปเดตปุ่ม
        if (isEquipped)
        {
            inventory.equipButtonText.text = "Unequip";
            inventory.equipButton.onClick.RemoveAllListeners();
            inventory.equipButton.onClick.AddListener(Unequip);
        }
        else
        {
            inventory.equipButtonText.text = "Equip";
            inventory.equipButton.onClick.RemoveAllListeners();
            inventory.equipButton.onClick.AddListener(UseItem);
        }
    }

    

    #endregion


    public void UseItem()
     {
         if (item.itemType == ItemType.Weapon)
         {
             inventory.equippedItemSlot = this; // ตั้งค่าช่องที่กำลังใช้อยู่



             if (inventory.myEqImage != null)
             {
                 inventory.myEqImage.sprite = item.icon;
                 inventory.myEqImage.color = Color.white; // ทำให้รูปไม่โปร่งใส
             }
             Debug.Log($"Equipped {item.itemName}");

             inventory.ShowItemName(item.itemName); // แสดงชื่อไอเทมใน UI
             if (item.itemName == "FireSword")
             {
                 playerManager.ChangeWeaponElement(ElementType.Fire);
             }
             else if (item.itemName == "WaterSword")
             {
                 playerManager.ChangeWeaponElement(ElementType.Water);
             }
             else if (item.itemName == "WindSword")
             {
                 playerManager.ChangeWeaponElement(ElementType.Wind);
             }
             else if (item.itemName == "EarthSword")
             {
                 playerManager.ChangeWeaponElement(ElementType.Earth);
             }
             else if (item.itemName == "LightSword")
             {
                 playerManager.ChangeWeaponElement(ElementType.Light);
             }
             else if (item.itemName == "DarkSword")
             {
                 playerManager.ChangeWeaponElement(ElementType.Dark);
             }
            UpdateEquipButton();
             return; // **ออกจากเมทอดทันที ไม่ลด stack**
         }
        else if (item.itemType == ItemType.Hat) //   Hat
        {
            inventory.equippedHatSlot = this;

            if (inventory.myEqImageHat != null)
            {
                inventory.myEqImageHat.sprite = item.icon;
                inventory.myEqImageHat.color = Color.white;
            }
            Debug.Log($"Equipped {item.itemName} (Hat)");

            inventory.ShowItemNameHat(item.itemName);
            UpdateEquipButton();
            return;
        }

        else if (item.itemType == ItemType.Armor) //   Armor
        {
            inventory.equippedArmorSlot = this;

            if (inventory.myEqImageArmor != null)
            {
                inventory.myEqImageArmor.sprite = item.icon;
                inventory.myEqImageArmor.color = Color.white;
            }
            Debug.Log($"Equipped {item.itemName} (Armor)");

            inventory.ShowItemNameArmor(item.itemName);
            UpdateEquipButton();
            return;
        }

        else if (item.itemType == ItemType.Boot) //   Boot
        {
            inventory.equippedBootSlot = this;

            if (inventory.myEqImageBoot != null)
            {
                inventory.myEqImageBoot.sprite = item.icon;
                inventory.myEqImageBoot.color = Color.white;
            }
            Debug.Log($"Equipped {item.itemName} (Boot)");

            inventory.ShowItemNameBoot(item.itemName);
            UpdateEquipButton();
            return;
        }

        else if (item.itemType == ItemType.Ring) //   Ring
        {
            inventory.equippedRingSlot = this;

            if (inventory.myEqImageRing != null)
            {
                inventory.myEqImageRing.sprite = item.icon;
                inventory.myEqImageRing.color = Color.white;
            }
            Debug.Log($"Equipped {item.itemName} (Ring)");

            inventory.ShowItemNameRing(item.itemName);
            UpdateEquipButton();
            return;
        }

        // ลด stack ตามปกติ
        stack = Mathf.Clamp(stack - 1, 0, item.maxStack);

         if (stack > 0)
         {
             CheckShowText();
             Debug.Log("Use Item");
             item.Use();

             // if (item.itemName == "Healing Potion")
             // {
             //     playerManager.Heal(110f);
             // }
             if (item.itemName == "Mana Potion")
             {
                 playerManager.RestoreMana(50f);
             }

             if (item.itemName == "Fire Skill Book")
             {

             }
         }
         else
         {
           
            inventory.RemoveItem(this);
         }
        
        inventory.ShowItemDetail(this);
        if (stack == 0)
        {
            inventory.HideItemDetail();

        }
        
    }

    public void Unequip()
    {
       
        if (inventory.currentSelectedSlot == null) return; // ป้องกัน null reference

        if (inventory.currentSelectedSlot == inventory.equippedItemSlot) // ตรวจสอบว่าช่องที่เลือกเป็นอันเดียวกับที่ใส่อุปกรณ์อยู่หรือไม่
        {
            if (inventory.currentSelectedSlot.item.itemType == ItemType.Weapon)
            {
                playerManager.ChangeWeaponElement(ElementType.None); // รีเซ็ตธาตุอาวุธ
            }

            // ลบรูปอาวุธใน UI
            inventory.myEqImage.sprite = null;
            inventory.myEqImage.color = new Color(0f, 0f, 0f, 177f / 255f);  // grey
            Debug.Log($"Unequipped {item.itemName}");
            inventory.ShowItemName(""); // ล้างชื่อไอเทมที่แสดงอยู่

            // ตั้งค่าช่องอุปกรณ์ให้เป็นว่าง
            inventory.equippedItemSlot = null;
            
        }
        else if (inventory.currentSelectedSlot == inventory.equippedHatSlot) //   Hat
        {
            inventory.myEqImageHat.sprite = null;
            inventory.myEqImageHat.color = new Color(0f, 0f, 0f, 177f / 255f);
            inventory.equippedHatSlot = null;
            Debug.Log($"Unequipped {item.itemName} (Hat)");
            inventory.ShowItemNameHat(""); // ล้างชื่อไอเทมที่แสดงอยู่
        }
        else if (inventory.currentSelectedSlot == inventory.equippedArmorSlot) //   Armor
        {
            inventory.myEqImageArmor.sprite = null;
            inventory.myEqImageArmor.color = new Color(0f, 0f, 0f, 177f / 255f);
            inventory.equippedArmorSlot = null;
            Debug.Log($"Unequipped {item.itemName} (Armor)");
            inventory.ShowItemNameArmor(""); // ล้างชื่อไอเทมที่แสดงอยู่
        }

        else if (inventory.currentSelectedSlot == inventory.equippedBootSlot) //   Boot
        {
            inventory.myEqImageBoot.sprite = null;
            inventory.myEqImageBoot.color = new Color(0f, 0f, 0f, 177f / 255f);
            inventory.equippedBootSlot = null;
            Debug.Log($"Unequipped {item.itemName} (Boot)");
            inventory.ShowItemNameBoot(""); // ล้างชื่อไอเทมที่แสดงอยู่
        }

        else if (inventory.currentSelectedSlot == inventory.equippedRingSlot) //   Ring
        {
            inventory.myEqImageRing.sprite = null;
            inventory.myEqImageRing.color = new Color(0f, 0f, 0f, 177f / 255f);
            inventory.equippedRingSlot = null;
            Debug.Log($"Unequipped {item.itemName} (Ring)");
            inventory.ShowItemNameRing(""); // ล้างชื่อไอเทมที่แสดงอยู่
        }

        UpdateEquipButton();
    }





    public void SwapSlot(InventorySlot newSlot)
    {
        SO_Item keepItem;
        int keepstack;

        keepItem = item;
        keepstack = stack;

        SetSwap(newSlot.item, newSlot.stack);

        newSlot.SetSwap(keepItem, keepstack);

    }

    public void SetSwap(SO_Item swapItem, int amount)
    {
        item = swapItem;
        stack = amount;
        icon.sprite = swapItem.icon;

        CheckShowText();
    }

    public void MergeThisSlot(InventorySlot mergeSlot)
    {
        if (stack == item.maxStack || mergeSlot.stack == mergeSlot.item.maxStack)
        {
            SwapSlot(mergeSlot);
            return;
        }
        int ItemAmount = stack + mergeSlot.stack;

        int intInthisSlot = Mathf.Clamp(ItemAmount, 0, item.maxStack);
        stack = intInthisSlot;

        CheckShowText();

        int amountLeft = ItemAmount - intInthisSlot;
        if (amountLeft > 0)
        {
            //set slot
            mergeSlot.SetThislot(mergeSlot.item, amountLeft);
        }
        else
        {
            //remove
            inventory.RemoveItem(mergeSlot);
        }
        inventory.ShowItemDetail(this);
    }
    public void MergeThisSlot(SO_Item mergeItem, int mergeAmount)
    {
        item = mergeItem;
        icon.sprite = mergeItem.icon;

        int ItemAmount = stack + mergeAmount; // total item from 2 slot

        int intInthisSlot = Mathf.Clamp(ItemAmount, 0, item.maxStack);
        stack = intInthisSlot;

        CheckShowText();

        int amountLeft = ItemAmount - intInthisSlot;
        if (amountLeft > 0)
        {
            InventorySlot slot = inventory.IsEmptySlotLeft(mergeItem, this);
            if (slot == null)
            {
                inventory.DropItem(mergeItem, amountLeft);
                return;
            }
            else
            {
                slot.MergeThisSlot(mergeItem, amountLeft);
            }
        }

    }

    public void SetThislot(SO_Item newItem, int amount)
    {
        item = newItem;
        icon.sprite = newItem.icon;

        int ItemAmount = amount;

        int intInthisSlot = Mathf.Clamp(ItemAmount, 0, newItem.maxStack);
        stack = intInthisSlot;

        CheckShowText();

        int amountLeft = ItemAmount - intInthisSlot;
        if (amountLeft > 0)
        {
            InventorySlot slot = inventory.IsEmptySlotLeft(newItem, this);
            if (slot == null)
            {
                //Drop Item
                return;
            }
            else
            {
                slot.SetThislot(newItem, amountLeft);
            }
        }
    }

    public void CheckShowText()
    {
        UpdateColorSlot();
        stackText.text = stack.ToString();
        if (item.maxStack < 2)
        {
            stackText.gameObject.SetActive(false);
        }
        else
        {
            if (stack > 1)
                stackText.gameObject.SetActive(true);
            else
                stackText.gameObject.SetActive(false);
        }
    }

    public void UpdateColorSlot()
    {
        if (item == inventory.EMPTY_ITEM)
            icon.color = emptyColor;
        else
            icon.color = itemColor;
    }
}