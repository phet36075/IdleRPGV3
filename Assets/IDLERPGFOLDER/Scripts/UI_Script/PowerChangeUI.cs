using System.Collections;
using UnityEngine;
using TMPro; // ใช้กับ TextMeshPro

public class PowerChangeUI : MonoBehaviour
{
    public TextMeshProUGUI powerChangeText;
    public float fadeDuration = 1.5f; // ระยะเวลาที่ข้อความจะหายไป

    private void Start()
    {
        powerChangeText.alpha = 0; // ซ่อนข้อความตอนเริ่ม
    }

    public void ShowPowerChange(int amount)
    {
        powerChangeText.text = (amount > 0 ? "+" : "") + amount; // แสดง + ถ้าเป็นค่าบวก
        StartCoroutine(FadeText());
    }

    private IEnumerator FadeText()
    {
        powerChangeText.alpha = 1; // แสดงข้อความ
        yield return new WaitForSeconds(1f); // รอ 1 วิ
       
        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            powerChangeText.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration); // ค่อย ๆ จางลง
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        powerChangeText.alpha = 0; // ซ่อนเมื่อจบ
    }
}