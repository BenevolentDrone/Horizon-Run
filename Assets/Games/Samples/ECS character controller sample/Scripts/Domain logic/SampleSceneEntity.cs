using System;

using HereticalSolutions.Modules.Core_DefaultECS;

using HereticalSolutions.Allocations.Factories;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleSceneEntity :
		ASceneEntityWithID<Guid>
	{
		public override Guid EntityID { get => Guid.Parse(entityID); }

#if UNITY_EDITOR
		protected override Guid AllocateID()
		{
			return IDAllocationFactory.BuildGUID();
		}
#endif
	}
}