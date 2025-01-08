using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHitEffect : MonoBehaviour
{
    public float effectDuration = 0.2f;
    public Color hitColor = Color.red;

    private Renderer characterRenderer;
    private Material originalMaterial;
    private Color originalColor;

    void Start()
    {
        characterRenderer = GetComponent<Renderer>();
        originalMaterial = characterRenderer.material;
        originalColor = originalMaterial.color;
    }

    public void StartHitEffect()
    {
        StopAllCoroutines();
        StartCoroutine(HitEffectCoroutine());
    }

    private IEnumerator HitEffectCoroutine()
    {
        // เปลี่ยนสีเป็นสีแดง
        characterRenderer.material.color = hitColor;

        // รอตามเวลาที่กำหนด
        yield return new WaitForSeconds(effectDuration);

        // เปลี่ยนกลับเป็นสีเดิม
        characterRenderer.material.color = originalColor;
    }
}
