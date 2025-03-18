using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowDropItem : MonoBehaviour
{
    public GameObject dropUIPrefab;   // Prefab for drop UI
    public Transform uiContainer;     // Parent transform for instantiated UIs
    public float displayTime = 2f;    // Time in seconds before UI disappears
   

    // Show drop item UI
    public void ShowDrop(SO_Item item)
    {
        // Instantiate the UI prefab
        GameObject dropInstance = Instantiate(dropUIPrefab, uiContainer);
        dropInstance.GetComponent<DropItemSlots>().HandleStatsSlots(item);
        Destroy(dropInstance, displayTime);
    }
}