using HereticalSolutions.Entities;

using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	/*
	public class PlaceEntity2DOnEntitySpawnedEventSystem : AEntitySetSystem<float>
	{
		private readonly UniversalTemplateEntityManager entityManager;

		private readonly DefaultECSEntityHierarchyManager entityHierarchyManager;

		private readonly ILogger logger;

		public PlaceEntity2DOnEntitySpawnedEventSystem(
			World world,
			UniversalTemplateEntityManager entityManager,
			DefaultECSEntityHierarchyManager entityHierarchyManager,
			ILogger logger = null)
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


			if (!sourceEntity.Has<Position2DComponent>())
				return;

			if (!targetEntity.Has<Position2DComponent>())
				return;


			ref var targetEntityPosition = ref targetEntity.Get<Position2DComponent>();


			Vector2 targetPosition = Vector2.zero;

			if (sourceEntity.Has<Transform2DComponent>())
			{
				var sourceTransformComponent = sourceEntity.Get<Transform2DComponent>();

				targetPosition = TransformHelpers.GetWorldPosition2D(
					sourceEntity,
					entityHierarchyManager,
					logger);
			}
			else
			{
				targetPosition = sourceEntity.Get<Position2DComponent>().Position;
			}

			targetEntityPosition.Position = targetPosition;

			if (targetEntity.Has<Transform2DComponent>())
			{
				ref Transform2DComponent transformComponent = ref targetEntity.Get<Transform2DComponent>();

				transformComponent.Dirty = true;
			}
		}
	}
	*/
}