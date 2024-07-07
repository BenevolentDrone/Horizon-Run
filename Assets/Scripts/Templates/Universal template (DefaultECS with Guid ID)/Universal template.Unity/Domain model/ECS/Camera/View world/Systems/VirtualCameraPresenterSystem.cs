using DefaultEcs;
using DefaultEcs.System;
using UnityEngine;
using HereticalSolutions.Entities;

namespace HereticalSolutions.Templates.Universal.Unity
{
	public class VirtualCameraPresenterSystem : AEntitySetSystem<float>
	{
		public VirtualCameraPresenterSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<VirtualCameraPresenterComponent>()
					.With<VirtualCameraViewComponent>()
					.AsSet())
		{
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			var virtualCameraPresenterComponent = entity.Get<VirtualCameraPresenterComponent>();

			ref var virtualCameraViewComponent = ref entity.Get<VirtualCameraViewComponent>();


			var targetEntity = virtualCameraPresenterComponent.TargetEntity;

			if (!targetEntity.IsAlive)
			{
				virtualCameraViewComponent.VirtualCamera.Follow = null;

				return;
			}


			var pooledViewComponent = targetEntity.Get<PooledGameObjectViewComponent>();

			var transform = pooledViewComponent.Element.Value.transform;


			if (virtualCameraViewComponent.VirtualCamera.Follow != transform)
			{
				virtualCameraViewComponent.VirtualCamera.Follow = transform;
			}
		}
	}
}