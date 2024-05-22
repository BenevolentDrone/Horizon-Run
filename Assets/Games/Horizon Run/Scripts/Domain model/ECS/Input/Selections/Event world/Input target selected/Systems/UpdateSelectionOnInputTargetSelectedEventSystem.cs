using HereticalSolutions.Entities;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
	public class UpdateSelectionOnInputTargetSelectedEventSystem : AEntitySetSystem<float>
	{
		private World viewWorld;

		private EntitySet selectionSet;

		public UpdateSelectionOnInputTargetSelectedEventSystem(
			World world,
			World viewWorld)
			: base(
				world
					.GetEntities()
					.With<InputTargetSelectedEventComponent>()
					.With<EventTargetWorldLocalEntityComponent<Entity>>()
					.Without<EventProcessedComponent>()
					.AsSet())
		{
			this.viewWorld = viewWorld;

			selectionSet = viewWorld
				.GetEntities()
				.With<InputTargetSelectionComponent>()
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

				ref var inputTargetSelectionComponent = ref selectionEntity.Get<InputTargetSelectionComponent>();

				inputTargetSelectionComponent.TargetEntity = eventTargetEntityComponent.Target;

				return;
			}


			var newSelectionEntity = viewWorld.CreateEntity();

			newSelectionEntity.Set<InputTargetSelectionComponent>(
				new InputTargetSelectionComponent
				{
					TargetEntity = eventTargetEntityComponent.Target
				});
		}
	}
}