using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip hitSound,hitCritSound , MonsterPainSound,DieSound;

    public AudioClip maceHitSound;
    public AudioClip spellHitSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void PlayHitSound()
    {
        audioSource.PlayOneShot(MonsterPainSound);
        audioSource.PlayOneShot(hitSound);
        
    }
    public void PlayMaceHitSound()
    {
        audioSource.PlayOneShot(MonsterPainSound);
        audioSource.PlayOneShot(maceHitSound);
        
    }
    public void PlaySpellHitSound()
    {
        audioSource.PlayOneShot(MonsterPainSound);
        audioSource.PlayOneShot(spellHitSound);
        
    }
    public void PlayHitCritSound()
    {
        audioSource.PlayOneShot(MonsterPainSound);
        audioSource.PlayOneShot(hitCritSound);
    }

    public void PlayDieSound()
    {
        audioSource.PlayOneShot(DieSound);
    }
}
