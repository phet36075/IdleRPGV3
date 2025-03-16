using TMPro;
using UnityEngine;

public class PowerStatsSlots : MonoBehaviour
{
    //[SerializeField] private PlayerProperty playerProperty;
    public TextMeshProUGUI statText;
    public TextMeshProUGUI oldText;
    public TextMeshProUGUI newText;
    public GameObject Up;
    public GameObject Down;
    public void HandleStatsSlots(string statName, float oldValue, float newValue)
    {
        // แสดงการแจ้งเตือนหรือ Effect พิเศษเมื่อสถิติบางตัวเปลี่ยนแปลง
        Debug.Log($"Stat changed: {statName} from {oldValue} to {newValue}");


        statText.text = $"{statName}";
        oldText.text = oldValue.ToString();
        newText.text = newValue.ToString();
        if (oldValue > newValue)
        {
            newText.color = Color.red;
            Down.gameObject.SetActive(true);
            Up.gameObject.SetActive(false);
        }else if (oldValue < newValue)
        {
            Up.gameObject.SetActive(true);
            Down.gameObject.SetActive(false);
            newText.color = Color.green;
        }
       
    }
}

    

