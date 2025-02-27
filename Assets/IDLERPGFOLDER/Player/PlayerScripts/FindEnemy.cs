using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class FindEnemy : Action
{
    public SharedTransform target;

    public override void OnStart()
    {
        FindNearestEnemy();
    }

    public override TaskStatus OnUpdate()
    {
        if (target.Value != null)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Failure;
    }
    
    public void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            EnemyHealth enemyScript = enemy.GetComponent<EnemyHealth>();

            if (enemyScript != null && !enemyScript.GetIsDead())
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestEnemy = enemy;
                }
            }
            
        }

        if (nearestEnemy != null)
        {
            target.Value = nearestEnemy.transform;
            
        }
        else
        {
            target.Value = null; // ไม่มีศัตรูในระยะ
          
        }
    }
}
