using System;
using DefaultEcs;
using HereticalSolutions.Entities;

namespace HereticalSolutions.Templates.Universal.Unity
{
	public class EmitImmediatelyInitializationSystem
		: IDefaultECSEntityInitializationSystem
	{
		private readonly IEventEntityBuilder<Entity, Guid> eventEntityBuilder;

		public EmitImmediatelyInitializationSystem(
			IEventEntityBuilder<Entity, Guid> eventEntityBuilder)
		{
			this.eventEntityBuilder = eventEntityBuilder;
		}

		//Required by ISystem
		public bool IsEnabled { get; set; } = true;

		public void Update(Entity entity)
		{
			if (!IsEnabled)
				return;

			if (!entity.Has<SpawnerComponent>())
				return;

			if (!entity.Has<EmitImmediatelyComponent>())
				return;

			eventEntityBuilder
				.NewEvent(out var eventEntity)
				.AddressedToWorldLocalEntity(
					eventEntity,
					entity)
				.WithData<SpawnerEmittedEventComponent>(
					eventEntity,
					new SpawnerEmittedEventComponent());

			entity.Remove<EmitImmediatelyComponent>();
		}

		public void Dispose()
		{
		}
	}
}
