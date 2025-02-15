using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class StageData
{
    public string stageName;
    public string stageDetails;
    public Sprite stageIcon;
}

public class StageSelectionManager : MonoBehaviour
{
    [SerializeField]private EnemySpawner enemySpawner;
    private TestTeleportPlayer _teleportPlayer;
    [Header("Stage Data")]
    public StageData[] stages;
    public int currentStage = 1;  // Latest unlocked stage (starting from 1)

    [Header("UI References")]
    public GameObject stageButtonPrefab;
    public Transform stageButtonContainer;
    public GameObject stageDetailPanel;
    public GameObject stagePanel;
    [Header("Stage Detail UI")]
    public TextMeshProUGUI stageNumberText;
    public TextMeshProUGUI stageDetailsText;
    public Button enterStageButton;

    [Header("Stage Button Colors")]
    public Color completedColor = Color.green;
    public Color currentColor = Color.yellow;
    public Color lockedColor = Color.gray;

    private int selectedStageIndex = 1;
    private bool isStageSelectOpen = false;
    private void Start()
    {
        _teleportPlayer = FindObjectOfType<TestTeleportPlayer>();
        CreateStageButtons();
        stageDetailPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            UpdateStageButtons();
        }
    }

    void CreateStageButtons()
    {
        for (int i = 1; i < stages.Length; i++)
        {
            GameObject buttonObj = Instantiate(stageButtonPrefab, stageButtonContainer);
            Button stageButton = buttonObj.GetComponent<Button>();
            Image buttonImage = buttonObj.GetComponent<Image>();

            // Set stage icon
            if (stages[i-1].stageIcon != null)
            {
                buttonImage.sprite = stages[i-1].stageIcon;
            }

            // Set button color based on progress
            if (i < currentStage)
            {
                buttonImage.color = completedColor;  // Completed stages
            }
            else if (i == currentStage)
            {
                buttonImage.color = currentColor;    // Current stage
            }
            else
            {
                buttonImage.color = lockedColor;     // Locked stages
            }

            // Store the stage index and add click listener
            int stageIndex = i;
            stageButton.onClick.AddListener(() => ShowStageDetails(stageIndex));

            // Disable button if stage is locked
            stageButton.interactable = (i <= currentStage);
        }
    }
    // เรียกใช้ method นี้เมื่อแสดง stage details
    void ShowStageDetails(int stageIndex)
    {
        selectedStageIndex = stageIndex;  // เก็บค่า stage ที่เลือก
        stageDetailPanel.SetActive(true);
        // No need to add 1 to stageIndex since it's already 1-based
        stageNumberText.text = $"Stage {stageIndex}";
        stageDetailsText.text = stages[stageIndex-1].stageDetails; // Array is still 0-based
        
        enterStageButton.interactable = (stageIndex <= currentStage);
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
           Hide();
        }
    }
    // Call this when a stage is completed
    public void CompleteStage(int stageIndex)
    {
        if (stageIndex == currentStage && stageIndex < stages.Length - 1)
        {
            currentStage++;
            UpdateStageButtons();
        }
    }

    void UpdateStageButtons()
    {
        // Update buttons using same logic as CreateStageButtons
        for (int i = 1; i <= stages.Length; i++)
        {
            // คำนวณ index ที่จะใช้เข้าถึง child
            int childIndex = i + 1;
            
            // ตรวจสอบว่า index ไม่เกินจำนวน child ที่มี
            if (childIndex < stageButtonContainer.childCount)
            {
                try
                {
                    Image buttonImage = stageButtonContainer.GetChild(childIndex).GetComponent<Image>();
                    Button stageButton = stageButtonContainer.GetChild(childIndex).GetComponent<Button>();

                    // Use same coloring logic as in CreateStageButtons
                    if (i < currentStage)
                    {
                        buttonImage.color = completedColor;
                        stageButton.interactable = true;
                    }
                    else if (i == currentStage)
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
                    Debug.LogWarning($"Error updating stage button {i}: {e.Message}");
                }
            }
        }
    }
}