using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Entities;

using HereticalSolutions.Logging;

using DefaultEcs;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample.Factories
{
	public static class SampleEntityFactory
	{
		/*
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

			var entityWorldRepository = DefaultECSEntityFactory.BuildDefaultECSEntityWorldRepository(loggerResolver);


			entityWorldRepository.AddWorld(
				WorldConstants.REGISTRY_WORLD_ID,
				DefaultECSEntityFactory.BuildDefaultECSRegistryWorldController(
					createIDComponentDelegate,
					DefaultECSEntityFactory.BuildDefaultECSPrototypeRepository(),
					loggerResolver));

			entityWorldRepository.AddWorld(
				WorldConstants.EVENT_WORLD_ID,
				DefaultECSEntityFactory.BuildDefaultECSEvenTEntityWorldController(
					loggerResolver));

			entityWorldRepository.AddWorld(
				WorldConstants.SIMULATION_WORLD_ID,
				DefaultECSEntityFactory.BuildDefaultECSWorldController
					<Guid,
					GUIDComponent,
					SimulationEntityComponent,
					ResolveComponent>(
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

						(source) => { return new ResolveComponent { Source = source }; },

						loggerResolver));

			entityWorldRepository.AddWorld(
				WorldConstants.VIEW_WORLD_ID,
				DefaultECSEntityFactory.BuildDefaultECSWorldController
					<Guid,
					GUIDComponent,
					ViewEntityComponent,
					ResolveComponent>(
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

						(source) => { return new ResolveComponent { Source = source }; },

						loggerResolver));

			List<World> childEntityWorlds = new List<World>();

			childEntityWorlds.Add(entityWorldRepository.GetWorld(WorldConstants.SIMULATION_WORLD_ID));
			childEntityWorlds.Add(entityWorldRepository.GetWorld(WorldConstants.VIEW_WORLD_ID));

			ILogger logger =
				loggerResolver?.GetLogger<DefaultECSEntityManager<Guid>>()
				?? null;

			return new SampleEntityManager(
				allocateIDDelegate,
				registryEntityRepository,
				entityWorldRepository,
				childEntityWorlds,
				logger);
		}
		*/
	}
}