using HereticalSolutions.Entities;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
	public class UpdateSelectionOnHUDTargetSelectedEventSystem : AEntitySetSystem<float>
	{
		private World viewWorld;

		private EntitySet selectionSet;

		public UpdateSelectionOnHUDTargetSelectedEventSystem(
			World world,
			World viewWorld)
			: base(
				world
					.GetEntities()
					.With<HUDTargetSelectedEventComponent>()
					.With<EventTargetWorldLocalEntityComponent<Entity>>()
					.Without<EventProcessedComponent>()
					.AsSet())
		{
			this.viewWorld = viewWorld;

			selectionSet = viewWorld
				.GetEntities()
				.With<HUDTargetSelectionComponent>()
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

				ref var hudTargetSelectionComponent = ref selectionEntity.Get<HUDTargetSelectionComponent>();

				hudTargetSelectionComponent.TargetEntity = eventTargetEntityComponent.Target;

				return;
			}


			var newSelectionEntity = viewWorld.CreateEntity();

			newSelectionEntity.Set<HUDTargetSelectionComponent>(
				new HUDTargetSelectionComponent
				{
					TargetEntity = eventTargetEntityComponent.Target
				});
		}
	}
}