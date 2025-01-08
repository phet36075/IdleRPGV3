using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class TestProjectorCircleFade : MonoBehaviour
{
   public float fadeDuration = 2f;
   public float maxProjectionDepth = 1f;
   
   
    public DecalProjector decalProjector;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TestFade());
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    IEnumerator TestFade()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float depth = Mathf.Lerp(0f, maxProjectionDepth, elapsedTime / fadeDuration);

            decalProjector.fadeFactor = depth;
            
           
            yield return null;
        }
    }
}
