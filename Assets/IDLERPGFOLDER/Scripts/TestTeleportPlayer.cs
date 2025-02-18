using UnityEngine;
using System.Collections;

public class TestTeleportPlayer : MonoBehaviour
{
    private Vector3 targetPosition;
    private bool isTeleporting = false;
    private Coroutine teleportCoroutine;
    private CharacterController characterController;
    public Transform AllyPosition;
    public CanvasGroup stageUI;
    public float fadeDuration = 0.5f; // ระยะเวลาการจางเข้า-ออก
    public float displayTime = 2f; // เวลาที่ UI จะคงอยู่ก่อนจางออก
    private void Start()
    {
        stageUI.alpha = 0;
        characterController = GetComponent<CharacterController>();
        ShowUI();
    }

    public void TeleportPlayer(Vector3 newPosition)
    {
        if (isTeleporting)
        {
            Debug.Log("กำลังเคลื่อนย้ายอยู่แล้ว กรุณารอสักครู่");
            return;
        }

        isTeleporting = true;
        targetPosition = newPosition;

        if (teleportCoroutine != null)
        {
            StopCoroutine(teleportCoroutine);
        }
        teleportCoroutine = StartCoroutine(TeleportCoroutine());
    }

    private IEnumerator TeleportCoroutine()
    {
        Debug.Log("เริ่มการเคลื่อนย้าย");
        TransitionManager.Instance.StartTransition(
            "Travelling...",
            () => { Debug.Log("เริ่ม fade in"); },
            () => { Debug.Log("เริ่ม fade out"); }
        );

        // รอให้ fade in เสร็จสิ้น
        yield return new WaitForSeconds(TransitionManager.Instance.fadeDuration);

        // ปิดการทำงานของ Character Controller
        characterController.enabled = false;

        // ทำการย้ายตำแหน่ง
        Debug.Log("กำลังย้ายผู้เล่นไปยังตำแหน่ง: " + targetPosition);
        transform.position = targetPosition;
        AllyPosition.transform.position = targetPosition;
        Debug.Log("ตำแหน่งปัจจุบันของผู้เล่น: " + transform.position);

        // เปิดการทำงานของ Character Controller อีกครั้ง
        characterController.enabled = true;

        // รอให้ fade out เสร็จสิ้น
        yield return new WaitForSeconds(TransitionManager.Instance.fadeDuration);
        ShowUI();
        Debug.Log("การเคลื่อนย้ายเสร็จสิ้น");
        isTeleporting = false;
    }
    public void ShowUI()
    {
        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float time = 0;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            stageUI.alpha = Mathf.Lerp(0, 1, time / fadeDuration);
            yield return null;
        }
        stageUI.alpha = 1;
        yield return new WaitForSeconds(displayTime);
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float time = 0;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            stageUI.alpha = Mathf.Lerp(1, 0, time / fadeDuration);
            yield return null;
        }
        stageUI.alpha = 0;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("ปุ่ม K ถูกกด");
            Vector3 newpos = new Vector3(-8, 2.1f, -6);
            TeleportPlayer(newpos);
        }
    }
}