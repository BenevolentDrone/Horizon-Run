using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleJoystickPresenterInitializationSystem : AEntitySetSystem<float>
	{
		private EntitySet playerSet;

		public SampleJoystickPresenterInitializationSystem(
			World world,
			World simulationWorld)
			: base(
				world
					.GetEntities()
					.With<SampleJoystickPresenterComponent>()
					.AsSet())
		{
			playerSet = simulationWorld
				.GetEntities()
				.With<SamplePlayerComponent>()
				.AsSet();
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var sampleJoystickPresenterComponent = ref entity.Get<SampleJoystickPresenterComponent>();

			var targetEntity = sampleJoystickPresenterComponent.TargetEntity;

			if (targetEntity.IsAlive)
			{
				return;
			}

			foreach (var playerEntity in playerSet.GetEntities())
			{
				if (!playerEntity.IsAlive)
				{
					continue;
				}

				sampleJoystickPresenterComponent.TargetEntity = playerEntity;

				return;
			}
		}
	}
}