using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Hierarchy;

using HereticalSolutions.Relations;

using HereticalSolutions.Entities;

using HereticalSolutions.Logging;

using DefaultEcs;


using TWorldID = System.String;

using TWorld = DefaultEcs.World;

using TPrototypeID = System.String;

using TEntityID = System.Guid;

using TEntity = DefaultEcs.Entity;

using TSystem = HereticalSolutions.Modules.Core_DefaultECS.IEntityInitializationSystem;



using TEntityListResource = System.Collections.Generic.List<DefaultEcs.Entity>;

using TEntityListHandle = System.UInt16; //ushort


using TEntityHierarchyResource = HereticalSolutions.Hierarchy.IReadOnlyHierarchyNode<DefaultEcs.Entity>;

using TEntityHierarchyHandle = System.UInt16; //ushort


using TEntityRelationsResource = HereticalSolutions.Relations.IReadOnlyDirectedNamedGraphNode<DefaultEcs.Entity>;

using TEntityRelationsHandle = System.UInt16; //ushort


namespace HereticalSolutions.Modules.Core_DefaultECS.Factories
{
	public static class EntityFactory
	{
		private const int INITIAL_ENTITY_LIST_POOL_CAPACITY = 5;

		private const int ADDITIONAL_ENTITY_LIST_POOL_CAPACITY = 5;

		#region Entity manager

		public static EntityManager BuildEntityManager(
			EEntityAuthoringPresets authoringPreset,
			ILoggerResolver loggerResolver)
		{
			bool includeSimulationWorld = authoringPreset != EEntityAuthoringPresets.NONE;

			bool includeViewWorld = authoringPreset == EEntityAuthoringPresets.DEFAULT
				|| authoringPreset == EEntityAuthoringPresets.NETWORKING_HOST
				|| authoringPreset == EEntityAuthoringPresets.NETWORKING_CLIENT;


			var registryEntityRepository = RepositoryFactory.BuildDictionaryRepository<TEntityID, TEntity>();

			var entityWorldRepository = BuildEntityWorldRepository(loggerResolver);

			List<TWorldID> worldsToSpawnEntitiesIn = new List<TWorldID>();


			entityWorldRepository.AddWorld(
				WorldConstants.REGISTRY_WORLD_ID,
				BuildRegistryWorldController(
					loggerResolver));

			entityWorldRepository.AddWorld(
				WorldConstants.EVENT_WORLD_ID,
				BuildEventEntityWorldController(
					loggerResolver));

			if (includeSimulationWorld)
			{
				var simulationWorld = BuildEntityWorldController(
					(registryEntity) =>
					{
						return registryEntity.Has<SimulationEntityComponent>();
					},
					(TEntity registryEntity, out TPrototypeID prototypeID, out TEntity localEntity) =>
					{
						var component = registryEntity.Get<SimulationEntityComponent>();

						prototypeID = component.PrototypeID;

						localEntity = component.SimulationEntity;
					},
					(TEntity registryEntity, TPrototypeID prototypeID, TEntity localEntity) =>
					{
						registryEntity.Set<SimulationEntityComponent>(
							new SimulationEntityComponent
							{
								PrototypeID = prototypeID,

								SimulationEntity = localEntity
							});
					},
					(registryEntity) =>
					{
						registryEntity.Remove<SimulationEntityComponent>();
					},
					loggerResolver);

				entityWorldRepository.AddWorld(
					WorldConstants.SIMULATION_WORLD_ID,
					simulationWorld);

				worldsToSpawnEntitiesIn.Add(WorldConstants.SIMULATION_WORLD_ID);
			}

			if (includeViewWorld)
			{
				var viewWorld = BuildEntityWorldController(
					(registryEntity) =>
					{
						return registryEntity.Has<ViewEntityComponent>();
					},
					(TEntity registryEntity, out TPrototypeID prototypeID, out TEntity localEntity) =>
					{
						var component = registryEntity.Get<ViewEntityComponent>();

						prototypeID = component.PrototypeID;

						localEntity = component.ViewEntity;
					},
					(TEntity registryEntity, TPrototypeID prototypeID, TEntity localEntity) =>
					{
						registryEntity.Set<ViewEntityComponent>(
							new ViewEntityComponent
							{
								PrototypeID = prototypeID,

								ViewEntity = localEntity
							});
					},
					(registryEntity) =>
					{
						registryEntity.Remove<ViewEntityComponent>();
					},
					loggerResolver);

				entityWorldRepository.AddWorld(
					WorldConstants.VIEW_WORLD_ID,
					viewWorld);

				worldsToSpawnEntitiesIn.Add(WorldConstants.VIEW_WORLD_ID);
			}

			ILogger logger =
				loggerResolver?.GetLogger<EntityManager>();

			return new EntityManager(
				registryEntityRepository,
				entityWorldRepository,
				worldsToSpawnEntitiesIn.ToArray(),
				logger);
		}

