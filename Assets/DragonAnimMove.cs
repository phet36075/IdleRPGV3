using UnityEngine;
using UnityEngine.AI;

public class DragonAnimMove : MonoBehaviour
{
    private NavMeshAgent agent;
    public Animator animator;
    public bool isAnimationCompleted = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    public void OnAnimationComplete()
    {
        isAnimationCompleted = true;
        Debug.Log("Animation Completed!");
    }
    // Update is called once per frame
    void Update()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);
    }
}
