using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
    [Component("Simulation world/Rotation")]
    public struct QuaternionSteeringComponent
    {
        public Quaternion TargetQuaternion;

        public float AngularSpeed;
    }
}