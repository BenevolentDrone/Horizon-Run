using HereticalSolutions.Samples.MonobehaviourCharacterControllerSample.Models;

using UnityEngine;

namespace HereticalSolutions.Samples.MonobehaviourCharacterControllerSample.Controllers
{
    public class JoystickRotationController : MonoBehaviour
    {
        [SerializeField]
        private RotationBehaviour rotationBehaviour;
        
        [SerializeField]
        private Joystick joystick;

        public Joystick Joystick
        {
            get => joystick;
            set => joystick = value;
        }
        
        private void FixedUpdate()
        {
            if (rotationBehaviour == null)
            {
                Debug.LogError("[JoystickRotationController] ROTATION BEHAVIOUR IS NULL", this);
                
                return;
            }
            
            if (joystick == null)
            {
                Debug.LogError("[JoystickRotationController] JOYSTICK IS NULL", this);
                
                return;
            }

            if (joystick.Sleeping)
            {
                return;
            }

            if (joystick.Direction.magnitude < MathHelpers.EPSILON)
            {
                return;
            }

            rotationBehaviour.LastLocomotionVectorNormalized = joystick.Direction;
        }
    }
}