using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ZoneData
{
    public string zoneName;        // ชื่อโซน (เช่น "ธาตุดิน", "ธาตุน้ำ")
    public string zoneDescription; // คำอธิบายโซน
    public Sprite zoneIcon;        // ไอคอนโซน
    public Button zoneButton;      // ปุ่มสำหรับเลือกโซน
    public bool isUnlocked = false; // สถานะการปลดล็อคโซน
}

public class ZoneSelectionManager : MonoBehaviour
{
    [Header("Zone Data")]
    public ZoneData[] zones;
    public int currentUnlockedZone = 0; // โซนล่าสุดที่ปลดล็อค (index ใน array)

    [Header("UI References")]
    public GameObject zoneSelectionPanel; // พาเนลหลักสำหรับเลือกโซน
    public TextMeshProUGUI zoneInfoText;  // ข้อมูลโซนที่เลือก (ถ้ามี)
    
    [Header("Stage Manager")]
    public StageSelectionManager stageManager; // อ้างอิงถึง StageSelectionManager ตัวเดียว
    
    [Header("Zone Button Colors")]
    public Color unlockedColor = Color.white;
    public Color lockedColor = Color.gray;

    private int selectedZoneIndex = -1;

    private void Start()
    {
        // ถ้าไม่ได้กำหนด stageManager ให้ค้นหาในฉาก
        if (stageManager == null)
        {
            stageManager = FindObjectOfType<StageSelectionManager>();
        }

        InitializeZoneButtons();
        
        // อัพเดทสถานะการปลดล็อคโซนตาม currentUnlockedZone
        UpdateZoneUnlockStatus();
        
        // แสดงพาเนลเลือกโซนตอนเริ่มต้น
        ShowZoneSelection();
    }

    void InitializeZoneButtons()
    {
        for (int i = 0; i < zones.Length; i++)
        {
            if (zones[i].zoneButton == null) continue;

            Button zoneButton = zones[i].zoneButton;
            Image buttonImage = zoneButton.GetComponent<Image>();

            // ตั้งค่าไอคอนของโซน (ถ้ามี)
            if (zones[i].zoneIcon != null)
            {
                buttonImage.sprite = zones[i].zoneIcon;
            }

            // เก็บค่า index ของโซนและเพิ่ม listener
            int zoneIndex = i;
            
            // ลบ listener เดิมทั้งหมด (ถ้ามี)
            zoneButton.onClick.RemoveAllListeners();
            zoneButton.onClick.AddListener(() => SelectZone(zoneIndex));

            // อัพเดทสีและการทำงานของปุ่มตามสถานะการปลดล็อค
            UpdateZoneButtonState(i);
        }
    }

    // อัพเดทสถานะการปลดล็อคของทุกโซน
    void UpdateZoneUnlockStatus()
    {
        for (int i = 0; i <= currentUnlockedZone && i < zones.Length; i++)
        {
            zones[i].isUnlocked = true;
        }

        for (int i = 0; i < zones.Length; i++)
        {
            UpdateZoneButtonState(i);
        }
    }

    // อัพเดทสถานะปุ่มตามการปลดล็อค
    void UpdateZoneButtonState(int zoneIndex)
    {
        if (zones[zoneIndex].zoneButton == null) return;

        Image buttonImage = zones[zoneIndex].zoneButton.GetComponent<Image>();
        Button zoneButton = zones[zoneIndex].zoneButton;

        if (zones[zoneIndex].isUnlocked)
        {
            buttonImage.color = unlockedColor;
            zoneButton.interactable = true;
        }
        else
        {
            buttonImage.color = lockedColor;
            zoneButton.interactable = false;
        }
    }

    // เลือกโซน
    public void SelectZone(int zoneIndex)
    {
        if (zoneIndex < 0 || zoneIndex >= zones.Length || !zones[zoneIndex].isUnlocked)
            return;

        selectedZoneIndex = zoneIndex;
        
        // แสดงข้อมูลโซน (ถ้ามีการแสดงข้อมูล)
        if (zoneInfoText != null)
        {
            zoneInfoText.text = $"{zones[zoneIndex].zoneName}\n{zones[zoneIndex].zoneDescription}";
        }

        // ซ่อนพาเนลเลือกโซนและแสดงแมพของโซนที่เลือก
        zoneSelectionPanel.SetActive(false);
        
        // แจ้ง StageSelectionManager ให้แสดงแมพโซนที่เลือก
        if (stageManager != null)
        {
            stageManager.ShowZoneMap(zoneIndex);
        }
    }

    // แสดงหน้าเลือกโซน
    public void ShowZoneSelection()
    {
        zoneSelectionPanel.SetActive(true);
        selectedZoneIndex = -1;
    }

    // ปลดล็อคโซนตาม index
    public void UnlockZone(int zoneIndex)
    {
        if (zoneIndex >= 0 && zoneIndex < zones.Length && !zones[zoneIndex].isUnlocked)
        {
            zones[zoneIndex].isUnlocked = true;
            UpdateZoneButtonState(zoneIndex);
            
            // ถ้า zoneIndex มากกว่า currentUnlockedZone ให้อัพเดท currentUnlockedZone
            if (zoneIndex > currentUnlockedZone)
            {
                currentUnlockedZone = zoneIndex;
            }
            
            // ที่นี่คุณสามารถเพิ่มเอฟเฟกต์แจ้งเตือนว่าปลดล็อคโซนใหม่ได้
            Debug.Log($"Unlocked new zone: {zones[zoneIndex].zoneName}");
        }
    }
}