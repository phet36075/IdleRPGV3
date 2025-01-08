using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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

    [Header("Mini Canvas")]
    public RectTransform miniCanvas;
    [SerializeField] protected InventorySlot rightClickSlot;

    private CanvasGroup inventoryCanvasGroup;
    private bool isVisible = false;
    [SerializeField] private float fadeSpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        inventoryCanvasGroup = GetComponent<CanvasGroup>();
        // เริ่มต้นด้วยการซ่อน Inventory
        SetInventoryVisibility(false);

        gridLayoutGroup = InventoryPanel.GetComponent<GridLayoutGroup>();
        CreateInventorySlots();
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
    public void AddItem(SO_Item item, int amount)
    {
        InventorySlot slot = IsEmptySlotLeft(item);
        if(slot == null)
        {
            //full
            DropItem(item, amount);
            return;
        }
        slot.MergeThisSlot(item, amount);
    }

    public void UseItem() //OnClick Event
    {
        // use
        rightClickSlot.UseItem();
        OnFinishMiniCanvas();
    }
    public void DestroyItem() //OnClick Event
    {
        rightClickSlot.SetThislot(EMPTY_ITEM, 0);
        OnFinishMiniCanvas();
    }
    public void DropItem() //OnCLick Event
    {
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
        foreach(InventorySlot slot in inventorySlots)
        {
            if(slot.item != EMPTY_ITEM)
                slotHaveItem.Add(slot);
        }

        var sortArray = Ascending ?
                        slotHaveItem.OrderBy(Slot => Slot.item.id).ToArray() :
                        slotHaveItem.OrderByDescending(Slot => Slot.item.id).ToArray();
        foreach (InventorySlot slot in inventorySlots )
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

            if(slot.item ==  itemChecker)
            {
                if(slot.stack < slot.item.maxStack)
                {
                    return slot;  
                }
            }else if(slot.item == EMPTY_ITEM && firstEmptySlot == null)
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
