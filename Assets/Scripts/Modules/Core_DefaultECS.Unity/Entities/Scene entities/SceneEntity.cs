using System;

using HereticalSolutions.Allocations.Factories;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
    public class SceneEntity :
        ASceneEntityWithID<Guid>
    {
        public override Guid EntityID { get => Guid.Parse(entityID); }

#if UNITY_EDITOR
        protected override Guid AllocateID()
        {
            return IDAllocationsFactory.BuildGUID();
        }
#endif
    }
}