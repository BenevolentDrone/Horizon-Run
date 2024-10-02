using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	public class VirtualCameraPresenterModelValidationSystem : AEntitySetSystem<float>
	{
		private EntitySet selectionSet;

		public VirtualCameraPresenterModelValidationSystem(
			World world)
			: base(
				world
					.GetEntities()
					.With<VirtualCameraPresenterComponent>()
					.AsSet())
		{
			selectionSet = world
				.GetEntities()
				.With<CameraTargetSelectionComponent>()
				.AsSet();
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			ref var virtualCameraPresenterComponent = ref entity.Get<VirtualCameraPresenterComponent>();

			//Update this all the time. Otherwise if selection changes and the previous selection target is still alive then we wont change the presenter's target at all
			/*
			var targetEntity = virtualCameraPresenterComponent.TargetEntity;

			if (targetEntity.IsAlive)
			{
				return;
			}
			*/

			foreach (var selectionEntity in selectionSet.GetEntities())
			{
				if (!selectionEntity.IsAlive)
				{
					continue;
				}

				virtualCameraPresenterComponent.TargetEntity = selectionEntity.Get<CameraTargetSelectionComponent>().TargetEntity;

				return;
			}
		}
	}
}