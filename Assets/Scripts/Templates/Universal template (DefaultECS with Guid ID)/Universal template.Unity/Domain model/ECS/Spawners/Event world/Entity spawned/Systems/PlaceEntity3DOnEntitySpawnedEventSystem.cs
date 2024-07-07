using HereticalSolutions.Entities;

using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Templates.Universal.Unity
{
	public class PlaceEntity3DOnEntitySpawnedEventSystem : AEntitySetSystem<float>
	{
		private readonly UniversalTemplateEntityManager entityManager;

		public PlaceEntity3DOnEntitySpawnedEventSystem(
			World world,
			UniversalTemplateEntityManager entityManager)
			: base(
				world
					.GetEntities()
					.With<EntitySpawnedEventComponent>()
					.With<EventSourceWorldLocalEntityComponent<Entity>>()
					.Without<EventProcessedComponent>()
					.AsSet())
		{
			this.entityManager = entityManager;
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

				targetPosition = TransformHelpers.GetWorldPosition3D(sourceEntity);
			}
			else
			{
				targetPosition = sourceEntity.Get<Position3DComponent>().Position;
			}

			//TODO
			/*
			if (sourceEntity.Has<CircleShapeComponent>())
			{
				var circleShapeComponent = sourceEntity.Get<CircleShapeComponent>();

				//Courtesy of https://stackoverflow.com/questions/5837572/generate-a-random-point-within-a-circle-uniformly/50746409#50746409
				float distanceFromCenter = circleShapeComponent.Radius
					* Mathf.Sqrt( //sqrt to get a uniform distribution, check the link above for more info
						UnityEngine.Random.Range(0.0f, 1.0f));
						
				float theta = UnityEngine.Random.Range(0.0f, 1.0f) * 2f * Mathf.PI;

				float distanceX = distanceFromCenter * Mathf.Cos(theta);
				float distanceY = distanceFromCenter * Mathf.Sin(theta);

				targetPosition += new Vector3(
					distanceX,
					0f,
					distanceY);
			}
			*/


			targetEntityPosition.Position = targetPosition;

			if (targetEntity.Has<Transform3DComponent>())
			{
				ref Transform3DComponent transformComponent = ref targetEntity.Get<Transform3DComponent>();

				transformComponent.Dirty = true;
			}
		}
	}
}