using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UIElements;
using UnityEngine.UI;


namespace Petchcious.GameDev3.Chapter11
{
    public class SoundManager : MonoBehaviour
    {
        private bool isVisible;
        public GameObject soundPanel;
        [SerializeField] protected SoundSettings m_SoundSettings;

        public Slider m_SliderMasterVolume;
        public Slider m_SliderMusicVolume;
        public Slider m_SliderMasterSFXVolume;
        public Slider m_SliderSFXVolume;
        public Slider m_SliderUIVolume;
      
       
        void Start()
        {
        InitialiseVolume();
        isVisible = false;
        }
        public void ToggleSoundPanel()
        {
            isVisible = !isVisible;
           soundPanel.gameObject.SetActive(isVisible);
        }
        private void InitialiseVolume()
        {
            SetMasterVolume(m_SoundSettings.MasterVolume);
             SetMusicVolume(m_SoundSettings.MusicVolume);
             SetMasterSFXVolume(m_SoundSettings.MasterSFXVolume);
             SetSFXVolume(m_SoundSettings.SFXVolume);
             SetUIVolume(m_SoundSettings.UIVolume);
        }


        public void SetMasterVolume(float vol)
        {
            vol = m_SliderMasterVolume.value;
            // set float to audiomixer
            m_SoundSettings.AudioMixer.SetFloat(m_SoundSettings.MasterVolumeName, vol);
            
            //set float to SO to persist the value although the game is closed
            m_SoundSettings.MasterVolume = vol;
            // set the slider bar's value
            //m_SliderMasterVolume.value = m_SoundSettings.MasterVolume;
        }

        public void SetMusicVolume(float vol)
        {
            vol = m_SliderMusicVolume.value;
            m_SoundSettings.AudioMixer.SetFloat(m_SoundSettings.MusicVolumeName, vol);
            
            m_SoundSettings.MasterVolume = vol;

          //  m_SliderMusicVolume.value = m_SoundSettings.MusicVolume;
            
        }

        public void SetMasterSFXVolume(float vol)
        {
            vol = m_SliderMasterSFXVolume.value;
            m_SoundSettings.AudioMixer.SetFloat(m_SoundSettings.MasterSFXVolumeName, vol);
            
            m_SoundSettings.MasterSFXVolume = vol;

           // m_SliderMasterSFXVolume.value = m_SoundSettings.MasterSFXVolume;
        }
        public void SetSFXVolume(float vol)
        {
            vol = m_SliderSFXVolume.value;
            m_SoundSettings.AudioMixer.SetFloat(m_SoundSettings.SFXVolumeName, vol);
            
            m_SoundSettings.SFXVolume = vol;

          //  m_SliderSFXVolume.value = m_SoundSettings.SFXVolume;
        }
        
        public void SetUIVolume(float vol)
        {
            vol = m_SliderUIVolume.value;
            m_SoundSettings.AudioMixer.SetFloat(m_SoundSettings.UIVolumeName, vol);
            
            m_SoundSettings.UIVolume = vol;

          //  m_SliderUIVolume.value = m_SoundSettings.UIVolume;
        }
    }
}

