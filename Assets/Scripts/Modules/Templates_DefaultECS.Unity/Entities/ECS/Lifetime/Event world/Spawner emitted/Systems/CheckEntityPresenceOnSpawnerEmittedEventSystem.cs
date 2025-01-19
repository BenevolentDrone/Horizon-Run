using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.SpacePartitioning;

using HereticalSolutions.Entities;

using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	public class CheckEntityPresenceOnSpawnerEmittedEventSystem : AEntitySetSystem<float>
	{
		/*
		private readonly IRepository<string, I2DPartitionedSpace<Entity>> spaceRepository;

		private readonly List<Entity> cachedEntitiesList;
		*/

		private readonly ILogger logger;

		public CheckEntityPresenceOnSpawnerEmittedEventSystem(
			World world,
			//IRepository<string, I2DPartitionedSpace<Entity>> spaceRepository,
			//List<Entity> cachedEntitiesList,
			ILogger logger)
			: base(
				world
					.GetEntities()
					.With<SpawnerEmittedEventComponent>()
					.With<EventReceiverWorldLocalEntityComponent<Entity>>()
					.Without<EventProcessedComponent>()
					.AsSet())
		{
			//TODO
			throw new NotImplementedException();

			/*
			this.spaceRepository = spaceRepository;

			this.cachedEntitiesList = cachedEntitiesList;
			
			this.logger = logger;
			*/
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			//TODO
			throw new NotImplementedException();

			/*
			var eventReceiverEntityComponent = entity.Get<EventReceiverWorldLocalEntityComponent<Entity>>();

			var receiverEntity = eventReceiverEntityComponent.Receiver;


			if (!receiverEntity.Has<SpawnerComponent>())
			{
				return;
			}

			if (!receiverEntity.Has<DoNotEmitIfEntitiesPresentComponent>())
			{
				return;
			}

			var doNotEmitIfEntitiesPresentComponent = receiverEntity.Get<DoNotEmitIfEntitiesPresentComponent>();

			var positionComponent = receiverEntity.Get<PositionComponent>();

			var circleShapeComponent = receiverEntity.Get<CircleShapeComponent>();

			foreach (var spaceID in doNotEmitIfEntitiesPresentComponent.RelevantSpaceIDs)
			{
				if (string.IsNullOrEmpty(spaceID))
				{
					logger?.LogError<CheckEntityPresenceOnSpawnerEmittedEventSystem>(
						$"ENTITY {entity.Get<GUIDComponent>().GUID} HAS EMPTY SPACE ID");

					continue;
				}

				if (!spaceRepository.TryGet(
					spaceID,
					out var space))
				{
					logger?.LogError<CheckEntityPresenceOnSpawnerEmittedEventSystem>(
						$"SPACE {spaceID} NOT FOUND IN SPACE REPOSITORY");

					continue;
				}

				cachedEntitiesList.Clear();

				space.AllInRangeNarrow(
					new Vector2(
						positionComponent.Position.x,
						positionComponent.Position.z),
					circleShapeComponent.Radius,
					cachedEntitiesList);

				if (cachedEntitiesList.Count > 0)
				{
					entity.Set(new EventProcessedComponent());

					return;
				}
			}
			*/
		}
	}
}