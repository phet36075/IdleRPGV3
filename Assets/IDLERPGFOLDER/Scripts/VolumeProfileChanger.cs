using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class VolumeProfileChanger : MonoBehaviour
{
    [Header("Volume Settings")]
    public Volume targetVolume;
    public VolumeProfile[] volumeProfiles;

    [Header("Light Prefabs")]
    public GameObject[] lightPrefabs;

    private GameObject currentLight;


    private void Start()
    {
        ChangeSceneSetup(1); // setup ที่ 1
    }

    // เปลี่ยน Volume Profile (index เริ่มจาก 1)
    public void ChangeVolumeProfile(int profileNumber)
    {
        int index = profileNumber - 1;
        if (index >= 0 && index < volumeProfiles.Length)
        {
            targetVolume.profile = volumeProfiles[index];
        }
        else
        {
            Debug.LogWarning($"Profile number {profileNumber} is invalid! (Valid range: 1-{volumeProfiles.Length})");
        }
    }

    // เปลี่ยน Directional Light (index เริ่มจาก 1)
    public void ChangeLightSetup(int lightNumber)
    {
        int index = lightNumber - 1;
        if (index >= 0 && index < lightPrefabs.Length)
        {
            if (currentLight != null)
            {
                Destroy(currentLight);
            }
            currentLight = Instantiate(lightPrefabs[index]);
        }
        else
        {
            Debug.LogWarning($"Light number {lightNumber} is invalid! (Valid range: 1-{lightPrefabs.Length})");
        }
    }

    // เปลี่ยนทั้ง Profile และ Light พร้อมกัน (index เริ่มจาก 1)
    public void ChangeSceneSetup(int setupNumber)
    {
        ChangeVolumeProfile(setupNumber);
        ChangeLightSetup(setupNumber);
    }

    // ตัวอย่างการใช้งาน
    void Update()
    {
        // ตัวอย่างการกดปุ่ม 1, 2 เพื่อเปลี่ยน setup
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeSceneSetup(1); // setup ที่ 1
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeSceneSetup(2); // setup ที่ 2
        }
    }
}