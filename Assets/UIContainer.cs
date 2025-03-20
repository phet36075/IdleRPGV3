using System.Runtime.Serialization.Json;
using UnityEngine;

public class UIContainer : MonoBehaviour
{
    [SerializeField] private SkillDetailWindow detailWindow;
    [SerializeField] private AllySkillDetailWindow allyDetailWindow;
    public GameObject statsPanel;
    public GameObject skillInventoryPanel;
    public GameObject allySkillInventoryPanel;
    public GameObject upgradeCanvas;
    public bool isSkillsInventoyryOpened;
    public bool isStatsOpened;
    public bool isAllySkillsInventoyryOpened;
    public bool isUpgradeCanvasOpened;
    private bool isInventoryOpen = false;
    [SerializeField] private GameObject uiContainerPanel;  // Panel หลักของ inventory
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isSkillsInventoyryOpened)
        {
            skillInventoryPanel.SetActive(true);
            statsPanel.SetActive(false);
        }
        else
        {
            skillInventoryPanel.SetActive(false);
            detailWindow.Hide();
        }
        
        if (isStatsOpened)
        {
            skillInventoryPanel.SetActive(false);
            statsPanel.SetActive(true);
            detailWindow.Hide();
        }
        else
        {
            statsPanel.SetActive(false);
        }

        if (isAllySkillsInventoyryOpened)
        {
            allySkillInventoryPanel.SetActive(true);
            
        }
        else
        {
            allySkillInventoryPanel.SetActive(false);
            allyDetailWindow.Hide();
        }

        if (isUpgradeCanvasOpened)
        {
            upgradeCanvas.SetActive(true);
        }
        else
        {
            upgradeCanvas.SetActive(false);
        }



        if (Input.GetKeyDown(KeyCode.Escape))
        {
           Hide();
        }
    }

    public void OnStatsClick()
    {
        HideOnlySubUI();
        isStatsOpened = true;
       
    }
    public void OnSkillInventoryClick()
    {
        HideOnlySubUI();
        isSkillsInventoyryOpened = true;
        
    }
    public void OnAllySkillInventoryClick()
    {
        HideOnlySubUI();
        isAllySkillsInventoyryOpened = true;
    }

    public void OnUpgradeCanvasClick()
    {
        HideOnlySubUI();
        isUpgradeCanvasOpened = true;
    }
    public void Show()
    {
       
       
    }

    public void Hide()
    {
      //  isInventoryOpen = false;
        uiContainerPanel.SetActive(false);
        isStatsOpened = false;
        isSkillsInventoyryOpened = false;
        isAllySkillsInventoyryOpened = false;
        allyDetailWindow.Hide();
        detailWindow.Hide();
        isUpgradeCanvasOpened = false;

    }

    public void HideOnlySubUI()
    {
        isStatsOpened = false;
        isSkillsInventoyryOpened = false;
        isAllySkillsInventoyryOpened = false;
        allyDetailWindow.Hide();
        detailWindow.Hide();
        isUpgradeCanvasOpened = false;
    }
    // public void Toggle()
    // {
    //     if (isInventoryOpen)
    //     {
    //         Hide();
    //     }
    //     else
    //     {
    //         Show();
    //     }
    // }
}
