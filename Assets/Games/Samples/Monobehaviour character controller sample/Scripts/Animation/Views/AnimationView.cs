using HereticalSolutions.Samples.MonobehaviourCharacterControllerSample.Models;

using UnityEngine;

namespace HereticalSolutions.Samples.MonobehaviourCharacterControllerSample.Views
{
    public class AnimationView : MonoBehaviour
    {
        [SerializeField]
        private Animator animator;

        [SerializeField]
        private string idleParameter;

        [SerializeField]
        private string runningParameter;
        
        [SerializeField]
        private string talkingParameter;
        
        [SerializeField]
        private string idleAnimationSelectionParameter;
        
        [SerializeField]
        private int idleAnimationsCount;

        [SerializeField]
        private LocomotionBehaviour locomotionBehaviour;

        private bool wasIdleLastUpdate = true;
        
        void Update()
        {
            if (animator == null)
            {
                Debug.LogError("[AnimationView] ANIMATOR IS NULL", this);
                
                return;
            }

            if (locomotionBehaviour == null)
            {
                Debug.LogError("[AnimationView] LOCOMOTION BEHAVIOUR IS NULL", this);
                
                return;
            }

            bool running = locomotionBehaviour.LocomotionVectorNormalized.magnitude > MathHelpers.EPSILON;

            bool talking = false;

            bool idle = 
                !running
                && !talking;
            
            animator.SetBool(idleParameter, idle);
            
            animator.SetBool(runningParameter, running);
            
            animator.SetBool(talkingParameter, talking);

            if (idle && !wasIdleLastUpdate)
            {
                animator.SetInteger(
                    idleAnimationSelectionParameter,
                    UnityEngine.Random.Range(0, idleAnimationsCount));
            }

            wasIdleLastUpdate = idle;
        }
    }
}