using System;

using HereticalSolutions.Entities;

using HereticalSolutions.Networking.ECS;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
	/*
	public class UpdateAuthoringPermissionOnEntityCreatedEventSystem
		: AEntitySetSystem<float>
	{
		private readonly UniversalTemplateEntityManager entityManager;
		
		public UpdateAuthoringPermissionOnEntityCreatedEventSystem(
			World world,
			UniversalTemplateEntityManager entityManager)
			: base(
				world
					.GetEntities()
					.With<EntitySpawnedEventComponent>()
					.With<EventSourceEntityComponent<Guid>>()
					.Without<EventProcessedComponent>()
					.AsSet())
		{
			this.entityManager = entityManager;
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			if (!EventEntityHelpers.TryGetEventSourceEntityID<Guid>(
			    entity,
			    out var sourceEntityID,
			    false))
			{
				return;
			}

			if (!EntityHelpers.TryGetSimulationEntity(
			    sourceEntityID,
			    entityManager,
			    out var sourceEntity))
			{
				return;
			}


			var entitySpawnedEventComponent = entity.Get<EntitySpawnedEventComponent>();

			var registryEntity = entityManager.GetRegistryEntity(
				entitySpawnedEventComponent.GUID);
			
			if (!registryEntity.IsAlive)
				return;
			
			if (!sourceEntity.Has<ProvideAuthoringPermissionComponent>())
				return;

			var provideAuthoringPermissionToSpawnedEntityComponent = sourceEntity.Get<ProvideAuthoringPermissionComponent>();
			
			registryEntity.Set<AuthoringPermissionComponent>(
				new AuthoringPermissionComponent
				{
					PlayerSlot = (byte)provideAuthoringPermissionToSpawnedEntityComponent.PlayerSlot
				});
		}
	}
	*/
}