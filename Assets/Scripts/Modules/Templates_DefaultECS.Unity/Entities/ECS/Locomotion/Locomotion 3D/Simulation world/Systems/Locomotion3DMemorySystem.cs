using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	public class Locomotion3DMemorySystem : AEntitySetSystem<float>
	{
		public Locomotion3DMemorySystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<Locomotion3DComponent>()
					.With<Locomotion3DMemoryComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			var locomotion3DComponent = entity.Get<Locomotion3DComponent>();

			ref var locomotionMemoryComponent = ref entity.Get<Locomotion3DMemoryComponent>();


			if (locomotion3DComponent.LocomotionSpeedNormal > MathHelpers.EPSILON)
			{
				locomotionMemoryComponent.LastLocomotionVectorNormalized = locomotion3DComponent.LocomotionVectorNormalized;
			}
		}
	}
}