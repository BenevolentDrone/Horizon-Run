using HereticalSolutions.Entities;

using DefaultEcs;

using UnityEngine;

namespace HereticalSolutions.HorizonRun
{
    [Component("Simulation world/Position")]
    public struct NestedPositionComponent
    {
        public Entity PositionSourceEntity;
        
        public Vector3 LocalPosition;
    }
}