using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkController : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public int blendShapeIndex;
    public float minBlinkInterval = 3.0f;
    public float maxBlinkInterval = 7.0f;
    public float blinkDuration = 0.1f;
    public float blendSpeed;
    private float nextBlinkTime;
    private bool isBlinking = false;

    void Start()
    {
        ScheduleNextBlink();
    }

    void Update()
    {
        float weight = Time.time * blendSpeed;
        if (Time.time >= nextBlinkTime && !isBlinking)
        {
            StartCoroutine(Blink());
        }
    }

    void ScheduleNextBlink()
    {
        nextBlinkTime = Time.time + Random.Range(minBlinkInterval, maxBlinkInterval);
    }

    System.Collections.IEnumerator Blink()
    {
        isBlinking = true;
        skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, 100);
        yield return new WaitForSeconds(blinkDuration);
        skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, 0);
        isBlinking = false;
        ScheduleNextBlink();
    }
   
}
