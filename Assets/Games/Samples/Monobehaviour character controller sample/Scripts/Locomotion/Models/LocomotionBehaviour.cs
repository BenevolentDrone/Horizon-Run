using UnityEngine;

namespace HereticalSolutions.Samples.MonobehaviourCharacterControllerSample.Models
{
    public class LocomotionBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Transform locomotionTransform;

        [SerializeField]
        private float locomotionSpeed;

        public float LocomotionSpeed
        {
            get => locomotionSpeed;
        }

        public Vector2 LocomotionVectorNormalized { get; set; }

        void FixedUpdate()
        {
            if (locomotionTransform == null)
            {
                Debug.LogError("[LocomotionBehaviour] LOCOMOTION TRANSFORM IS NULL", this);
                
                return;
            }

            Vector2 locomotionVector = LocomotionVectorNormalized * (locomotionSpeed * UnityEngine.Time.fixedDeltaTime);

            locomotionTransform.position += MathHelpersUnity.Vector2XZTo3(locomotionVector);
        }
    }
}