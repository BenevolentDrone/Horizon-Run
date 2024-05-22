using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
    [Component("Simulation world/Locomotion")]
    public struct Locomotion3DComponent
    {
        public float LocomotionSpeedNormal;

        public float MaxLocomotionSpeed;

        public Vector3 LocomotionVectorNormalized;
    }
}