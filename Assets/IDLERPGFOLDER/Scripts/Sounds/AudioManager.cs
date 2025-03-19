using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioSource uiAudioSource;
    
    public AudioClip hitSound, hitCritSound, MonsterPainSound, DieSound, HealSound;

    public AudioClip maceHitSound;
    public AudioClip spellHitSound;

     [Header("Player Sound")] 
    public List<AudioClip> jumpSounds;
    public List<AudioClip> attackSounds;
    public List<AudioClip> playerPainSounds;

    public void PlayJumpSound()
    {
      if (jumpSounds == null || jumpSounds.Count == 0 || audioSource == null)
        return;
        
         // Get a random jump sound from the list
     AudioClip randomJumpSound = jumpSounds[Random.Range(0, jumpSounds.Count)];
    
        // Play the selected jump sound
     audioSource.PlayOneShot(randomJumpSound);
    }
    public void PlayHealSound()
    {
        uiAudioSource.PlayOneShot(HealSound);
    }
    public void PlayHitSound()
    {
        audioSource.PlayOneShot(MonsterPainSound);
        audioSource.PlayOneShot(hitSound);
    }

    public void PlayPlayerPainSounds()
    {
        if (playerPainSounds == null || playerPainSounds.Count == 0 || audioSource == null)
            return;
        
        // Get a random jump sound from the list
        AudioClip randomPlayerPainSound = playerPainSounds[Random.Range(0, playerPainSounds.Count)];
    
        // Play the selected jump sound
        audioSource.PlayOneShot(randomPlayerPainSound);
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

    public void PlayAttackSound(int index)
    {
        audioSource.PlayOneShot(attackSounds[index]);
    }
   
    
    
}
