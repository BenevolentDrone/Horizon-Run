using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity
{
    [Component("Simulation world/Rotation")]
    [ServerAuthoredOnInitializationComponent]
    [ServerAuthoredComponent]
    public struct QuaternionComponent
    {
        public Quaternion Quaternion;
    }
}