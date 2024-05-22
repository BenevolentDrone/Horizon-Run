using HereticalSolutions.Entities;

using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
	public class PlaceEntityOnEntitySpawnedEventSystem : AEntitySetSystem<float>
	{
		private readonly HorizonRunEntityManager entityManager;

		public PlaceEntityOnEntitySpawnedEventSystem(
			World world,
			HorizonRunEntityManager entityManager)
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


			if (!sourceEntity.Has<PositionComponent>()
			    && !sourceEntity.Has<NestedPositionComponent>())
				return;

			if (!targetEntity.Has<PositionComponent>())
				return;
				

			ref var targetEntityPosition = ref targetEntity.Get<PositionComponent>();


			Vector3 targetPosition = Vector3.zero;

			if (sourceEntity.Has<PositionComponent>())
			{
				targetPosition = sourceEntity.Get<PositionComponent>().Position;
			}
			
			if (sourceEntity.Has<NestedPositionComponent>())
			{
				var nestedPositionComponent = sourceEntity.Get<NestedPositionComponent>();

				var positionSource = nestedPositionComponent.PositionSourceEntity;

				var localPosition = nestedPositionComponent.LocalPosition;

				targetPosition = positionSource.Get<PositionComponent>().Position;
				
				if (positionSource.Has<UniformRotationComponent>())
				{
					var angle = positionSource.Get<UniformRotationComponent>().Angle;

					var rotation = Quaternion.Euler(0, angle, 0) 
					               * Quaternion.LookRotation(localPosition);
					
					targetPosition +=  rotation 
					                   * Vector3.forward 
					                   * localPosition.magnitude;;
				}
				else
				{
					targetPosition += localPosition;
				}
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
		}
	}
}