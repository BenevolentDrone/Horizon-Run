using HereticalSolutions.Entities;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Templates.Universal.Unity
{
	public class RotateEntityOnEntitySpawnedEventSystem : AEntitySetSystem<float>
	{
		private readonly UniversalTemplateEntityManager entityManager;

		public RotateEntityOnEntitySpawnedEventSystem(
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

			if (!sourceEntity.Has<RotateSpawnedEntityComponent>())
				return;


			if (sourceEntity.Has<UniformRotationComponent>()
				&& targetEntity.Has<UniformRotationComponent>())
			{
				var sourceEntityRotation = sourceEntity.Get<UniformRotationComponent>();

				ref var targetEntityRotation = ref targetEntity.Get<UniformRotationComponent>();

				targetEntityRotation.Angle = sourceEntityRotation.Angle;
			}

			if (sourceEntity.Has<QuaternionComponent>()
				&& targetEntity.Has<QuaternionComponent>())
			{
				var sourceEntityRotation = sourceEntity.Get<QuaternionComponent>();

				ref var targetEntityRotation = ref targetEntity.Get<QuaternionComponent>();

				targetEntityRotation.Quaternion = sourceEntityRotation.Quaternion;
			}
		}
	}
}