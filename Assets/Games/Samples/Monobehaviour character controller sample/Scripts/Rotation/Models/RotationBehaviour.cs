using UnityEngine;

namespace HereticalSolutions.Samples.MonobehaviourCharacterControllerSample.Models
{
    public class RotationBehaviour : MonoBehaviour
    {
        [SerializeField]
        private Transform rotationTransform;

        [SerializeField]
        private float rotationSpeed;

        public float RotationSpeed
        {
            get => rotationSpeed;
        }

        public Vector2 LastLocomotionVectorNormalized { get; set; }

        private void Awake()
        {
            LastLocomotionVectorNormalized = new Vector2(
                1f,
                0f);
        }

        void FixedUpdate()
        {
            if (rotationTransform == null)
            {
                Debug.LogError("[RotationBehaviour] ROTATION TRANSFORM IS NULL", this);
                
                return;
            }

            float angle = Mathf.Atan2(
                              -LastLocomotionVectorNormalized.y,
                              LastLocomotionVectorNormalized.x)
                          * Mathf.Rad2Deg
                          + 90f;

            rotationTransform.eulerAngles = MathHelpersUnity.AxisVector3Y(angle);
        }
    }
}