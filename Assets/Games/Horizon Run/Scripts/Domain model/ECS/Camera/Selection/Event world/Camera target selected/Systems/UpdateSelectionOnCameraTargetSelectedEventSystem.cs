using HereticalSolutions.Entities;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
	public class UpdateSelectionOnCameraTargetSelectedEventSystem : AEntitySetSystem<float>
	{
		private World viewWorld;

		private EntitySet selectionSet;

		public UpdateSelectionOnCameraTargetSelectedEventSystem(
			World world,
			World viewWorld)
			: base(
				world
					.GetEntities()
					.With<CameraTargetSelectedEventComponent>()
					.With<EventTargetWorldLocalEntityComponent<Entity>>()
					.Without<EventProcessedComponent>()
					.AsSet())
		{
			this.viewWorld = viewWorld;

			selectionSet = viewWorld
				.GetEntities()
				.With<CameraTargetSelectionComponent>()
				.AsSet();
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			var eventTargetEntityComponent = entity.Get<EventTargetWorldLocalEntityComponent<Entity>>();


			foreach (var selectionEntity in selectionSet.GetEntities())
			{
				if (!selectionEntity.IsAlive)
				{
					continue;
				}

				ref var cameraTargetSelectionComponent = ref selectionEntity.Get<CameraTargetSelectionComponent>();

				cameraTargetSelectionComponent.TargetEntity = eventTargetEntityComponent.Target;

				return;
			}


			var newSelectionEntity = viewWorld.CreateEntity();

			newSelectionEntity.Set<CameraTargetSelectionComponent>(
				new CameraTargetSelectionComponent
				{
					TargetEntity = eventTargetEntityComponent.Target
				});
		}
	}
}