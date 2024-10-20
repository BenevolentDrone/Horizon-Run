using HereticalSolutions.Entities;

using HereticalSolutions.Modules.Core_DefaultECS;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleVirtualCameraPresenterInitializationSystem : AEntitySetSystem<float>
	{
		private EntityManager entityManager;

		private EntitySet playerSet;

		public SampleVirtualCameraPresenterInitializationSystem(
			World world,
			World simulationWorld,
			EntityManager entityManager)
			: base(
				world
					.GetEntities()
					.With<SampleVirtualCameraPresenterComponent>()
					.AsSet())
		{
			this.entityManager = entityManager;

			playerSet = simulationWorld
				.GetEntities()
				.With<SamplePlayerComponent>()
				.AsSet();
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var sampleVirtualCameraPresenterComponent = ref entity.Get<SampleVirtualCameraPresenterComponent>();

			var targetEntity = sampleVirtualCameraPresenterComponent.TargetEntity;

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

				if (!entityManager.TryGetEntity(
					playerEntity.Get<GUIDComponent>().GUID,
					WorldConstants.VIEW_WORLD_ID,
					out var playerViewEntity))
				{
					continue;
				}

				sampleVirtualCameraPresenterComponent.TargetEntity = playerViewEntity;

				return;
			}
		}
	}
}