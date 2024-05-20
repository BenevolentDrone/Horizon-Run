using HereticalSolutions.Entities;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleVirtualCameraPresenterInitializationSystem : AEntitySetSystem<float>
	{
		private SampleEntityManager sampleEntityManager;

		private EntitySet playerSet;

		public SampleVirtualCameraPresenterInitializationSystem(
			World world,
			World simulationWorld,
			SampleEntityManager sampleEntityManager)
			: base(
				world
					.GetEntities()
					.With<SampleVirtualCameraPresenterComponent>()
					.AsSet())
		{
			this.sampleEntityManager = sampleEntityManager;

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

				var playerViewEntity = sampleEntityManager.GetEntity(
					playerEntity.Get<GUIDComponent>().GUID,
					WorldConstants.VIEW_WORLD_ID);

				sampleVirtualCameraPresenterComponent.TargetEntity = playerViewEntity;

				return;
			}
		}
	}
}