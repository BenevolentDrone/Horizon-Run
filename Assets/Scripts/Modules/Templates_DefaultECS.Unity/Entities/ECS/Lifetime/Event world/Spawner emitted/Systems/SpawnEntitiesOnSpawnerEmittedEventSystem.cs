using System;

using HereticalSolutions.Entities;

using DefaultEcs;
using DefaultEcs.System;
using HereticalSolutions.Allocations;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	public class SpawnEntitiesOnSpawnerEmittedEventSystem : AEntitySetSystem<float>
	{
		private readonly IIDAllocationController<Guid> iDAllocationController;

		private readonly IEventEntityBuilder<Entity, Guid> eventEntityBuilder;

		public SpawnEntitiesOnSpawnerEmittedEventSystem(
			World world,
			IIDAllocationController<Guid> iDAllocationController,
			IEventEntityBuilder<Entity, Guid> eventEntityBuilder)
			: base(
				world
					.GetEntities()
					.With<SpawnerEmittedEventComponent>()
					.With<EventReceiverWorldLocalEntityComponent<Entity>>()
					.Without<EventProcessedComponent>()
					.AsSet())
		{
			this.iDAllocationController = iDAllocationController;

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

			if (!receiverEntity.Has<SpawnEntitiesComponent>())
			{
				return;
			}

			
			var spawnEntitiesComponent = receiverEntity.Get<SpawnEntitiesComponent>();

			if (string.IsNullOrEmpty(spawnEntitiesComponent.PrototypeID))
			{
				return;
			}

			for (int i = 0; i < spawnEntitiesComponent.Amount; i++)
			{
				iDAllocationController.AllocateID(
					out var newID);

				eventEntityBuilder
					.NewEvent(out var eventEntity)
					.CausedByWorldLocalEntity(
						eventEntity,
						receiverEntity)
					.WithData<EntitySpawnedEventComponent>(
						eventEntity,
						new EntitySpawnedEventComponent
						{
							GUID = newID,

							PrototypeID = spawnEntitiesComponent.PrototypeID,

							Override = default
						});
			}
		}
	}
}