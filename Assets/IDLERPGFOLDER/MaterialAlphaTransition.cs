using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialAlphaTransition : MonoBehaviour
{
    [System.Serializable]
    public class ObjectToTransition
    {
        public Renderer objectRenderer;
        public float startAlpha = 150f / 255f;
        public float endAlpha = 1f;
    }

    public List<ObjectToTransition> objectsToTransition = new List<ObjectToTransition>();
    public float transitionDuration = 1f;

    private float elapsedTime;
    private bool isTransitioning = false;

    private void Start()
    {
        StartGroupTransition();
    }

    public void StartGroupTransition()
    {
        elapsedTime = 0f;
        isTransitioning = true;

        // Initialize all objects with their start alpha
        foreach (var obj in objectsToTransition)
        {
            SetAlpha(obj.objectRenderer, obj.startAlpha);
        }
    }

    void Update()
    {
        if (isTransitioning && elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);

            foreach (var obj in objectsToTransition)
            {
                float currentAlpha = Mathf.Lerp(obj.startAlpha, obj.endAlpha, t);
                SetAlpha(obj.objectRenderer, currentAlpha);
            }

            if (t >= 1f)
            {
                isTransitioning = false;
            }
        }
    }

    void SetAlpha(Renderer renderer, float alpha)
    {
        if (renderer != null)
        {
            Color color = renderer.material.color;
            color.a = alpha;
            renderer.material.color = color;
        }
    }
}
