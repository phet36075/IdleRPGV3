using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class PlayerAttackBT : Action
{
  
//  private AllyManager _allyManager;
 // private WeaponSystem _weaponSystem;
    
  public Animator animator;
  private PlayerMovement playerMovement;
 // public AllyRangedCombat rangedAllies;  // เพิ่มอาร์เรย์ของพวกพ้องที่โจมตีระยะไกล
 public SharedTransform target;
  private int _comboStep;
  private float _lastAttackTime;
  public float comboCooldown = 1f;
  public bool isAttacking;
  private Transform _vfxPos;
  public GameObject attackVFX;
  // [FormerlySerializedAs("_Trail")] public Trail trail;
    
  public float detectionRadius = 5f; // รัศมีในการตรวจจับศัตรู
  public float moveSpeed = 5f; // ความเร็วในการเคลื่อนที่
  public float attackRange = 2f; // ระยะโจมตี
  public float attackRadius = 1f; // รัศมีการโจมตี
  private Transform _nearestEnemy; // เก็บ Transform ของศัตรูที่ใกล้ที่สุด
  private bool _isMovingToEnemy; // เพิ่มตัวแปรเพื่อตรวจสอบว่ากำลังเคลื่อนที่หาศัตรูหรือไม่
    
  private float _currentSpeed; // เพิ่มตัวแปรเก็บความเร็วปัจจุบัน
    
  public Transform attackPoint; // จุดศูนย์กลางของการโจมตี
  public LayerMask enemyLayers; // Layer ของศัตรู
  private float nextAttackTime;

  public override void OnStart()
  {
    PlayerAttack playerAttack = GetComponent<PlayerAttack>();
    // _allyManager = GetComponent<AllyManager>();
    //   _weaponSystem = GetComponent<WeaponSystem>();
//playerMovement = GetComponent<PlayerMovement>();
  }
  
  public override TaskStatus OnUpdate()
  {
    if (Time.time - _lastAttackTime > 0.3f)
    {
     // _comboStep = 0;
      isAttacking = false;
    }
RotateTowardsTarget();

    if (Time.time >= _lastAttackTime + comboCooldown)
    {
      Attack();
      return TaskStatus.Success;
    }
     
    return TaskStatus.Running;
  }
  
  public void Attack()
  {
    if (!isAttacking)
    {
     // nextAttackTime = Time.time;
      _lastAttackTime = Time.time;
      isAttacking = true;
      _comboStep++;
      PerformAttackAnimation();
    }
  }
  
  
  private void PerformAttackAnimation()
  {
    // หยุดเล่น animation การเดินก่อนเริ่ม animation การโจมตี
    StopMoving();

    if (_comboStep == 1)
    {
      animator.SetTrigger("Attack1");
    }
    else if (_comboStep == 2)
    {
      animator.SetTrigger("Attack2");
    }
    else if (_comboStep == 3)    
    {
      animator.SetTrigger("Attack3");
      _comboStep = 0;
      animator.ResetTrigger("Attack1");
      animator.ResetTrigger("Attack2");
            
    }
  }
  
  void RotateTowardsTarget()
  {
    Vector3 direction = (target.Value.position - transform.position).normalized;
    //Debug.Log("Direction: " + direction);  // ตรวจสอบทิศทางการหมุน
    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
    //Debug.Log("Target Rotation: " + lookRotation.eulerAngles);  // ตรวจสอบการหมุนที่ควรจะเป็น
    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3);
  }
  private void StopMoving()
  {
    // หยุดเล่น animation การเดิน
    _currentSpeed = 0f;
  }
}
