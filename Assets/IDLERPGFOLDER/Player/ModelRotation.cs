using UnityEngine;

public class ModelRotation : MonoBehaviour
{
    [Tooltip("ความเร็วในการหมุน (องศาต่อวินาที)")]
    public float rotationSpeed = 30f;
    
    [Tooltip("หมุนตามแกน X")]
    public bool rotateX = false;
    
    [Tooltip("หมุนตามแกน Y")]
    public bool rotateY = true;
    
    [Tooltip("หมุนตามแกน Z")]
    public bool rotateZ = false;
    
    [Tooltip("หมุนเมื่อกดปุ่มเมาส์ซ้าย")]
    public bool rotateOnMouseDrag = false;
    
    private Vector3 rotation;
    
    void Update()
    {
        // หมุนอัตโนมัติ
        if (!rotateOnMouseDrag || (rotateOnMouseDrag && Input.GetMouseButton(0)))
        {
            rotation = Vector3.zero;
            
            if (rotateX)
                rotation.x = rotationSpeed * Time.deltaTime;
            
            if (rotateY)
                rotation.y = rotationSpeed * Time.deltaTime;
            
            if (rotateZ)
                rotation.z = rotationSpeed * Time.deltaTime;
            
            transform.Rotate(rotation);
        }
    }
}