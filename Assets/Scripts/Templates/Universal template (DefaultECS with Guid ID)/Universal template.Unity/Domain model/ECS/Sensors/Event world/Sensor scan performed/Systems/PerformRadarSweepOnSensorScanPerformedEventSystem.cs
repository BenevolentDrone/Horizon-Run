using System;
//using System.Collections.Generic;
//using HereticalSolutions.Repositories;

//using HereticalSolutions.SpacePartitioning;

using HereticalSolutions.Entities;

//using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

//using UnityEngine;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Templates.Universal.Unity
{
	public class PerformRadarSweepOnSensorScanPerformedEventSystem : AEntitySetSystem<float>
	{
		/*
		private readonly IRepository<string, I2DPartitionedSpace<Entity>> spaceRepository;
		
		private readonly DefaultECSEntityListManager entityListManager;

		private readonly ILogger logger;

		private List<Entity> spaceScanResult;
		*/
		
		public PerformRadarSweepOnSensorScanPerformedEventSystem(
			World world,
			//IRepository<string, I2DPartitionedSpace<Entity>> spaceRepository,
			//DefaultECSEntityListManager entityListManager,
			ILogger logger = null)
			: base(
				world
					.GetEntities()
					.With<SensorScanPerformedEventComponent>()
					.With<EventSourceWorldLocalEntityComponent<Entity>>()
					.Without<EventProcessedComponent>()
					.AsSet())
		{
			/*
			this.spaceRepository = spaceRepository;

			this.entityListManager = entityListManager;

			this.logger = logger;

			spaceScanResult = new List<Entity>();
			*/

			//TODO
			throw new NotImplementedException();
		}

		protected override void Update(
			float deltaTime,
			in Entity entity)
		{
			//TODO
			throw new NotImplementedException();

			/*
			var eventSourceEntityComponent = entity.Get<EventSourceWorldLocalEntityComponent<Entity>>();

			var sensorEntity = eventSourceEntityComponent.Source;

			if (!sensorEntity.Has<RadarSensorComponent>())
			{
				return;
			}
			
			
			//Get sensor position
			if (!EntityHelpers.TryGetPosition(
				sensorEntity,
				out Vector3 sensorPosition))
			{
				return;
			}

			//Get sensor range
			float radius = sensorEntity.Get<CircleShapeComponent>().Radius;
			
			//Get scan results
			var sensorComponent = sensorEntity.Get<SensorComponent>();
			
			var scanResults =  entityListManager.GetList(
				sensorComponent.EntityListHandle);

			if (scanResults == null)
			{
				return;
			}
			
			//Scan all relevant spaces
			var radarSensorComponent = sensorEntity.Get<RadarSensorComponent>();
			
			foreach (var spaceID in radarSensorComponent.TargetSpaceIDs)
			{
				if (string.IsNullOrEmpty(spaceID))
				{
					logger?.LogError<PerformRadarSweepOnSensorScanPerformedEventSystem>(
						$"SENSOR {sensorEntity} HAS EMPTY SPACE ID");

					continue;
				}
				
				//logger?.Log(
				//	$"SENSOR {sensorEntity} LOOKING FOR TARGETS IN SPACE {spaceID}");

				if (!spaceRepository.TryGet(
					spaceID,
					out var space))
				{
					logger?.LogError<PerformRadarSweepOnSensorScanPerformedEventSystem>(
						$"SPACE {spaceID} NOT FOUND IN SPACE REPOSITORY");

					continue;
				}
				
				spaceScanResult.Clear();

				space.AllInRangeNarrow(
					new Vector2(
						sensorPosition.x,
						sensorPosition.z),
					radius,
					spaceScanResult);
				
				scanResults.AddRange(spaceScanResult);
			}
			
			//logger?.Log(
			//	$"RADAR REPORTS {scanResults.Count} ENTITIES");
			
			UnityDebugHelpers.DrawCircle(
				sensorPosition,
				radius,
				16,
				(scanResults.Count == 0)
					? Color.red
					: Color.green);
			*/
		}
	}
}