		#endregion

		#region Entity world controllers

		public static EventEntityWorldController BuildEventEntityWorldController(
			ILoggerResolver loggerResolver)
		{
			TWorld world = new TWorld();

			ILogger logger =
				loggerResolver?.GetLogger<EventEntityWorldController>();

			return new EventEntityWorldController(
				world,
				logger);
		}

		public static RegistryWorldController BuildRegistryWorldController(
			ILoggerResolver loggerResolver)
		{
			TWorld world = new TWorld();

			ILogger logger =
				loggerResolver?.GetLogger<RegistryWorldController>();

			return new RegistryWorldController(
				world,

				BuildEntityPrototypeRepository(),
				new ComponentCloner(),
				logger);
		}

		public static EntityWorldController BuildEntityWorldController(
			HasWorldIdentityComponentDelegate<TEntity> hasWorldIdentityComponentDelegate,
			GetWorldIdentityComponentDelegate<TPrototypeID, TEntity> getWorldIdentityComponentDelegate,
			SetWorldIdentityComponentDelegate<TPrototypeID, TEntity> setWorldIdentityComponentDelegate,
			RemoveWorldIdentityComponentDelegate<TEntity> removeWorldIdentityComponentDelegate,

			ILoggerResolver loggerResolver)
		{
			World world = new World();

			ILogger logger =
				loggerResolver?.GetLogger<EntityWorldController>();

			return new EntityWorldController(
					world,
					BuildEntityPrototypeRepository(),
					new ComponentCloner(),

					hasWorldIdentityComponentDelegate,
					getWorldIdentityComponentDelegate,
					setWorldIdentityComponentDelegate,
					removeWorldIdentityComponentDelegate,

					logger);
		}

