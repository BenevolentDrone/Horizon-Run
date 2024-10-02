using System;

using HereticalSolutions.Entities;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	public class InvokeSubSpawnerOnSpawnerEmittedEventSystem : AEntitySetSystem<float>
	{
		private readonly IEventEntityBuilder<Entity, Guid> eventEntityBuilder;

		public InvokeSubSpawnerOnSpawnerEmittedEventSystem(
			World world,
			IEventEntityBuilder<Entity, Guid> eventEntityBuilder)
			: base(
				world
					.GetEntities()
					.With<SpawnerEmittedEventComponent>()
					.With<EventReceiverWorldLocalEntityComponent<Entity>>()
					.Without<EventProcessedComponent>()
					.AsSet())
		{
			this.eventEntityBuilder = eventEntityBuilder;
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			var eventReceiverEntityComponent = entity.Get<EventReceiverWorldLocalEntityComponent<Entity>>();

			var receiverEntity = eventReceiverEntityComponent.Receiver;


			if (!receiverEntity.Has<SpawnerComponent>())
			{
				return;
			}

			if (!receiverEntity.Has<InvokeSubSpawnersComponent>())
			{
				return;
			}

			var invokeSubSpawnersComponent = receiverEntity.Get<InvokeSubSpawnersComponent>();

			foreach (var subSpawner in invokeSubSpawnersComponent.SubSpawners)
			{
				eventEntityBuilder
					.NewEvent(out var eventEntity)
					.AddressedToWorldLocalEntity(
						eventEntity,
						subSpawner)
					.WithData<SpawnerEmittedEventComponent>(
						eventEntity,
						new SpawnerEmittedEventComponent());
			}
		}
	}
}