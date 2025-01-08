using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
public class PlayerController : MonoBehaviour
{
    public AIController aiController;
    public NavMeshAgent agent;
    public bool isAIEnabled = true;
    public bool isAIActive = true;
    private bool wasManualInputActive = false;

    public TextMeshProUGUI OnOffTxT;
    void Update()
    {
        if (isAIEnabled == true)
        {
            OnOffTxT.text = "On";
        }else if (isAIEnabled == false)
        {
            OnOffTxT.text = "Off";
        }
        
        if (Input.GetKeyDown(KeyCode.T)) // ปุ่มเพื่อสลับระหว่างผู้เล่นและ AI
        {
            ToggleAI();
        }
        
       /* if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            aiController.enabled = false;
            agent.enabled = false;
            GetComponent<CharacterController>().enabled = true;
        }*/
        
        bool isManualInputActive = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;

        if (isAIEnabled && isAIActive)
        {
            if (isManualInputActive)
            {
                isAIActive = false;
                wasManualInputActive = true;
            }
            else
            {
                aiController.enabled = true;
                agent.enabled = true;
                GetComponent<CharacterController>().enabled = false;
            }
        }
        else
        {
            if (!isManualInputActive && wasManualInputActive && isAIEnabled)
            {
                isAIActive = true;
                wasManualInputActive = false;
            }
            else
            {
                aiController.enabled = false;
                agent.enabled = false;
                GetComponent<CharacterController>().enabled = true;
            }
        }
       
    }

    public void ToggleAI()
    {
       
       // isAIEnabled = aiController.enabled;
        isAIEnabled = !isAIEnabled;
        isAIActive = isAIEnabled;
        
        aiController.enabled = isAIEnabled;
        agent.enabled = isAIEnabled;
        
        Debug.Log("AI " + (isAIEnabled ? "Enabled" : "Disabled"));
        if (!isAIEnabled) // ถ้ากำลังจะเปลี่ยนไป AI Mode
        {
            aiController.FindNearestEnemy();
        }
        // ปิด/เปิดการควบคุมผู้เล่น (เช่น Input ของ Character Controller)
        GetComponent<CharacterController>().enabled = !isAIEnabled;
    }
}
