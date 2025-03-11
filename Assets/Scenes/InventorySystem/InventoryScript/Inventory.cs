using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{

    [Header("Inventory")]
    public SO_Item EMPTY_ITEM;
    public Transform slotPrefab;
    public Transform InventoryPanel;
    protected GridLayoutGroup gridLayoutGroup;
    [Space(5)]
    public int slotAmount = 30;
    public InventorySlot[] inventorySlots;
    public InventorySlot currentSelectedSlot;


    [Header("Mini Canvas")]
    public RectTransform miniCanvas;
    [SerializeField] protected InventorySlot rightClickSlot;

    private CanvasGroup inventoryCanvasGroup;
    private bool isVisible = false;
    [SerializeField] private float fadeSpeed = 5f;


    [SerializeField] private PlayerManager playerManager;

    [Header("5 EquippedSlot")]
    [Header("equippedItemSlot")]
    [SerializeField] public InventorySlot equippedItemSlot;
    public Image myEqImage;
    public TextMeshProUGUI itemNameText; //  UI 

    [Header("equippedHatSlot")]
    public InventorySlot equippedHatSlot;
    public Image myEqImageHat;
    public TextMeshProUGUI itemNameTextHat;

    [Header("equippedArmorSlot")]
    public InventorySlot equippedArmorSlot;
    public Image myEqImageArmor;
    public TextMeshProUGUI itemNameTextArmor;


    [Header("equippedBootSlot")]
    public InventorySlot equippedBootSlot;
    public Image myEqImageBoot;
    public TextMeshProUGUI itemNameTextBoot;

    [Header("equippedRingSlot")]
    public InventorySlot equippedRingSlot;
    public Image myEqImageRing;
    public TextMeshProUGUI itemNameTextRing;


    [Header("Detail UI")]
    public GameObject detailPanel; // หน้าต่าง detail
    public Image detailIcon; // รูปไอเทมใน detail
    public TextMeshProUGUI detailName; // ชื่อไอเทมใน detail
    public TextMeshProUGUI detailDescription; // คำอธิบายไอเทมใน detail
    public TextMeshProUGUI detailStackText;

    public Button equipButton;
    public TextMeshProUGUI equipButtonText;


    // Start is called before the first frame update
    void Start()
    {
        inventoryCanvasGroup = GetComponent<CanvasGroup>();
        // เริ่มต้นด้วยการซ่อน Inventory
        SetInventoryVisibility(false);

        gridLayoutGroup = InventoryPanel.GetComponent<GridLayoutGroup>();
        CreateInventorySlots();

        detailPanel.SetActive(false);

        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }

        // ทำการ Fade In/Out
        if (isVisible && inventoryCanvasGroup.alpha < 1f)
        {
            inventoryCanvasGroup.alpha += Time.deltaTime * fadeSpeed;
        }
        else if (!isVisible && inventoryCanvasGroup.alpha > 0f)
        {
            inventoryCanvasGroup.alpha -= Time.deltaTime * fadeSpeed;
        }
    }

    #region Inventory Methods


    

  
    //5 equipment
    public void ShowItemName(string name)
    {
        itemNameText.text = name; // แสดงชื่อไอเทม
    }

    public void ClearItemName()
    {
        itemNameText.text = ""; // clear ชื่อไอเทม
    }

    public void ShowItemNameHat(string name)
    {
        itemNameTextHat.text = name; // แสดงชื่อไอเทม
    }

    /*
    public void ClearItemNameHat()
    {
        itemNameTextHat.text = ""; // clear ชื่อไอเทม
    }
    */
    public void ShowItemNameArmor(string name)
    {
        itemNameTextArmor.text = name; // แสดงชื่อไอเทม
    }

   

    public void ShowItemNameBoot(string name)
    {
        itemNameTextBoot.text = name; // แสดงชื่อไอเทม
    }

    

    public void ShowItemNameRing(string name)
    {
        itemNameTextRing.text = name; // แสดงชื่อไอเทม
    }

    


    public void ToggleInventory()
    {
        isVisible = !isVisible;
        SetInventoryVisibility(isVisible);
    }

    private void SetInventoryVisibility(bool visible)
    {
        isVisible = visible;
        inventoryCanvasGroup.interactable = visible;
        inventoryCanvasGroup.blocksRaycasts = visible;
    }


    public void ShowInventory(bool show)
    {
        isVisible = show;
        SetInventoryVisibility(show);
    }

    public void ShowItemDetail(InventorySlot slot)
    {
        /*
        if (slot.item == EMPTY_ITEM)
            return;

        // อัปเดต UI ด้วยข้อมูลจาก SO_Item
        detailIcon.sprite = slot.item.icon;
        detailIcon.color = Color.white;
        detailName.text = slot.item.itemName;
        detailDescription.text = slot.item.description;

        // แสดงหน้าต่าง detail
        detailPanel.SetActive(true);

        */
        detailPanel.SetActive(true);
        detailIcon.sprite = slot.item.icon;
        detailIcon.color = Color.white;
        detailName.text = slot.item.itemName;
        detailDescription.text = slot.item.description;
        

        // อัปเดตจำนวน Stack
        if (slot.stack > 1)
        {
            detailStackText.gameObject.SetActive(true);
            detailStackText.text = slot.stack.ToString();
        }
        else
        {
            detailStackText.gameObject.SetActive(false);
        }

        
    }

    public void HideItemDetail()
    {
        // ซ่อนหน้าต่าง detail
        detailPanel.SetActive(false);
    }

  

    public bool IsItemEquipped(InventorySlot slot)
    {
        return equippedItemSlot == slot;
    }






    public void AddItem(SO_Item item, int amount)
    {
        InventorySlot slot = IsEmptySlotLeft(item);
        if (slot == null)
        {
            //full
            DropItem(item, amount);
            return;
        }
        slot.MergeThisSlot(item, amount);
    }





    public void UseItem() //OnClick Event
    {


        if (currentSelectedSlot == null) return; // ป้องกัน null reference

        currentSelectedSlot.UseItem();
    }
    public void DestroyItem() //OnClick Event
    {
        if (rightClickSlot == equippedItemSlot)
        {
            // ลบรูปภาพใน myEqImage
            myEqImage.sprite = null;  // ลบไอคอนของไอเทม
            myEqImage.color = new Color(1f, 1f, 1f, 0f);  // ทำให้ภาพเป็นโปร่งใส
        }
        rightClickSlot.SetThislot(EMPTY_ITEM, 0);
        OnFinishMiniCanvas();
    }
    public void DropItem() //OnCLick Event
    {
        if (rightClickSlot == equippedItemSlot)
        {
            // ลบรูปภาพใน myEqImage
            myEqImage.sprite = null;  // ลบไอคอนของไอเทม
            myEqImage.color = new Color(1f, 1f, 1f, 0f);  // ทำให้ภาพเป็นโปร่งใส
            ShowItemName(""); // clear ชื่อไอเทม
        }

        // สร้างไอเทมใหม่ที่ถูกทิ้ง
        ItemSpawner.Instance.SpawnItem(rightClickSlot.item, rightClickSlot.stack);
        DestroyItem();
    }


    public void DropItem(SO_Item item, int amount)
    {
        ItemSpawner.Instance.SpawnItem(item, amount);
    }


    public void RemoveItem(InventorySlot slot)
    {

        slot.SetThislot(EMPTY_ITEM, 0);
    }
    public void SortItem(bool Ascending = true)
    {
        //Sorting Item
        SetLayoutControlChild(true);

        List<InventorySlot> slotHaveItem = new List<InventorySlot>();
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.item != EMPTY_ITEM)
                slotHaveItem.Add(slot);
        }

        var sortArray = Ascending ?
                        slotHaveItem.OrderBy(Slot => Slot.item.id).ToArray() :
                        slotHaveItem.OrderByDescending(Slot => Slot.item.id).ToArray();
        foreach (InventorySlot slot in inventorySlots)
        {
            Destroy(slot.gameObject);
        }
        CreateInventorySlots();

        foreach (InventorySlot slot in sortArray)
        {
            AddItem(slot.item, slot.stack);
        }
    }





    




    public void CreateInventorySlots()
    {
        inventorySlots = new InventorySlot[slotAmount];
        for (int i = 0; i < slotAmount; i++)
        {
            Transform slot = Instantiate(slotPrefab, InventoryPanel);
            InventorySlot invSlot = slot.GetComponent<InventorySlot>();

            inventorySlots[i] = invSlot;
            invSlot.inventory = this;
            invSlot.SetThislot(EMPTY_ITEM, 0);

        }
    }

    public InventorySlot IsEmptySlotLeft(SO_Item itemChecker = null, InventorySlot itemSlot = null)
    {
        InventorySlot firstEmptySlot = null;
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot == itemSlot)
                continue;

            if (slot.item == itemChecker)
            {
                if (slot.stack < slot.item.maxStack)
                {
                    return slot;
                }
            }
            else if (slot.item == EMPTY_ITEM && firstEmptySlot == null)
                firstEmptySlot = slot;
        }
        return firstEmptySlot;
    }

    public void SetLayoutControlChild(bool isControlled)
    {
        gridLayoutGroup.enabled = isControlled;
    }

    #endregion

    #region Mini Canvas

    public void SetRightClickSlot(InventorySlot slot)
    {
        rightClickSlot = slot;
    }

    public void OpenMiniCanvas(Vector2 clickPosition)
    {
        miniCanvas.position = clickPosition;
        miniCanvas.gameObject.SetActive(true);
    }

    public void OnFinishMiniCanvas()
    {
        rightClickSlot = null;
        miniCanvas.gameObject.SetActive(false);
    }

    #endregion

}