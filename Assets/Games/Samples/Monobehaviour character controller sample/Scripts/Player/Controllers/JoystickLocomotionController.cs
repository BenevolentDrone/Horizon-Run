using HereticalSolutions.Samples.MonobehaviourCharacterControllerSample.Models;

using UnityEngine;

namespace HereticalSolutions.Samples.MonobehaviourCharacterControllerSample.Controllers
{
    public class JoystickLocomotionController : MonoBehaviour
    {
        [SerializeField]
        private LocomotionBehaviour locomotionBehaviour;
        
        [SerializeField]
        private Joystick joystick;

        public Joystick Joystick
        {
            get => joystick;
            set => joystick = value;
        }

        private void FixedUpdate()
        {
            if (locomotionBehaviour == null)
            {
                Debug.LogError("[JoystickLocomotionController] LOCOMOTION BEHAVIOUR IS NULL", this);
                
                return;
            }
            
            if (joystick == null)
            {
                Debug.LogError("[JoystickLocomotionController] JOYSTICK IS NULL", this);
                
                return;
            }

            if (joystick.Sleeping)
            {
                locomotionBehaviour.LocomotionVectorNormalized = Vector2.zero;
                
                return;
            }

            locomotionBehaviour.LocomotionVectorNormalized = joystick.Direction;
        }
    }
}