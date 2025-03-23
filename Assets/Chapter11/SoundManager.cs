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
            // กำหนดค่า slider จาก ScriptableObject
            m_SliderMasterVolume.value = m_SoundSettings.MasterVolume;
            m_SliderMusicVolume.value = m_SoundSettings.MusicVolume;
            m_SliderMasterSFXVolume.value = m_SoundSettings.MasterSFXVolume;
            m_SliderSFXVolume.value = m_SoundSettings.SFXVolume;
            m_SliderUIVolume.value = m_SoundSettings.UIVolume;

            // ตั้งค่า mixer จาก ScriptableObject
            m_SoundSettings.AudioMixer.SetFloat(m_SoundSettings.MasterVolumeName, m_SoundSettings.MasterVolume);
            m_SoundSettings.AudioMixer.SetFloat(m_SoundSettings.MusicVolumeName, m_SoundSettings.MusicVolume);
            m_SoundSettings.AudioMixer.SetFloat(m_SoundSettings.MasterSFXVolumeName, m_SoundSettings.MasterSFXVolume);
            m_SoundSettings.AudioMixer.SetFloat(m_SoundSettings.SFXVolumeName, m_SoundSettings.SFXVolume);
            m_SoundSettings.AudioMixer.SetFloat(m_SoundSettings.UIVolumeName, m_SoundSettings.UIVolume);
        }


        public void SetMasterVolume(float vol)
        {
            vol = m_SliderMasterVolume.value;
            // set float to audiomixer
            m_SoundSettings.AudioMixer.SetFloat(m_SoundSettings.MasterVolumeName, vol);
            
            //set float to SO to persist the value although the game is closed
            m_SoundSettings.MasterVolume = vol;
        }

        public void SetMusicVolume(float vol)
        {
            vol = m_SliderMusicVolume.value;
            m_SoundSettings.AudioMixer.SetFloat(m_SoundSettings.MusicVolumeName, vol);
            
            m_SoundSettings.MusicVolume = vol;  // แก้ไข - ต้องเป็น MusicVolume ไม่ใช่ MasterVolume
        }

        public void SetMasterSFXVolume(float vol)
        {
            vol = m_SliderMasterSFXVolume.value;
            m_SoundSettings.AudioMixer.SetFloat(m_SoundSettings.MasterSFXVolumeName, vol);
            
            m_SoundSettings.MasterSFXVolume = vol;
        }

        public void SetSFXVolume(float vol)
        {
            vol = m_SliderSFXVolume.value;
            m_SoundSettings.AudioMixer.SetFloat(m_SoundSettings.SFXVolumeName, vol);
            
            m_SoundSettings.SFXVolume = vol;
        }
        
        public void SetUIVolume(float vol)
        {
            vol = m_SliderUIVolume.value;
            m_SoundSettings.AudioMixer.SetFloat(m_SoundSettings.UIVolumeName, vol);
            
            m_SoundSettings.UIVolume = vol;
        }
    }
}