using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
public class ManaUI : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
   // [SerializeField] private Image manaBarFill;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private PlayerProperty playerProperty;
    public Slider manaBar;
    private void Start()
    {
        manaBar.maxValue = playerManager.playerProperty.maxMana;
        manaBar.value = playerManager.GetCurrentMana();
        if (playerManager != null)
        {
            // Subscribe to mana change event
            playerManager.OnManaChanged += UpdateManaUI;
            // Update UI ครั้งแรก
            UpdateManaUI(playerManager.GetCurrentMana());
        }

        StartCoroutine(RegenerateMana());
    }

    private IEnumerator RegenerateMana()
    {
        while (true)
        {
            yield return new WaitForSeconds(5);
            if (playerManager.GetCurrentMana() < playerProperty.maxMana)
            {
                playerManager.RestoreMana(playerManager.playerProperty.manaRegenRate);
            }
         
        }
        
    }
    private void UpdateManaUI(float currentMana)
    {
        // อัพเดทแถบ mana
        //manaBarFill.fillAmount = currentMana / playerManager.GetMaxMana();
        // อัพเดทตัวเลข mana
        manaText.text = $"{Mathf.Ceil(currentMana)}/{playerManager.GetMaxMana()}";
        manaBar.maxValue = playerManager.playerProperty.maxMana;
       StartCoroutine(SmoothManaBar());
       
    }
    IEnumerator SmoothManaBar()
    {
        float elapsedTime = 0f;
        float duration = 0.2f; // ระยะเวลาที่ต้องการให้การลดลงของแถบลื่นไหล
        float startValue = manaBar.value;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            manaBar.value = Mathf.Lerp(startValue, playerManager.GetCurrentMana(), elapsedTime / duration);
            yield return null;
        }

        manaBar.value = playerManager.GetCurrentMana();
      
    }
    private void OnDestroy()
    {
        if (playerManager != null)
        {
            playerManager.OnManaChanged -= UpdateManaUI;
        }
    }
}