		#endregion

/*
		#region Entity list manager

		public static EntityListManager BuildEntityListManager(
			ILoggerResolver loggerResolver)
		{
			Func<TEntityListResource> valueAllocationDelegate =
				AllocationFactory.ActivatorAllocationDelegate<TEntityListResource>;

			var initialAllocationCommand = new AllocationCommand<TEntityListResource>
			{
				Descriptor = new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = INITIAL_ENTITY_LIST_POOL_CAPACITY
				},
				AllocationDelegate = valueAllocationDelegate
			};

			var additionalAllocationCommand = new AllocationCommand<TEntityListResource>
			{
				Descriptor = new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = ADDITIONAL_ENTITY_LIST_POOL_CAPACITY
				},
				AllocationDelegate = valueAllocationDelegate
			};

			return new EntityListManager(
				RepositoryFactory.BuildDictionaryRepository<TEntityListHandle, TEntityListResource>(),
				new Queue<TEntityListHandle>(),
				(handle) => { return ++handle; },
				new PoolWithListCleanup<TEntityListResource>(
					StackPoolFactory.BuildStackPool<TEntityListResource>(
						initialAllocationCommand,
						additionalAllocationCommand,
						loggerResolver)),
				logger: loggerResolver?.GetLogger<EntityListManager>());
		}

		#endregion

		#region Entity hierarchy manager

		public static EntityHierarchyManager BuildEntityHierarchyManager(
			ILoggerResolver loggerResolver)
		{
			Func<TEntityHierarchyResource> valueAllocationDelegate =
				() => AllocationFactory.FuncAllocationDelegate<TEntityHierarchyResource, HierarchyNode<TEntity>>(
					() =>
					{
						return new HierarchyNode<TEntity>(
							new List<TEntityHierarchyResource>());
					});

			var initialAllocationCommand = new AllocationCommand<TEntityHierarchyResource>
			{
				Descriptor = new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = INITIAL_ENTITY_LIST_POOL_CAPACITY
				},
				AllocationDelegate = valueAllocationDelegate
			};

			var additionalAllocationCommand = new AllocationCommand<TEntityHierarchyResource>
			{
				Descriptor = new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = ADDITIONAL_ENTITY_LIST_POOL_CAPACITY
				},
				AllocationDelegate = valueAllocationDelegate
			};

			return new EntityHierarchyManager(
				RepositoryFactory.BuildDictionaryRepository<TEntityHierarchyHandle, TEntityHierarchyResource>(),
				new Queue<TEntityHierarchyHandle>(),
				(handle) => { return ++handle; },
				new PoolWithCleanup<TEntityHierarchyResource>(
					StackPoolFactory.BuildStackPool<TEntityHierarchyResource>(
						initialAllocationCommand,
						additionalAllocationCommand,
						loggerResolver)),
				logger: loggerResolver?.GetLogger<EntityHierarchyManager>());
		}

		#endregion

		#region Entity relations manager

		public static EntityRelationsManager BuildEntityRelationsManager(
			ILoggerResolver loggerResolver)
		{
			Func<TEntityRelationsResource> valueAllocationDelegate =
				() => AllocationFactory.FuncAllocationDelegate<
					TEntityRelationsResource,
					DirectedNamedGraphNode<TEntity>>(
					() =>
					{
						return new DirectedNamedGraphNode<TEntity>(
							RepositoryFactory
								.BuildDictionaryRepository<string, TEntityRelationsResource>(),
							new List<RelationDTO<TEntity>>());
					});

			var initialAllocationCommand = new AllocationCommand<TEntityRelationsResource>
			{
				Descriptor = new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = INITIAL_ENTITY_LIST_POOL_CAPACITY
				},
				AllocationDelegate = valueAllocationDelegate
			};

			var additionalAllocationCommand = new AllocationCommand<TEntityRelationsResource>
			{
				Descriptor = new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = ADDITIONAL_ENTITY_LIST_POOL_CAPACITY
				},
				AllocationDelegate = valueAllocationDelegate
			};

			return new EntityRelationsManager(
				RepositoryFactory.BuildDictionaryRepository<TEntityRelationsHandle, TEntityRelationsResource>(),
				new Queue<TEntityRelationsHandle>(),
				(handle) => { return ++handle; },
				new PoolWithCleanup<TEntityRelationsResource>(
					StackPoolFactory.BuildStackPool<TEntityRelationsResource>(
						initialAllocationCommand,
						additionalAllocationCommand,
						loggerResolver)),
				logger: loggerResolver?.GetLogger<EntityRelationsManager>());
		}

		#endregion
*/

		#region Entity prototype repository

		public static EntityPrototypeRepository BuildEntityPrototypeRepository()
		{
			return new EntityPrototypeRepository(
				new TWorld(),
				RepositoryFactory.BuildDictionaryRepository<TPrototypeID, TEntity>());
		}

		#endregion

		#region Entity world repository

		public static EntityWorldRepository BuildEntityWorldRepository(
			ILoggerResolver loggerResolver)
		{
			var worldRepository = RepositoryFactory.BuildDictionaryRepository<TWorldID, TWorld>();

			var worldControllerRepository = RepositoryFactory.BuildDictionaryRepository<
				TWorld,
				IEntityWorldController<TWorld, TEntity>>();

			ILogger logger =
				loggerResolver?.GetLogger<EntityWorldRepository>();

			return new EntityWorldRepository(
				worldRepository,
				worldControllerRepository,
				logger);
		}

		#endregion

		#region Event entity builder

		public static EventEntityBuilder BuildEventEntityBuilder<TEntityID>(
			World world)
		{
			return new EventEntityBuilder(
				world);
		}

		#endregion
	}
}