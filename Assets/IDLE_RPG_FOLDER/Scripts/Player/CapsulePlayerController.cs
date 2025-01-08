using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CapsulePlayerController : MonoBehaviour
{
    [SerializeField] private float m_RotationSpeed = 180;
    [SerializeField] private float m_DirectionalSpeed = 3;
    [SerializeField] private float m_DirectionalSprintSpeed = 5;

    [Header("Key Config")] 
    [SerializeField] protected Key m_ForwardKey = Key.W;
    [SerializeField] protected Key m_BackwardKey = Key.S;
    [SerializeField] protected Key m_TurnLeftKey = Key.A;
    [SerializeField] protected Key m_TurnRightKey = Key.D;

    [SerializeField] protected Rigidbody _rigidbody;

    public Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(_rigidbody.velocity);
        
        
        float speedMagnitude = m_DirectionalSpeed;
        
        Keyboard keyboard = Keyboard.current;

        if (keyboard[m_TurnLeftKey].isPressed)
        {
            
            transform.Rotate(transform.up,-m_RotationSpeed*Time.deltaTime,Space.Self);
        }
        else if (keyboard[m_TurnRightKey].isPressed)
        {
            transform.Rotate(transform.up,m_RotationSpeed*Time.deltaTime,Space.Self);
        }
        
        
        if (keyboard[Key.LeftShift].isPressed)
        {
            animator.SetBool("IsRunning", true);
            speedMagnitude = m_DirectionalSprintSpeed;
        }

        if (keyboard[m_ForwardKey].isPressed)
        {
            animator.SetBool("IsWalking", true);
            transform.Translate(transform.forward*speedMagnitude*Time.deltaTime,Space.World);
            
        }
       
        
        else if (keyboard[m_BackwardKey].isPressed)
        {
            transform.Translate(transform.forward*-speedMagnitude*0.4f*Time.deltaTime,Space.World);
        }
        
        else if (_rigidbody.velocity.magnitude > 0)
        {
            animator.SetBool("IsWalking", true);
            
        }
        
        else if(_rigidbody.velocity.magnitude <=0)
        {  
            animator.SetBool("IsWalking", false);
            
        }else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            animator.SetBool("IsRunning", false);
        }
       

        {
            
        }

    }
}
