using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
    [Component("Simulation world/Rotation")]
    public struct QuaternionSteeringComponent
    {
        public Quaternion TargetQuaternion;

        public float AngularSpeed;
    }
}