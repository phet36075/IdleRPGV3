using UnityEngine;
using UnityEngine.AI;

public class DragonAnimMove : MonoBehaviour
{
    private NavMeshAgent agent;
    public Animator animator;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);
    }
}
