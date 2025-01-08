using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class InventorySlot : MonoBehaviour, IDropHandler, IDragHandler,IBeginDragHandler,IEndDragHandler, IPointerClickHandler
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
    private PlayerManager _playerManager;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        siblingIndex = transform.GetSiblingIndex();
        _playerManager = FindObjectOfType<PlayerManager>();
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
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item == inventory.EMPTY_ITEM)
                return;

            // inventory open mini canvas
            inventory.OpenMiniCanvas(eventData.position);
            inventory.SetRightClickSlot(this);
        }
        
    }

    #endregion

    public void UseItem()
    {
        stack = Mathf.Clamp(stack - 1, 0, item.maxStack);
        if (stack > 0)
        {
            CheckShowText();
            Debug.Log("Use Item");
            item.Use();
            if (item.itemName == "Healing Potion")
            {
                _playerManager.Heal(10f);
            }
        }
        else
        {
            if (item.itemName == "Iron Sword")
            {
                _playerManager.ChangeWeapon(15f);
                Debug.Log("Use Iron Sword");
            }
            inventory.RemoveItem(this);
        }
        

            
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
        if(stack == item.maxStack || mergeSlot.stack == mergeSlot.item.maxStack )
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
            if(stack > 1)
                stackText.gameObject.SetActive(true);
            else
                stackText.gameObject.SetActive(false);
        }
    }

    public void UpdateColorSlot()
    {
        if(item == inventory.EMPTY_ITEM)
            icon.color = emptyColor;
        else
            icon.color = itemColor;
    }
}
