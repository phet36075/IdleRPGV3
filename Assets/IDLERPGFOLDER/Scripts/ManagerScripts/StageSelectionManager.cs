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
    [SerializeField]private StageManager stageManager;
    [Header("Stage Data")]
    public StageData[] stages;
    public int currentStage = 0;  // Latest unlocked stage

    [Header("UI References")]
    public GameObject stageButtonPrefab;
    public Transform stageButtonContainer;
    public GameObject stageDetailPanel;
    
    [Header("Stage Detail UI")]
    public TextMeshProUGUI stageNumberText;
    public TextMeshProUGUI stageDetailsText;
    public Button enterStageButton;

    [Header("Stage Button Colors")]
    public Color completedColor = Color.green;
    public Color currentColor = Color.yellow;
    public Color lockedColor = Color.gray;

    private void Start()
    {
        CreateStageButtons();
        stageDetailPanel.SetActive(false);
    }

    void CreateStageButtons()
    {
        for (int i = 0; i < stages.Length; i++)
        {
            GameObject buttonObj = Instantiate(stageButtonPrefab, stageButtonContainer);
            Button stageButton = buttonObj.GetComponent<Button>();
            Image buttonImage = buttonObj.GetComponent<Image>();

            // Set stage icon
            if (stages[i].stageIcon != null)
            {
                buttonImage.sprite = stages[i].stageIcon;
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
    private int selectedStageIndex;
    // เรียกใช้ method นี้เมื่อแสดง stage details
    void ShowStageDetails(int stageIndex)
    {
        selectedStageIndex = stageIndex;  // เก็บค่า stage ที่เลือก
        stageDetailPanel.SetActive(true);
        
        stageNumberText.text = $"Stage {stageIndex + 1}";
        stageDetailsText.text = stages[stageIndex].stageDetails;
        
        enterStageButton.interactable = (stageIndex <= currentStage);
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
            Vector3 newpos = new Vector3(-8, 2.1f, -6);
            //_teleportPlayer.TeleportPlayer(newpos);
            
            stageManager.ChangeMap(selectedStageIndex);
            // เรียกใช้ script เข้าด่านของคุณที่นี่
            // ตัวอย่าง: YourLevelLoader.LoadLevel(selectedStageIndex);
            Debug.Log($"Entering stage {selectedStageIndex + 1}");
            CloseStageDetails();
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
        // Update button colors
        for (int i = 0; i < stageButtonContainer.childCount; i++)
        {
            Image buttonImage = stageButtonContainer.GetChild(i).GetComponent<Image>();
            Button stageButton = stageButtonContainer.GetChild(i).GetComponent<Button>();

            if (i < currentStage)
            {
                buttonImage.color = completedColor;
            }
            else if (i == currentStage)
            {
                buttonImage.color = currentColor;
            }
            else
            {
                buttonImage.color = lockedColor;
            }

            stageButton.interactable = (i <= currentStage);
        }
    }
}