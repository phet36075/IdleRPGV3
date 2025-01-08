using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkill2 : MonoBehaviour
{
     public GameObject skillCirclePrefab;
        public float minDistance = 2f; // ระยะห่างขั้นต่ำระหว่างวงสกิล
        
        List<Vector3> spawnedPositions = new List<Vector3>();
    
        Vector3 GetRandomPosition(float radius)
        {
            for (int attempts = 0; attempts < 100; attempts++)
            {
                Vector2 randomCircle = Random.insideUnitCircle * radius;
                Vector3 position = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
                
                if (IsPositionValid(position))
                {
                    return position;
                }
            }
            
            Debug.LogWarning("ไม่สามารถหาตำแหน่งที่เหมาะสมได้หลังจากพยายาม 100 ครั้ง");
            return Vector3.zero;
        }
    
        bool IsPositionValid(Vector3 position)
        {
            foreach (Vector3 spawnedPos in spawnedPositions)
            {
                if (Vector3.Distance(position, spawnedPos) < minDistance)
                {
                    return false;
                }
            }
            return true;
        }
    
        public void SpawnSkillCircles(int count, float radius)
        {
            spawnedPositions.Clear();
        
            for (int i = 0; i < count; i++)
            {
                Vector3 randomPos = GetRandomPosition(radius);
                if (randomPos != Vector3.zero)
                {
                    // สร้างวงสกิลและหมุน -90 องศารอบแกน X
                    Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);
                    Instantiate(skillCirclePrefab, randomPos, rotation);
                    spawnedPositions.Add(randomPos);
                }
            }
        }

        public void StartAttackSkill2Boss()
        {
            SpawnSkillCircles(6, 5f); // สร้าง 4 วง ในรัศมี 5 หน่วย
        }
        void Start()
        {
            
        }
}
