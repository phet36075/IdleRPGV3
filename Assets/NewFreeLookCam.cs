using UnityEngine;
using Cinemachine;
public class NewFreeLookCam : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera;
    public Transform character;
    public string mouseXInputName = "Mouse X";
    public string mouseYInputName = "Mouse Y";
    public string mouseScrollInputName = "Mouse ScrollWheel";
    public float zoomSpeed = 2f;
    public float minZoom = 1f;
    public float maxZoom = 10f;
    public float lookAtCameraDistance = 3f;
    public float rotationSpeedMultiplier = 2f;
    
    // เพิ่มตัวแปรสำหรับควบคุมการเปลี่ยน height ของ TopRig
    public float maxTopRigHeight = 4f;  // ความสูงสูงสุดของ TopRig
    public float minTopRigHeight = 1f;  // ความสูงต่ำสุดของ TopRig (ใกล้เคียงกับ MiddleRig)
    
    private float currentZoom = 5f;
    private float initialTopRigHeight;  // เก็บค่าความสูงเริ่มต้นของ TopRig

    void Start()
    {
        freeLookCamera.m_XAxis.m_MaxSpeed = 300f * rotationSpeedMultiplier;
        freeLookCamera.m_YAxis.m_MaxSpeed = 2f * rotationSpeedMultiplier;
        currentZoom = freeLookCamera.m_Orbits[1].m_Radius;
        
        // เก็บค่าความสูงเริ่มต้นของ TopRig
        initialTopRigHeight = freeLookCamera.m_Orbits[0].m_Height;
    }

    void Update()
    {
        if (Input.GetMouseButton(1)) 
        {
            freeLookCamera.m_XAxis.m_InputAxisName = mouseXInputName;
            freeLookCamera.m_YAxis.m_InputAxisName = mouseYInputName;
        }
        else
        {
            freeLookCamera.m_XAxis.m_InputAxisValue = 0f;
            freeLookCamera.m_YAxis.m_InputAxisValue = 0f;
            freeLookCamera.m_XAxis.m_InputAxisName = "";
            freeLookCamera.m_YAxis.m_InputAxisName = "";
        }

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            currentZoom -= scrollInput * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

            AdjustCameraZoom(currentZoom);
        }
        
        if (currentZoom <= lookAtCameraDistance)
        {
            LookAtCamera();
        }
    }

    void AdjustCameraZoom(float zoom)
    {
        // คำนวณสัดส่วนการซูมเทียบกับระยะทั้งหมด
        float zoomRatio = Mathf.InverseLerp(minZoom, maxZoom, zoom);
        
        // ปรับความสูงของ TopRig ให้ลดลงเมื่อซูมเข้าใกล้
        float targetTopRigHeight = Mathf.Lerp(minTopRigHeight, maxTopRigHeight, zoomRatio);
        freeLookCamera.m_Orbits[0].m_Height = targetTopRigHeight;
        
        // ปรับ radius ของแต่ละ rig
        freeLookCamera.m_Orbits[0].m_Radius = zoom * 0.8f; // Top Rig
        freeLookCamera.m_Orbits[1].m_Radius = zoom;        // Middle Rig
        //freeLookCamera.m_Orbits[2].m_Radius = zoom * 1.2f; // Bottom Rig
    }

    void LookAtCamera()
    {
        Vector3 directionToCamera = freeLookCamera.transform.position - character.position;
        directionToCamera.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(directionToCamera);
        character.rotation = Quaternion.Slerp(character.rotation, lookRotation, Time.deltaTime * 5f);
    }
}
