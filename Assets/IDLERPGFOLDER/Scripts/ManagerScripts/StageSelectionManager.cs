using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class StageData
{
    public string stageName;
    public string stageDetails;
    public Sprite stageIcon;
    public Button stageButton; // ปุ่มที่มีอยู่แล้วในฉาก
    public bool isEnabled = true;
    public int zoneIndex; // โซนที่ด่านนี้อยู่
}

public class StageSelectionManager : MonoBehaviour
{
    [SerializeField]private EnemySpawner enemySpawner;
    private TestTeleportPlayer _teleportPlayer;
    
    [Header("Zone References")]
    public ZoneSelectionManager zoneManager;
    public GameObject[] zonePanels; // แผงแมพของแต่ละโซน
    
    [Header("Stage Data")]
    public StageData[] stages; // ด่านทั้งหมดในทุกโซน
    public int currentStage = 1; // ด่านล่าสุดที่ปลดล็อค

    [Header("UI References")]
    public GameObject stageDetailPanel;
    public GameObject stagePanel;
    public GameObject backButton; // ปุ่มกลับไปยังหน้าเลือกโซน
    
    [Header("Stage Detail UI")]
    public TextMeshProUGUI stageNumberText;
    public TextMeshProUGUI stageDetailsText;
    
    public Button enterStageButton;

