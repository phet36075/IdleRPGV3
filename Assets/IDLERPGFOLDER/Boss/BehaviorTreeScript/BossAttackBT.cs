using System.Collections;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    public class BossAttackBT : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The name of the parameter")]
        public SharedString paramaterName;
        public SharedFloat arriveDistance = 0.2f;
        public SharedGameObject target;
        private Animator animator;
        private GameObject prevGameObject;
        [Header("Enemy Settings")]
       // [SerializeField] private float detectionRange = 10f;  // ระยะการตรวจจับ Player
       // [SerializeField] public float attackRange = 2f;      // ระยะโจมตี
        public float attackCooldown = 2f;   // ระยะเวลารอระหว่างการโจมตี
        private bool screamFirstTime;
        private float nextAttackTime;
        private bool isAttacking;
        private int attackCount;
        private bool isFlyingTowardPlayer;
        public SharedVector3 targetPosition;
        public float rotationSpeed = 4;
        protected UnityEngine.AI.NavMeshAgent navMeshAgent;
        private Transform player;
        public SharedFloat speed = 3;
        public SharedFloat angularSpeed = 45;
        
        public override void OnAwake()
        {
            navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        }
        
        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject) {
                animator = currentGameObject.GetComponent<Animator>();
                prevGameObject = currentGameObject;
            }
            player = GameObject.FindGameObjectWithTag("Player").transform;
            SetDestination(player.position);
          
            navMeshAgent.speed = speed.Value;
            navMeshAgent.angularSpeed = angularSpeed.Value;
            
        }
        private Vector3 Target()
        {
            if (target.Value != null) {
                return target.Value.transform.position;
            }
            return targetPosition.Value;
        }
       
        public override TaskStatus OnUpdate()
        {
            if (animator == null) {
                Debug.LogWarning("Animator is null");
                return TaskStatus.Failure;
            }

          //  animator.SetTrigger(paramaterName.Value);
         

          
          
          if (!CloseToPlayer())
          {
              SetDestination(player.position);
          }
          else
          {
              navMeshAgent.isStopped = true;
              if (Time.time >= nextAttackTime)
                  Attack();
                  StartCoroutine(FacePlayer());
                
          }
              
              
          if (attackCount >= 3)
          {
              animator.ResetTrigger("ClawAttack");
              attackCount = 0;
              return TaskStatus.Success;
             
          }
               // animator.SetTrigger(paramaterName.Value);
               return TaskStatus.Running;
        }
        
        private void Attack()
        {
            nextAttackTime = Time.time + attackCooldown;
            attackCount++;
            animator.SetTrigger("ClawAttack");
        }
        public override void OnReset()
        {
            targetGameObject = null;
            paramaterName = "";
        }
        IEnumerator FacePlayer()
        {
            if (player != null)
            {
                // คำนวณทิศทางไปยัง player
                Vector3 direction = player.position - transform.position;
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
        }
        private bool CloseToPlayer()
        {
            navMeshAgent.isStopped = false;
            // The path hasn't been computed yet if the path is pending.
            float remainingDistance;
            if (navMeshAgent.pathPending) {
                remainingDistance = float.PositiveInfinity;
            } else {
                remainingDistance = navMeshAgent.remainingDistance;
            }

            return remainingDistance <= arriveDistance.Value;
        }
        private bool SetDestination(Vector3 destination)
        {
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5
            navMeshAgent.Resume();
#else
            navMeshAgent.isStopped = false;
#endif
            return navMeshAgent.SetDestination(destination);
        }
        
    }
}
