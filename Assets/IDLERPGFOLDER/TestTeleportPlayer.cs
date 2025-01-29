using UnityEngine;
using System.Collections;

public class TestTeleportPlayer : MonoBehaviour
{
    private Vector3 targetPosition;
    private bool isTeleporting = false;
    private Coroutine teleportCoroutine;
    private CharacterController characterController;
    public Transform AllyPosition;
    private void Start()
    {
        characterController = GetComponent<CharacterController>();
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

        Debug.Log("การเคลื่อนย้ายเสร็จสิ้น");
        isTeleporting = false;
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