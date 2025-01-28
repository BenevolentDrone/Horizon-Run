using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.Modules.Core_DefaultECS;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleSceneEntity :
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