    [Header("Stage Button Colors")]
    public Color completedColor = Color.green;
    public Color currentColor = Color.yellow;
    public Color lockedColor = Color.gray;
    public Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);

    private int selectedStageIndex = -1;
    private Dictionary<int, List<int>> zoneToStageMap; // จัดกลุ่มด่านตามโซน
    private int currentZoneIndex = -1;
    private bool isStageSelectOpen = false;
    private void Start()
    {
        _teleportPlayer = FindObjectOfType<TestTeleportPlayer>();
        
        // ถ้าไม่ได้กำหนด zoneManager ให้ค้นหาในฉาก
        if (zoneManager == null)
        {
            zoneManager = FindObjectOfType<ZoneSelectionManager>();
        }
        
        // สร้าง mapping ระหว่างโซนและด่าน
        CreateZoneStageMapping();
        
        // ตั้งค่าเริ่มต้นให้กับปุ่มทุกปุ่ม
        InitializeStageButtons();
        
        // ซ่อนรายละเอียดด่าน
        stageDetailPanel.SetActive(false);
        
        // ซ่อนทุกพาเนลโซนตอนเริ่มต้น
        HideAllZonePanels();
        
        // ตั้งค่าปุ่มกลับ
        if (backButton != null && zoneManager != null)
        {
            Button backBtn = backButton.GetComponent<Button>();
            if (backBtn != null)
            {
                backBtn.onClick.RemoveAllListeners();
                backBtn.onClick.AddListener(() => BackToZoneSelection());
            }
        }
    }

    // สร้างการจัดกลุ่มด่านตามโซน
    private void CreateZoneStageMapping()
    {
        zoneToStageMap = new Dictionary<int, List<int>>();
        
        // วนลูปผ่านทุกด่าน
        for (int i = 0; i < stages.Length; i++)
        {
            int zoneIndex = stages[i].zoneIndex;
            
            // ถ้ายังไม่มีรายการของโซนนี้ ให้สร้างใหม่
            if (!zoneToStageMap.ContainsKey(zoneIndex))
            {
                zoneToStageMap[zoneIndex] = new List<int>();
            }
            
            // เพิ่มด่านลงในรายการของโซน
            zoneToStageMap[zoneIndex].Add(i);
        }
    }

    // ตั้งค่าเริ่มต้นให้กับปุ่มทุกปุ่ม
    public void InitializeStageButtons()
    {
        for (int i = 0; i < stages.Length; i++)
        {
            // ข้ามถ้าไม่ได้ตั้งค่าปุ่มไว้หรือด่านถูกปิดใช้งาน
            if (stages[i].stageButton == null || !stages[i].isEnabled) continue;
            
            // ดึง component ที่จำเป็น
            Button stageButton = stages[i].stageButton;
            Image buttonImage = stageButton.GetComponent<Image>();

            // ตั้งค่า icon ของด่าน (ถ้ามี)
            if (stages[i].stageIcon != null)
            {
                buttonImage.sprite = stages[i].stageIcon;
            }

            // เลขด่านจริง คือ index+1
            int actualStageNumber = i + 1;

            // กำหนดสีปุ่มตามความคืบหน้า
            if (actualStageNumber < currentStage)
            {
                buttonImage.color = completedColor;  // ด่านที่ผ่านแล้ว
            }
            else if (actualStageNumber == currentStage)
            {
                buttonImage.color = currentColor;    // ด่านปัจจุบัน
            }
            else
            {
                buttonImage.color = lockedColor;     // ด่านที่ยังไม่ปลดล็อค
            }

            // เก็บค่า index ของด่านและเพิ่ม listener
            int stageIndex = actualStageNumber;
            
            // ลบ listener เดิมทั้งหมด (ถ้ามี) เพื่อป้องกันการทำงานซ้ำซ้อน
            stageButton.onClick.RemoveAllListeners();
            stageButton.onClick.AddListener(() => ShowStageDetails(stageIndex));

            // ปิดปุ่มถ้าด่านยังล็อคอยู่
            stageButton.interactable = (actualStageNumber <= currentStage);
        }
    }
    
    // แสดงรายละเอียดด่าน
    void ShowStageDetails(int stageNumber)
    {
        selectedStageIndex = stageNumber;  // เก็บค่า stage ที่เลือก
        stageDetailPanel.SetActive(true);
        stageNumberText.text = $"Stage {stageNumber}";
        
        int arrayIndex = stageNumber - 1;
        if (arrayIndex >= 0 && arrayIndex < stages.Length)
        {
            stageDetailsText.text = stages[arrayIndex].stageDetails;
        }
        else
        {
            stageDetailsText.text = "รายละเอียดด่านไม่พบ";
        }
        
        enterStageButton.interactable = (stageNumber <= currentStage);
    }
    
    // แสดงแมพของโซนที่เลือก
    public void ShowZoneMap(int zoneIndex)
    {
        currentZoneIndex = zoneIndex;
        
        // ซ่อนแมพทุกโซนก่อน
        HideAllZonePanels();
        
        // แสดงแมพของโซนที่เลือก
        if (zoneIndex >= 0 && zoneIndex < zonePanels.Length)
        {
            zonePanels[zoneIndex].SetActive(true);
        }
        
        // อัพเดทสถานะปุ่มในโซนนี้
        UpdateStageButtonsForZone(zoneIndex);
    }
    public void ToggleUI()
    {
        if (isStageSelectOpen)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }
    public void Show()
    {
        stagePanel.SetActive(true);
        isStageSelectOpen = true;
    }

    public void Hide()
    {
        stagePanel.SetActive(false);
        isStageSelectOpen = false;
    }
    // ซ่อนแมพทุกโซน
    void HideAllZonePanels()
    {
        foreach (var panel in zonePanels)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }
    
    // กลับไปยังหน้าเลือกโซน
    public void BackToZoneSelection()
    {
        HideAllZonePanels();
        stageDetailPanel.SetActive(false);
        
        if (zoneManager != null)
        {
            zoneManager.ShowZoneSelection();
        }
    }
    
    // ปิดหน้ารายละเอียดด่าน
    public void CloseStageDetails()
    {
        stageDetailPanel.SetActive(false);
    }
    
    // เรียกใช้ method นี้เมื่อกดปุ่ม Enter Stage
    public void EnterSelectedStage()
    {
        if (selectedStageIndex <= currentStage)
        {
            enemySpawner.SetStage(selectedStageIndex);
            
            // เรียกใช้ script เข้าด่านของคุณที่นี่
            // ตัวอย่าง: YourLevelLoader.LoadLevel(selectedStageIndex);
            Debug.Log($"Entering stage {selectedStageIndex}");
            CloseStageDetails();
        }
    }
    
    // เรียกเมื่อผ่านด่าน
    public void CompleteStage(int stageNumber)
    {
        if (stageNumber == currentStage)
        {
            currentStage++;
            UpdateStageButtons();
            
            // ตรวจสอบว่าเป็นด่านสุดท้ายของโซนหรือไม่
            if (zoneManager != null && IsLastStageOfZone(stageNumber))
            {
                // หาว่าเป็นโซนไหน
                int zoneIndex = GetZoneIndexForStage(stageNumber);
                if (zoneIndex != -1 && zoneIndex < zoneManager.zones.Length - 1)
                {
                    zoneManager.UnlockZone(zoneIndex + 1);
                }
            }
        }
    }
    
    // ตรวจสอบว่าเป็นด่านสุดท้ายของโซนหรือไม่
    public bool IsLastStageOfZone(int stageNumber)
    {
        int zoneIndex = GetZoneIndexForStage(stageNumber);
        
        if (zoneIndex != -1 && zoneToStageMap.ContainsKey(zoneIndex))
        {
            List<int> stagesInZone = zoneToStageMap[zoneIndex];
            if (stagesInZone.Count > 0)
            {
                // หาเลขด่านสุดท้ายของโซน
                int lastStageIndex = stagesInZone[stagesInZone.Count - 1];
                return (stageNumber - 1) == lastStageIndex;
            }
        }
        
        return false;
    }
    
    // หาว่าด่านนี้อยู่ในโซนไหน
    public int GetZoneIndexForStage(int stageNumber)
    {
        int arrayIndex = stageNumber - 1;
        
        if (arrayIndex >= 0 && arrayIndex < stages.Length)
        {
            return stages[arrayIndex].zoneIndex;
        }
        
        return -1;
    }

    // อัพเดทปุ่มทั้งหมด
    public void UpdateStageButtons()
    {
        // อัพเดทเฉพาะโซนปัจจุบัน
        if (currentZoneIndex != -1)
        {
            UpdateStageButtonsForZone(currentZoneIndex);
        }
        // หรืออัพเดททุกโซน
        else
        {
            foreach (int zoneIndex in zoneToStageMap.Keys)
            {
                UpdateStageButtonsForZone(zoneIndex);
            }
        }
    }
    
    // อัพเดทปุ่มในโซนที่กำหนด
    void UpdateStageButtonsForZone(int zoneIndex)
    {
        if (!zoneToStageMap.ContainsKey(zoneIndex)) return;
        
        List<int> stageIndices = zoneToStageMap[zoneIndex];
        
        foreach (int arrayIndex in stageIndices)
        {
            if (stages[arrayIndex].stageButton != null && stages[arrayIndex].isEnabled)
            {
                try
                {
                    Image buttonImage = stages[arrayIndex].stageButton.GetComponent<Image>();
                    Button stageButton = stages[arrayIndex].stageButton;
                    
                    // เลขด่านจริง คือ index+1
                    int actualStageNumber = arrayIndex + 1;

                    // ใช้เงื่อนไขเดียวกับตอนตั้งค่าปุ่ม
                    if (actualStageNumber < currentStage)
                    {
                        buttonImage.color = completedColor;
                        stageButton.interactable = true;
                    }
                    else if (actualStageNumber == currentStage)
                    {
                        buttonImage.color = currentColor;
                        stageButton.interactable = true;
                    }
                    else
                    {
                        buttonImage.color = lockedColor;
                        stageButton.interactable = false;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Error updating stage button {arrayIndex + 1}: {e.Message}");
                }
            }
        }
    }
    
    // Method สำหรับเปิด/ปิดด่าน
    public void SetStageEnabled(int stageNumber, bool enabled)
    {
        int arrayIndex = stageNumber - 1;
        if(arrayIndex >= 0 && arrayIndex < stages.Length)
        {
            stages[arrayIndex].isEnabled = enabled;
            
            // อัพเดทการแสดงผลของปุ่ม
            if(stages[arrayIndex].stageButton != null)
            {
                stages[arrayIndex].stageButton.gameObject.SetActive(enabled);
            }
        }
    }
}