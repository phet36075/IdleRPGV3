using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

    public class MySetTrigger : Action
    {
      
        public SharedGameObject targetGameObject;
      
        public SharedString paramaterName;

        private Animator animator;
        private GameObject prevGameObject;
        private DragonAnimMove animController;
        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject) {
                animator = currentGameObject.GetComponent<Animator>();
                prevGameObject = currentGameObject;
            }

            animController = GetComponent<DragonAnimMove>();
            animator.SetTrigger(paramaterName.Value);
        }
        // public MySetTrigger(DragonAnimMove controller)
        // {
        //     animController = controller;
        // }
        public override TaskStatus OnUpdate()
        {
            if (animator == null) {
                Debug.LogWarning("Animator is null");
                return TaskStatus.Failure;
            }

            if (animController.isAnimationCompleted)
            {
                animController.isAnimationCompleted = false;
                return TaskStatus.Success;
            }
          

            return TaskStatus.Running;

        }

        public override void OnReset()
        {
            targetGameObject = null;
            paramaterName = "";
        }
    }
