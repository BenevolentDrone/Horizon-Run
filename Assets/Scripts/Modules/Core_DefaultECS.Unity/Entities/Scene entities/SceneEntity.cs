using System;

using HereticalSolutions.Allocations;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
    public class SceneEntity :
        ASceneEntityWithID<Guid>
    {
        public override Guid EntityID { get => Guid.Parse(entityID); }

#if UNITY_EDITOR
        protected override Guid AllocateID()
        {
            //return IDAllocationFactory.BuildGUID();
            return GUIDAllocationController.AllocateGUIDStatic();
        }
#endif
    }
}