using System.Runtime.Serialization.Json;
using UnityEngine;

public class UIContainer : MonoBehaviour
{
    [SerializeField] private SkillDetailWindow detailWindow;
    public GameObject statsPanel;
    public GameObject skillInventoryPanel;
    public bool isSkillsInventoyryOpened;
    public bool isStatsOpened;
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
