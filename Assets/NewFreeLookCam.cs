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

    private float currentZoom = 5f; // ค่าระยะห่างเริ่มต้น

    void Start()
    {
        freeLookCamera.m_XAxis.m_MaxSpeed = 300f * rotationSpeedMultiplier;
        freeLookCamera.m_YAxis.m_MaxSpeed = 2f * rotationSpeedMultiplier;
        currentZoom = freeLookCamera.m_Orbits[1].m_Radius; // ตั้งค่าระยะเริ่มต้นจากค่า MiddleRig
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

        // ซูมโดยขยับระยะของกล้องแทนการปรับ Field of View
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
        freeLookCamera.m_Orbits[0].m_Radius = zoom * 0.8f; // Top Rig
        freeLookCamera.m_Orbits[1].m_Radius = zoom;        // Middle Rig
       // freeLookCamera.m_Orbits[2].m_Radius = zoom * 1.2f; // Bottom Rig
    }

    void LookAtCamera()
    {
        Vector3 directionToCamera = freeLookCamera.transform.position - character.position;
        directionToCamera.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(directionToCamera);
        character.rotation = Quaternion.Slerp(character.rotation, lookRotation, Time.deltaTime * 5f);
    }
}
