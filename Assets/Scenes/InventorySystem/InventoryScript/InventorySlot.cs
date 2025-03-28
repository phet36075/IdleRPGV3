﻿using System.Collections;
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
            // ตรวจสอบว่ามีอาวุธถูกสวมใส่อยู่หรือไม่
            if (inventory.equippedItemSlot != null && inventory.equippedItemSlot != this)
            {
                // ถอดอาวุธเก่าออก
                Debug.Log($"Unequipping {inventory.equippedItemSlot.item.itemName} (Weapon) before Equipping {item.itemName}");
                inventory.equippedItemSlot.Unequip();
            }

            inventory.equippedItemSlot = this; // ตั้งค่าช่องที่กำลังใช้อยู่

            if (inventory.myEqImage != null)
            {
                inventory.myEqImage.sprite = item.icon;
                inventory.myEqImage.color = Color.white; // ทำให้รูปไม่โปร่งใส
            }
            Debug.Log($"Equipped {item.itemName} (Weapon)");

            inventory.ShowItemName(item.itemName); // แสดงชื่อไอเทมใน UI
            if (item.itemName == "Fire Sword")
            {
                playerManager.ChangeWeaponElement(ElementType.Fire);
            }
            else if (item.itemName == "Water Sword")
            {
                playerManager.ChangeWeaponElement(ElementType.Water);
            }
            else if (item.itemName == "Wind Sword")
            {
                playerManager.ChangeWeaponElement(ElementType.Wind);
            }
            else if (item.itemName == "Earth Sword")
            {
                playerManager.ChangeWeaponElement(ElementType.Earth);
            }
            else if (item.itemName == "Light Sword")
            {
                playerManager.ChangeWeaponElement(ElementType.Light);
            }
            else if (item.itemName == "Dark Sword")
            {
                playerManager.ChangeWeaponElement(ElementType.Dark);
            }
            UpdateEquipButton();
            inventory.UpdateEquipStrip(this, inventory.equipStripWeapon);
            return; // **ออกจากเมทอดทันที ไม่ลด stack**
        }


        else if (item.itemType == ItemType.Hat) //  Hat
        {
            // ตรวจสอบว่ามีหมวกถูกสวมใส่อยู่หรือไม่
            if (inventory.equippedHatSlot != null && inventory.equippedHatSlot != this)
            {
                // ถอดหมวกเก่าออก
                Debug.Log($"Unequipping {inventory.equippedHatSlot.item.itemName} (Hat) before Equipping {item.itemName}");
                inventory.equippedHatSlot.Unequip();
            }

            inventory.equippedHatSlot = this;
            //Equip
            playerManager.healthBonus += inventory.equippedHatSlot.item.bonusHealth;
            playerManager.defenseBonus += inventory.equippedHatSlot.item.bonusDefense;
            playerManager.armorPenetrationBonus += inventory.equippedHatSlot.item.bonusPen;
            playerManager.regenRateBonus += inventory.equippedHatSlot.item.bonusHealthRegen;
            playerManager.baseDamageBonus += inventory.equippedHatSlot.item.bonusDamage;
            playerManager.criticalChanceBonus += inventory.equippedHatSlot.item.bonusCritChance;
            playerManager.maxManaBonus += inventory.equippedHatSlot.item.bonusMana;
            playerManager.manaRegenRateBonus += inventory.equippedHatSlot.item.bonusManaRegen;
            playerManager.criticalDamageBonus += inventory.equippedHatSlot.item.bonusCritDamage;
            if (inventory.myEqImageHat != null)
            {
                inventory.myEqImageHat.sprite = item.icon;
                inventory.myEqImageHat.color = Color.white;
            }
            Debug.Log($"Equipped {item.itemName} (Hat)");

            inventory.ShowItemNameHat(item.itemName);
            UpdateEquipButton();

            inventory.UpdateEquipStrip(this, inventory.equipStripHat);
            playerManager.UpdateHealthBar();
            playerManager.RecalculateStats();
            return;
        }

   
        else if (item.itemType == ItemType.Armor) //  Armor
        {
            // ตรวจสอบว่ามีเกราะถูกสวมใส่อยู่หรือไม่
            if (inventory.equippedArmorSlot != null && inventory.equippedArmorSlot != this)
            {
                // ถอดเกราะเก่าออก
                Debug.Log($"Unequipping {inventory.equippedArmorSlot.item.itemName} (Armor) before Equipping {item.itemName}");
                inventory.equippedArmorSlot.Unequip();
            }

            

            inventory.equippedArmorSlot = this;
            
            //Equip
            playerManager.healthBonus += inventory.equippedArmorSlot.item.bonusHealth;
            playerManager.defenseBonus += inventory.equippedArmorSlot.item.bonusDefense;
            playerManager.armorPenetrationBonus += inventory.equippedArmorSlot.item.bonusPen;
            playerManager.regenRateBonus += inventory.equippedArmorSlot.item.bonusHealthRegen;
            playerManager.baseDamageBonus += inventory.equippedArmorSlot.item.bonusDamage;
            playerManager.criticalChanceBonus += inventory.equippedArmorSlot.item.bonusCritChance;
            playerManager.maxManaBonus += inventory.equippedArmorSlot.item.bonusMana;
            playerManager.manaRegenRateBonus += inventory.equippedArmorSlot.item.bonusManaRegen;
            playerManager.criticalDamageBonus += inventory.equippedArmorSlot.item.bonusCritDamage;
            
            if (inventory.myEqImageArmor != null)
            {
                inventory.myEqImageArmor.sprite = item.icon;
                inventory.myEqImageArmor.color = Color.white;
            }
            Debug.Log($"Equipped {item.itemName} (Armor)");

            inventory.ShowItemNameArmor(item.itemName);
            UpdateEquipButton();
            playerManager.UpdateHealthBar();
            playerManager.RecalculateStats();
            inventory.UpdateEquipStrip(this, inventory.equipStripArmor);
            return;
        }

    

        else if (item.itemType == ItemType.Boot) //  Boot
        {
            // ตรวจสอบว่ามีรองเท้าถูกสวมใส่อยู่หรือไม่
            if (inventory.equippedBootSlot != null && inventory.equippedBootSlot != this)
            {
                // ถอดรองเท้าเก่าออก
                Debug.Log($"Unequipping {inventory.equippedBootSlot.item.itemName} (Boot) before Equipping {item.itemName}");
                inventory.equippedBootSlot.Unequip();
            }

            inventory.equippedBootSlot = this;

            //Equip
            playerManager.healthBonus += inventory.equippedBootSlot.item.bonusHealth;
            playerManager.defenseBonus += inventory.equippedBootSlot.item.bonusDefense;
            playerManager.armorPenetrationBonus += inventory.equippedBootSlot.item.bonusPen;
            playerManager.regenRateBonus += inventory.equippedBootSlot.item.bonusHealthRegen;
            playerManager.baseDamageBonus += inventory.equippedBootSlot.item.bonusDamage;
            playerManager.criticalChanceBonus += inventory.equippedBootSlot.item.bonusCritChance;
            playerManager.maxManaBonus += inventory.equippedBootSlot.item.bonusMana;
            playerManager.manaRegenRateBonus += inventory.equippedBootSlot.item.bonusManaRegen;
            playerManager.criticalDamageBonus += inventory.equippedBootSlot.item.bonusCritDamage;
            
            
            
            if (inventory.myEqImageBoot != null)
            {
                inventory.myEqImageBoot.sprite = item.icon;
                inventory.myEqImageBoot.color = Color.white;
            }
            Debug.Log($"Equipped {item.itemName} (Boot)");

            inventory.ShowItemNameBoot(item.itemName);
            UpdateEquipButton();
            playerManager.UpdateHealthBar();
            playerManager.RecalculateStats();
            inventory.UpdateEquipStrip(this, inventory.equipStripBoot);
            return;
        }


        else if (item.itemType == ItemType.Ring) //  Ring
        {
            // ตรวจสอบว่ามีแหวนถูกสวมใส่อยู่หรือไม่
            if (inventory.equippedRingSlot != null && inventory.equippedRingSlot != this)
            {
                // ถอดแหวนเก่าออก
                Debug.Log($"Unequipping {inventory.equippedRingSlot.item.itemName} (Ring) before Equipping {item.itemName}");
                inventory.equippedRingSlot.Unequip();
            }

            inventory.equippedRingSlot = this;

            //Equip
            playerManager.healthBonus += inventory.equippedRingSlot.item.bonusHealth;
            playerManager.defenseBonus += inventory.equippedRingSlot.item.bonusDefense;
            playerManager.armorPenetrationBonus += inventory.equippedRingSlot.item.bonusPen;
            playerManager.regenRateBonus += inventory.equippedRingSlot.item.bonusHealthRegen;
            playerManager.baseDamageBonus += inventory.equippedRingSlot.item.bonusDamage;
            playerManager.criticalChanceBonus += inventory.equippedRingSlot.item.bonusCritChance;
            playerManager.maxManaBonus += inventory.equippedRingSlot.item.bonusMana;
            playerManager.manaRegenRateBonus += inventory.equippedRingSlot.item.bonusManaRegen;
            playerManager.criticalDamageBonus += inventory.equippedRingSlot.item.bonusCritDamage;
            
            if (inventory.myEqImageRing != null)
            {
                inventory.myEqImageRing.sprite = item.icon;
                inventory.myEqImageRing.color = Color.white;
            }
            Debug.Log($"Equipped {item.itemName} (Ring)");

            inventory.ShowItemNameRing(item.itemName);
            UpdateEquipButton();
            inventory.UpdateEquipStrip(this, inventory.equipStripRing);
            playerManager.UpdateHealthBar();
            playerManager.RecalculateStats();
            return;
        }
        item.Use();
        if (item.itemName == "Healing Potion")
        {
            playerManager.Heal(110f);
        }
        if (item.itemName == "Mana Potion")
        {
            playerManager.RestoreMana(10f);
        }

        if (item.itemName == "Fire Skill Book")
        {

        }

        // ลด stack ตามปกติ
        stack = Mathf.Clamp(stack - 1, 0, item.maxStack);

        if (stack > 0)
        {
            CheckShowText();
            //Debug.Log("Use Item");
            Debug.Log("UseItem() called, stack: " + stack);
            
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
        if (inventory.equippedItemSlot != null && inventory.equippedItemSlot == this) // ถอดอาวุธ
        {
            if (inventory.equippedItemSlot.item.itemType == ItemType.Weapon)
            {
                playerManager.ChangeWeaponElement(ElementType.None); // รีเซ็ตธาตุอาวุธ
            }

            // ลบไอคอนอาวุธออกจาก UI
            inventory.myEqImage.sprite = null;
            inventory.myEqImage.color = new Color(0f, 0f, 0f, 177f / 255f);
            Debug.Log($"Unequipped {inventory.equippedItemSlot.item.itemName} (Weapon)");
            inventory.ShowItemName("");
            inventory.equippedItemSlot = null;
            inventory.UpdateEquipStrip(null, inventory.equipStripWeapon);
        }
        else if (inventory.equippedHatSlot != null && inventory.equippedHatSlot == this) // ถอดหมวก
        {
            inventory.myEqImageHat.sprite = null;
            inventory.myEqImageHat.color = new Color(0f, 0f, 0f, 177f / 255f);
            Debug.Log($"Unequipped {inventory.equippedHatSlot.item.itemName} (Hat)");
            inventory.ShowItemNameHat("");
            
            playerManager.healthBonus -= inventory.equippedHatSlot.item.bonusHealth;
            playerManager.defenseBonus -= inventory.equippedHatSlot.item.bonusDefense;
            playerManager.armorPenetrationBonus -= inventory.equippedHatSlot.item.bonusPen;
            playerManager.regenRateBonus -= inventory.equippedHatSlot.item.bonusHealthRegen;
            playerManager.baseDamageBonus -= inventory.equippedHatSlot.item.bonusDamage;
            playerManager.criticalChanceBonus -= inventory.equippedHatSlot.item.bonusCritChance;
            playerManager.maxManaBonus -= inventory.equippedHatSlot.item.bonusMana;
            playerManager.manaRegenRateBonus -= inventory.equippedHatSlot.item.bonusManaRegen;
            playerManager.criticalDamageBonus -= inventory.equippedHatSlot.item.bonusCritDamage;
            
            inventory.equippedHatSlot = null;

            inventory.UpdateEquipStrip(null, inventory.equipStripHat);
            playerManager.UpdateHealthBar();
            playerManager.RecalculateStats();
        }
        else if (inventory.equippedArmorSlot != null && inventory.equippedArmorSlot == this) // ถอดเกราะ
        {
            inventory.myEqImageArmor.sprite = null;
            inventory.myEqImageArmor.color = new Color(0f, 0f, 0f, 177f / 255f);
            Debug.Log($"Unequipped {inventory.equippedArmorSlot.item.itemName} (Armor)");
            inventory.ShowItemNameArmor("");
            //Unequip
            
            
            playerManager.healthBonus -= inventory.equippedArmorSlot.item.bonusHealth;
            playerManager.defenseBonus -= inventory.equippedArmorSlot.item.bonusDefense;
            playerManager.armorPenetrationBonus -= inventory.equippedArmorSlot.item.bonusPen;
            playerManager.regenRateBonus -= inventory.equippedArmorSlot.item.bonusHealthRegen;
            playerManager.baseDamageBonus -= inventory.equippedArmorSlot.item.bonusDamage;
            playerManager.criticalChanceBonus -= inventory.equippedArmorSlot.item.bonusCritChance;
            playerManager.maxManaBonus -= inventory.equippedArmorSlot.item.bonusMana;
            playerManager.manaRegenRateBonus -= inventory.equippedArmorSlot.item.bonusManaRegen;
            playerManager.criticalDamageBonus -= inventory.equippedArmorSlot.item.bonusCritDamage;
            
            
            inventory.equippedArmorSlot = null;
            
            inventory.UpdateEquipStrip(null, inventory.equipStripArmor);

            
            playerManager.UpdateHealthBar();
            playerManager.RecalculateStats();

        }
        else if (inventory.equippedBootSlot != null && inventory.equippedBootSlot == this) // ถอดรองเท้า
        {
            inventory.myEqImageBoot.sprite = null;
            inventory.myEqImageBoot.color = new Color(0f, 0f, 0f, 177f / 255f);
            Debug.Log($"Unequipped {inventory.equippedBootSlot.item.itemName} (Boot)");
            inventory.ShowItemNameBoot("");
            
            playerManager.healthBonus -= inventory.equippedBootSlot.item.bonusHealth;
            playerManager.defenseBonus -= inventory.equippedBootSlot.item.bonusDefense;
            playerManager.armorPenetrationBonus -= inventory.equippedBootSlot.item.bonusPen;
            playerManager.regenRateBonus -= inventory.equippedBootSlot.item.bonusHealthRegen;
            playerManager.baseDamageBonus -= inventory.equippedBootSlot.item.bonusDamage;
            playerManager.criticalChanceBonus -= inventory.equippedBootSlot.item.bonusCritChance;
            playerManager.maxManaBonus -= inventory.equippedBootSlot.item.bonusMana;
            playerManager.manaRegenRateBonus -= inventory.equippedBootSlot.item.bonusManaRegen;
            playerManager.criticalDamageBonus -= inventory.equippedBootSlot.item.bonusCritDamage;
            
            inventory.equippedBootSlot = null;
            inventory.UpdateEquipStrip(null, inventory.equipStripBoot);
            playerManager.UpdateHealthBar();
            playerManager.RecalculateStats();
        }
        else if (inventory.equippedRingSlot != null && inventory.equippedRingSlot == this) // ถอดแหวน
        {
            inventory.myEqImageRing.sprite = null;
            inventory.myEqImageRing.color = new Color(0f, 0f, 0f, 177f / 255f);
            Debug.Log($"Unequipped {inventory.equippedRingSlot.item.itemName} (Ring)");
            inventory.ShowItemNameRing("");
            
            playerManager.healthBonus -= inventory.equippedRingSlot.item.bonusHealth;
            playerManager.defenseBonus -= inventory.equippedRingSlot.item.bonusDefense;
            playerManager.armorPenetrationBonus -= inventory.equippedRingSlot.item.bonusPen;
            playerManager.regenRateBonus -= inventory.equippedRingSlot.item.bonusHealthRegen;
            playerManager.baseDamageBonus -= inventory.equippedRingSlot.item.bonusDamage;
            playerManager.criticalChanceBonus -= inventory.equippedRingSlot.item.bonusCritChance;
            playerManager.maxManaBonus -= inventory.equippedRingSlot.item.bonusMana;
            playerManager.manaRegenRateBonus -= inventory.equippedRingSlot.item.bonusManaRegen;
            playerManager.criticalDamageBonus -= inventory.equippedRingSlot.item.bonusCritDamage;
            
            inventory.equippedRingSlot = null;
            inventory.UpdateEquipStrip(null, inventory.equipStripRing);
        }
        playerManager.UpdateHealthBar();
        playerManager.RecalculateStats();
        UpdateEquipButton();
    }


    



    public void SwapSlot(InventorySlot newSlot)
    {
        if (newSlot == null) return;

        // ถ้าเป็นไอเทมชนิดเดียวกันให้รวม stack แทน
        if (item == newSlot.item)
        {
            MergeThisSlot(newSlot);
            return;
        }

        // ถ้าเป็นไอเทมคนละชนิด ให้สลับตำแหน่งกัน
        SO_Item keepItem = item;
        int keepStack = stack;

        SetSwap(newSlot.item, newSlot.stack);
        newSlot.SetSwap(keepItem, keepStack);

        // อัปเดตสถานะ currentSelectedSlot
        if (inventory.currentSelectedSlot == this)
        {
            inventory.currentSelectedSlot = newSlot;
        }
        else if (inventory.currentSelectedSlot == newSlot)
        {
            inventory.currentSelectedSlot = this;
        }

        // อัปเดตสถานะ Equipped Slots
        SwapEquippedSlot(ref inventory.equippedItemSlot, newSlot, inventory.myEqImage, inventory.itemNameText);
        SwapEquippedSlot(ref inventory.equippedHatSlot, newSlot, inventory.myEqImageHat, inventory.itemNameTextHat);
        SwapEquippedSlot(ref inventory.equippedArmorSlot, newSlot, inventory.myEqImageArmor, inventory.itemNameTextArmor);
        SwapEquippedSlot(ref inventory.equippedBootSlot, newSlot, inventory.myEqImageBoot, inventory.itemNameTextBoot);
        SwapEquippedSlot(ref inventory.equippedRingSlot, newSlot, inventory.myEqImageRing, inventory.itemNameTextRing);

        // อัปเดตปุ่ม Equip/Unequip
        inventory.currentSelectedSlot.UpdateEquipButton();
        newSlot.UpdateEquipButton();
    }

    // ฟังก์ชันช่วยลดโค้ดซ้ำ
    private void SwapEquippedSlot(ref InventorySlot equippedSlot, InventorySlot newSlot, Image image, TextMeshProUGUI text)
    {
        if (equippedSlot == this)
        {
            equippedSlot = newSlot;
        }
        else if (equippedSlot == newSlot)
        {
            equippedSlot = this;
        }
        UpdateEquippedSlotUI(equippedSlot, image, text);
    }


    private void UpdateEquippedSlotUI(InventorySlot slot, Image image, TextMeshProUGUI text)
    {
        if (slot != null && slot.item != inventory.EMPTY_ITEM)
        {
            image.sprite = slot.item.icon;
            image.color = Color.white;
            text.text = slot.item.itemName;
        }
        else
        {
            image.sprite = null;
            image.color = new Color(0f, 0f, 0f, 177f / 255f); // grey
            text.text = "";
        }
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
        UpdateEquipButton();
        mergeSlot.UpdateEquipButton();
        //inventory.ShowItemDetail(this);
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
