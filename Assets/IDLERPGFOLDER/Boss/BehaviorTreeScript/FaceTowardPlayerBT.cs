using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class FaceTowardPlayerBT : Action
{
    public SharedTransform playerTransform;
    public float rotationSpeed = 5f;
    
    private Coroutine facePlayerCoroutine;
    private bool isRotationComplete = false;

    public override void OnStart()
    {
        isRotationComplete = false;
        facePlayerCoroutine = StartCoroutine(FacePlayer());
    }

    public override TaskStatus OnUpdate()
    {
        if (isRotationComplete)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }
    
    
    
    IEnumerator FacePlayer()
    {
        if (playerTransform.Value != null)
        {
            // คำนวณทิศทางไปยัง player
            Vector3 direction = playerTransform.Value.position - transform.position;
            direction.y = 0; // ไม่หมุนในแกน y (ขึ้น-ลง)
        
            if (direction != Vector3.zero)
            {
                // คำนวณ rotation เป้าหมาย
                Quaternion targetRotation = Quaternion.LookRotation(direction);
            
                // หมุนช้าๆ จนกว่าจะถึงเป้าหมาย
                float angle = Quaternion.Angle(transform.rotation, targetRotation);
            
                // ทำการหมุนจนกว่าจะหันไปหา player เกือบสมบูรณ์ (น้อยกว่า 2 องศา)
                while (angle > 2f)
                {
                    // ค่อยๆ หมุนด้วย Slerp
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation, 
                        targetRotation, 
                        rotationSpeed * Time.deltaTime
                    );
                
                    // คำนวณมุมที่เหลืออยู่
                    angle = Quaternion.Angle(transform.rotation, targetRotation);
                
                    // รอจนกว่าจะถึงเฟรมถัดไป
                    yield return null;
                }
            
                // หมุนให้ตรงกับเป้าหมายเมื่อใกล้ถึง
                transform.rotation = targetRotation;
            }
        }
        
        // เมื่อทำการหมุนเสร็จสิ้น
        isRotationComplete = true;
    }
}