using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Entities;
using HereticalSolutions.Entities.Factories;

using HereticalSolutions.Logging;

using DefaultEcs;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample.Factories
{
	public static class SampleEntityFactory
	{
		public static SampleEntityManager BuildSampleEntityManager(
			ILoggerResolver loggerResolver = null)
		{
			Func<Guid> allocateIDDelegate = () =>
			{
				return IDAllocationsFactory.BuildGUID();
			};

			Func<GUIDComponent, Guid> getEntityIDFromIDComponentDelegate = (GUIDComponent) =>
			{
				return GUIDComponent.GUID;
			};

			Func<Guid, GUIDComponent> createIDComponentDelegate = (guid) =>
			{
				return new GUIDComponent
				{
					GUID = guid
				};
			};


			var registryEntityRepository = RepositoriesFactory.BuildDictionaryRepository<Guid, Entity>();

			var entityWorldsRepository = DefaultECSEntityFactory.BuildDefaultECSEntityWorldsRepository(loggerResolver);


			entityWorldsRepository.AddWorld(
				WorldConstants.REGISTRY_WORLD_ID,
				DefaultECSEntityFactory.BuildDefaultECSRegistryWorldController(
					createIDComponentDelegate,
					DefaultECSEntityFactory.BuildDefaultECSPrototypesRepository(),
					loggerResolver));

			entityWorldsRepository.AddWorld(
				WorldConstants.EVENT_WORLD_ID,
				DefaultECSEntityFactory.BuildDefaultECSEventWorldController(
					loggerResolver));

			entityWorldsRepository.AddWorld(
				WorldConstants.SIMULATION_WORLD_ID,
				DefaultECSEntityFactory.BuildDefaultECSWorldController
					<Guid,
					GUIDComponent,
					SimulationEntityComponent,
					ResolveSimulationComponent>(
						getEntityIDFromIDComponentDelegate,
						createIDComponentDelegate,

						(component) => { return component.SimulationEntity; },
						(component) => { return component.PrototypeID; },
						(prototypeID, entity) =>
						{
							return new SimulationEntityComponent
							{
								PrototypeID = prototypeID,

								SimulationEntity = entity
							};
						},

						(source) => { return new ResolveSimulationComponent { Source = source }; },

						loggerResolver));

			entityWorldsRepository.AddWorld(
				WorldConstants.VIEW_WORLD_ID,
				DefaultECSEntityFactory.BuildDefaultECSWorldController
					<Guid,
					GUIDComponent,
					ViewEntityComponent,
					ResolveViewComponent>(
						getEntityIDFromIDComponentDelegate,
						createIDComponentDelegate,

						(component) => { return component.ViewEntity; },
						(component) => { return component.PrototypeID; },
						(prototypeID, entity) =>
						{
							return new ViewEntityComponent
							{
								PrototypeID = prototypeID,

								ViewEntity = entity
							};
						},

						(source) => { return new ResolveViewComponent { Source = source }; },

						loggerResolver));

			List<World> childEntityWorlds = new List<World>();

			childEntityWorlds.Add(entityWorldsRepository.GetWorld(WorldConstants.SIMULATION_WORLD_ID));
			childEntityWorlds.Add(entityWorldsRepository.GetWorld(WorldConstants.VIEW_WORLD_ID));

			ILogger logger =
				loggerResolver?.GetLogger<DefaultECSEntityManager<Guid>>()
				?? null;

			return new SampleEntityManager(
				allocateIDDelegate,
				registryEntityRepository,
				entityWorldsRepository,
				childEntityWorlds,
				logger);
		}
	}
}