using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
    [Component("Simulation world/Rotation")]
    [ServerAuthoredOnInitializationComponent]
    [ServerAuthoredComponent]
    public struct QuaternionComponent
    {
        public Quaternion Quaternion;
    }
}