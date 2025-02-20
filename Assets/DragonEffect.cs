using UnityEngine;

public class DragonEffect : MonoBehaviour
{
    public GameObject flameEffect;

    public void EnableFlame()
    {
        if (flameEffect != null)
        {
            flameEffect.SetActive(true);
        }
    }
    public void DisableFlame()
    {
        if (flameEffect != null)
        {
            flameEffect.SetActive(false);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DisableFlame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
