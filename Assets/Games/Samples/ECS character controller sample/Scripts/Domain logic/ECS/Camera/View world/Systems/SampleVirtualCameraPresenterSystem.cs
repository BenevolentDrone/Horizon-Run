using DefaultEcs;
using DefaultEcs.System;
using HereticalSolutions.Entities;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleVirtualCameraPresenterSystem : AEntitySetSystem<float>
	{
		public SampleVirtualCameraPresenterSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<SampleVirtualCameraPresenterComponent>()
					.With<SampleVirtualCameraViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var sampleVirtualCameraPresenterComponent = ref entity.Get<SampleVirtualCameraPresenterComponent>();

			ref var sampleVirtualCameraViewComponent = ref entity.Get<SampleVirtualCameraViewComponent>();


			var targetEntity = sampleVirtualCameraPresenterComponent.TargetEntity;

			if (!targetEntity.IsAlive)
			{
				sampleVirtualCameraViewComponent.VirtualCamera.Follow = null;

				return;
			}


			var pooledViewComponent = targetEntity.Get<PooledGameObjectViewComponent>();

			var transform = pooledViewComponent.Element.Value.transform;


			if (sampleVirtualCameraViewComponent.VirtualCamera.Follow != transform)
			{
				sampleVirtualCameraViewComponent.VirtualCamera.Follow = transform;
			}
		}
	}
}