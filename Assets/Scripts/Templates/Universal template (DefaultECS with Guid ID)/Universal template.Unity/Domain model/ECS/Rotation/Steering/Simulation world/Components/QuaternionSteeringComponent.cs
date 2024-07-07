using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity
{
    [Component("Simulation world/Rotation")]
    public struct QuaternionSteeringComponent
    {
        public Quaternion TargetQuaternion;

        public float AngularSpeed;
    }
}