using HereticalSolutions.Entities;

using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	/*
	public class PlaceEntity3DOnEntitySpawnedEventSystem : AEntitySetSystem<float>
	{
		private readonly UniversalTemplateEntityManager entityManager;

		private readonly DefaultECSEntityHierarchyManager entityHierarchyManager;

		private readonly ILogger logger;

		public PlaceEntity3DOnEntitySpawnedEventSystem(
			World world,
			UniversalTemplateEntityManager entityManager,
			DefaultECSEntityHierarchyManager entityHierarchyManager,
			ILogger logger)
			: base(
				world
					.GetEntities()
					.With<EntitySpawnedEventComponent>()
					.With<EventSourceWorldLocalEntityComponent<Entity>>()
					.Without<EventProcessedComponent>()
					.AsSet())
		{
			this.entityManager = entityManager;

			this.entityHierarchyManager = entityHierarchyManager;

			this.logger = logger;
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			var sourceEntityComponent = entity.Get<EventSourceWorldLocalEntityComponent<Entity>>();

			var sourceEntity = sourceEntityComponent.Source;


			var entitySpawnedEventComponent = entity.Get<EntitySpawnedEventComponent>();

			var targetEntity = entityManager.GetEntity(
				entitySpawnedEventComponent.GUID,
				WorldConstants.SIMULATION_WORLD_ID);

			if (!sourceEntity.Has<PlaceSpawnedEntityComponent>())
				return;


			if (!sourceEntity.Has<Position3DComponent>())
				return;

			if (!targetEntity.Has<Position3DComponent>())
				return;
				

			ref var targetEntityPosition = ref targetEntity.Get<Position3DComponent>();


			Vector3 targetPosition = Vector3.zero;

			if (sourceEntity.Has<Transform3DComponent>())
			{
				var sourceTransformComponent = sourceEntity.Get<Transform3DComponent>();

				targetPosition = TransformHelpers.GetWorldPosition3D(
					sourceEntity,
					entityHierarchyManager,
					logger);
			}
			else
			{
				targetPosition = sourceEntity.Get<Position3DComponent>().Position;
			}

			targetEntityPosition.Position = targetPosition;

			if (targetEntity.Has<Transform3DComponent>())
			{
				ref Transform3DComponent transformComponent = ref targetEntity.Get<Transform3DComponent>();

				transformComponent.Dirty = true;
			}
		}
	}
	*/
}