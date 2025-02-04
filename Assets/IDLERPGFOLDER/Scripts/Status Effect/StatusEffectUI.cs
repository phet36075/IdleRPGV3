using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class StatusEffectData
{
    public string effectName;
    public int maxStacks = 1;
    public string evolveToEffect; // effect ที่จะเปลี่ยนไปเมื่อ stack เต็ม
    public float evolveDuration = 5f; // ระยะเวลาของ effect ใหม่
    
    public StatusEffectData(string name, int maxStacks, string evolveEffect = "", float evolveDuration = 5f)
    {
        this.effectName = name;
        this.maxStacks = maxStacks;
        this.evolveToEffect = evolveEffect;
        this.evolveDuration = evolveDuration;
    }
}

public class StatusEffect : MonoBehaviour
{
    public string effectName;
    public Sprite icon;
    public float duration;
    public float remainingTime;
    public Text durationText;
    public Text stackCountText;
    public int stackCount = 1;
    public int maxStacks = 1;
    public StatusEffectData effectData;
    
    private void Start()
    {
        durationText = transform.Find("Duration")?.GetComponent<Text>();
        stackCountText = transform.Find("StackCount")?.GetComponent<Text>();
        UpdateStackDisplay();
    }

    private void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            if (durationText != null)
            {
                durationText.text = remainingTime.ToString("F1");
            }

            if (remainingTime <= 0)
            {
                StatusEffectUI manager = transform.parent.GetComponentInParent<StatusEffectUI>();
                if (manager != null)
                {
                    manager.RemoveStatusEffect(effectName);
                }
            }
        }
    }

    public bool AddStack()
    {
        if (stackCount < maxStacks)
        {
            stackCount++;
            UpdateStackDisplay();
            return false; // ยังไม่ถึง max stack
        }
        return true; // ถึง max stack แล้ว
    }

    public void UpdateStackDisplay()
    {
        if (stackCountText != null)
        {
            stackCountText.text = stackCount > 1 ? stackCount.ToString() : "";
        }
    }
}

public class StatusEffectUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject statusEffectPrefab;
    [SerializeField] private Transform statusEffectContainer;
    
    [Header("Status Effect Settings")]
    [SerializeField] private List<StatusEffectData> statusEffectSettings = new List<StatusEffectData>();
    
    [Header("Effect Icons")]
    [SerializeField] private Sprite poisonIcon;
    [SerializeField] private Sprite poisonExplodeIcon;
    [SerializeField] private Sprite burnIcon;
    [SerializeField] private Sprite stunIcon;

    private Dictionary<string, GameObject> activeStatusEffects = new Dictionary<string, GameObject>();
    private Dictionary<string, Sprite> effectIcons = new Dictionary<string, Sprite>();

    private void Start()
    {
        // กำหนดค่าเริ่มต้นสำหรับ effects ต่างๆ
        if (statusEffectSettings.Count == 0)
        {
            statusEffectSettings.Add(new StatusEffectData("Poison", 3, "PoisonExplode", 5f));
            statusEffectSettings.Add(new StatusEffectData("PoisonExplode", 1));
            statusEffectSettings.Add(new StatusEffectData("Burn", 1));
            statusEffectSettings.Add(new StatusEffectData("Stun", 1));
        }

        // เก็บ icons ไว้ใน dictionary
        effectIcons.Add("Poison", poisonIcon);
        effectIcons.Add("PoisonExplode", poisonExplodeIcon);
        effectIcons.Add("Burn", burnIcon);
        effectIcons.Add("Stun", stunIcon);
    }

    public void AddStatusEffect(string effectName, Sprite icon = null, float duration = 5f)
    {
        var settings = statusEffectSettings.Find(s => s.effectName == effectName);
        if (settings == null)
        {
            Debug.LogWarning($"No settings found for effect: {effectName}");
            return;
        }

        // ถ้ามี effect เดิมอยู่แล้ว
        if (activeStatusEffects.TryGetValue(effectName, out GameObject existingEffect))
        {
            StatusEffect existingStatus = existingEffect.GetComponent<StatusEffect>();
            if (existingStatus != null)
            {
                if (existingStatus.AddStack()) // ถ้า return true แปลว่า stack เต็มแล้ว
                {
                    // ถ้ามี effect ที่จะ evolve ไป
                    if (!string.IsNullOrEmpty(settings.evolveToEffect))
                    {
                        // ลบ effect เดิม
                        RemoveStatusEffect(effectName);
                        
                        // สร้าง effect ใหม่
                        if (effectIcons.TryGetValue(settings.evolveToEffect, out Sprite evolveIcon))
                        {
                            AddStatusEffect(settings.evolveToEffect, evolveIcon, settings.evolveDuration);
                        }
                    }
                }
                else
                {
                    // รีเซ็ตเวลาเมื่อเพิ่ม stack
                    existingStatus.remainingTime = duration;
                }
            }
            return;
        }

        // สร้าง effect ใหม่
        GameObject newEffect = Instantiate(statusEffectPrefab, statusEffectContainer);
        
        Image iconImage = newEffect.GetComponentInChildren<Image>();
        Text durationText = newEffect.transform.Find("Duration")?.GetComponent<Text>();
        Text stackCountText = newEffect.transform.Find("StackCount")?.GetComponent<Text>();

        if (iconImage != null && durationText != null)
        {
            // ใช้ icon จาก dictionary ถ้าไม่ได้ระบุ icon มา
            iconImage.sprite = icon ?? effectIcons[effectName];
            durationText.text = duration.ToString("F1");

            StatusEffect statusData = newEffect.AddComponent<StatusEffect>();
            statusData.effectName = effectName;
            statusData.icon = icon;
            statusData.duration = duration;
            statusData.remainingTime = duration;
            statusData.durationText = durationText;
            statusData.stackCountText = stackCountText;
            statusData.maxStacks = settings.maxStacks;
            statusData.effectData = settings;
            statusData.UpdateStackDisplay();

            activeStatusEffects.Add(effectName, newEffect);
        }
    }

    public void RemoveStatusEffect(string effectName)
    {
        if (activeStatusEffects.TryGetValue(effectName, out GameObject effectObj))
        {
            Destroy(effectObj);
            activeStatusEffects.Remove(effectName);
        }
    }
}