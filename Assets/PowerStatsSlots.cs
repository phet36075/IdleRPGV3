using TMPro;
using UnityEngine;

public class PowerStatsSlots : MonoBehaviour
{
    //[SerializeField] private PlayerProperty playerProperty;
    public TextMeshProUGUI statText;
    public TextMeshProUGUI oldText;
    public TextMeshProUGUI newText;

    public void HandleStatsSlots(string statName, float oldValue, float newValue)
    {
        // แสดงการแจ้งเตือนหรือ Effect พิเศษเมื่อสถิติบางตัวเปลี่ยนแปลง
        Debug.Log($"Stat changed: {statName} from {oldValue} to {newValue}");


        statText.text = $"{statName}";
        oldText.text = oldValue.ToString();
        newText.text = newValue.ToString();
    }
}

// ตัวอย่าง: แสดงการแจ้งเตือนเมื่อค่า HP สูงสุดเพิ่มขึ้น
        // if (statName == "maxHealth" && newValue > oldValue)
        // {
        //       ShowStatIncreaseEffect("maxHealth", oldValue, newValue);
        // }
    
    // private void ShowStatIncreaseEffect(string statName, float oldValue, float newValue)
    // {
    //
    //     switch (statName)
    //     {
    //         case "maxHealth":
    //           
    //             // อาจจะเพิ่ม animation สีเขียวที่ตัวเลข
    //             break;
    //     }

       
        // if (statName == "maxHealth" && newValue < oldValue)
        // {
        //     maxHealthChange.text = "Health " + newValue + " " + oldValue;
        // }
       
        // แสดง animation หรือ visual effect เพื่อเน้นย้ำการเปลี่ยนแปลงของสถิติ
        // เช่น การส่องสว่างของไอคอนเกราะเมื่อค่า defense เพิ่มขึ้น
    

