using UnityEngine;

public class PotionSettingsUIToggle : MonoBehaviour
{
    public void ToggleUI(GameObject targetUI)
    {
       
            // Toggle visibility
            targetUI.SetActive(!targetUI.activeSelf);
        
    }
}
