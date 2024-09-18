using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Hierarchy;

using HereticalSolutions.Relations;

using HereticalSolutions.Logging;

using DefaultEcs;

namespace HereticalSolutions.Entities.Factories
{
    /// <summary>
    /// Class containing methods to build entities and their components.
    /// </summary>
    public static partial class DefaultECSEntityFactory
    {
        private const int INITIAL_ENTITY_LIST_POOL_CAPACITY = 5;
        
        private const int ADDITIONAL_ENTITY_LIST_POOL_CAPACITY = 5;
        
        #region Entity manager

        public static DefaultECSEntityManager<TEntityID> BuildDefaultECSSimpleEntityManager<TEntityID, TEntityIDComponent>(
            Func<TEntityID> allocateIDDelegate,

            Func<TEntityIDComponent, TEntityID> getEntityIDFromIDComponentDelegate,
            Func<TEntityID, TEntityIDComponent> createIDComponentDelegate,

            ILoggerResolver loggerResolver = null)
        {
            var registryEntityRepository = RepositoriesFactory.BuildDictionaryRepository<TEntityID, Entity>();

            var entityWorldsRepository = BuildDefaultECSEntityWorldsRepository(loggerResolver);


            entityWorldsRepository.AddWorld(
                WorldConstants.REGISTRY_WORLD_ID,
                BuildDefaultECSRegistryWorldController(
                    createIDComponentDelegate,
                    BuildDefaultECSPrototypesRepository(),
                    loggerResolver));

            entityWorldsRepository.AddWorld(
                WorldConstants.EVENT_WORLD_ID,
                BuildDefaultECSEventWorldController(
                    loggerResolver));

            entityWorldsRepository.AddWorld(
                WorldConstants.SIMULATION_WORLD_ID,
                BuildDefaultECSWorldController
                    <TEntityID,
                    TEntityIDComponent,
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
                BuildDefaultECSWorldController
                    <TEntityID,
                    TEntityIDComponent,
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
                loggerResolver?.GetLogger<DefaultECSEntityManager<TEntityID>>()
                ?? null;

            return new DefaultECSEntityManager<TEntityID>(
                allocateIDDelegate,
                registryEntityRepository,
                entityWorldsRepository,
                childEntityWorlds,
                logger);
        }

        public static DefaultECSEventWorldController BuildDefaultECSEventWorldController(
            ILoggerResolver loggerResolver = null)
        {
            World world = new World();

            ILogger logger =
                loggerResolver?.GetLogger<DefaultECSEventWorldController>()
                ?? null;

            return new DefaultECSEventWorldController(
                world,
                logger);
        }

        public static DefaultECSRegistryWorldController<TEntityID, TEntityIDComponent>
            BuildDefaultECSRegistryWorldController<TEntityID, TEntityIDComponent>(
                Func<TEntityID, TEntityIDComponent> createIDComponentDelegate,
                IPrototypesRepository<World, Entity> prototypeRepository,
                ILoggerResolver loggerResolver = null)
        {
            World world = new World();

            ILogger logger =
                loggerResolver?.GetLogger<DefaultECSRegistryWorldController<TEntityID, TEntityIDComponent>>()
                ?? null;

            return new DefaultECSRegistryWorldController<TEntityID, TEntityIDComponent>(
                world,

                createIDComponentDelegate,

                prototypeRepository,
                new ComponentCloner(),
                logger);
        }

        public static DefaultECSWorldController
            <TEntityID,
            TEntityIDComponent,
            TWorldIdentityComponent,
            TResolveWorldIdentityComponent>
            BuildDefaultECSWorldController
                <TEntityID,
                TEntityIDComponent,
                TWorldIdentityComponent,
                TResolveWorldIdentityComponent>(
                    Func<TEntityIDComponent, TEntityID> getEntityIDFromIDComponentDelegate,
                    Func<TEntityID, TEntityIDComponent> createIDComponentDelegate,

                    Func<TWorldIdentityComponent, Entity> getEntityFromWorldIdentityComponentDelegate,
                    Func<TWorldIdentityComponent, string> getPrototypeIDFromWorldIdentityComponentDelegate,
                    Func<string, Entity, TWorldIdentityComponent> createWorldIdentityComponentDelegate,

                    Func<object, TResolveWorldIdentityComponent> createResolveWorldIdentityComponentDelegate,
                    ILoggerResolver loggerResolver = null)
        {
            World world = new World();

            ILogger logger =
                loggerResolver?.GetLogger
                    <DefaultECSWorldController
                        <TEntityID,
                        TEntityIDComponent,
                        TWorldIdentityComponent,
                        TResolveWorldIdentityComponent>>()
                ?? null;

            return new DefaultECSWorldController
                <TEntityID,
                TEntityIDComponent,
                TWorldIdentityComponent,
                TResolveWorldIdentityComponent>(
                    world,

                    getEntityIDFromIDComponentDelegate,
                    createIDComponentDelegate,

                    getEntityFromWorldIdentityComponentDelegate,
                    getPrototypeIDFromWorldIdentityComponentDelegate,
                    createWorldIdentityComponentDelegate,

                    createResolveWorldIdentityComponentDelegate,

                    BuildDefaultECSPrototypesRepository(),
                    new ComponentCloner(),
                    logger);
        }

        #endregion

        #region Entity list manager

        public static DefaultECSEntityListManager BuildDefaultECSEntityListManager(
            ILoggerResolver loggerResolver = null)
        {
            Func<List<Entity>> valueAllocationDelegate =
                AllocationsFactory.ActivatorAllocationDelegate<List<Entity>>;

            var initialAllocationCommand = new AllocationCommand<List<Entity>>
            {
                Descriptor = new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = INITIAL_ENTITY_LIST_POOL_CAPACITY
                },
                AllocationDelegate = valueAllocationDelegate
            };
            
            var additionalAllocationCommand = new AllocationCommand<List<Entity>>
            {
                Descriptor = new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,
                    
                    Amount = ADDITIONAL_ENTITY_LIST_POOL_CAPACITY
                },
                AllocationDelegate = valueAllocationDelegate
            };
            
            return new DefaultECSEntityListManager(
                RepositoriesFactory.BuildDictionaryRepository<ushort, List<Entity>>(),
                new Queue<ushort>(),
                (handle) => { return ++handle; },
                //() => AllocationsFactory.ActivatorAllocationDelegate<List<Entity>>(),
                new PoolWithListCleanup<List<Entity>>(
                    StackPoolFactory.BuildStackPool<List<Entity>>(
                        initialAllocationCommand,
                        additionalAllocationCommand,
                        loggerResolver)),
                loggerResolver?.GetLogger<DefaultECSEntityListManager>());
        }

        #endregion
        
        #region Entity hierarchy manager

        public static DefaultECSEntityHierarchyManager BuildDefaultECSEntityHierarchyManager(
            ILoggerResolver loggerResolver = null)
        {
            Func<IReadOnlyHierarchyNode<Entity>> valueAllocationDelegate =
                () => AllocationsFactory.FuncAllocationDelegate<IReadOnlyHierarchyNode<Entity>, HierarchyNode<Entity>>(
                    () =>
                    {
                        return new HierarchyNode<Entity>(
                            new List<IReadOnlyHierarchyNode<Entity>>());
                    });

            var initialAllocationCommand = new AllocationCommand<IReadOnlyHierarchyNode<Entity>>
            {
                Descriptor = new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = INITIAL_ENTITY_LIST_POOL_CAPACITY
                },
                AllocationDelegate = valueAllocationDelegate
            };
            
            var additionalAllocationCommand = new AllocationCommand<IReadOnlyHierarchyNode<Entity>>
            {
                Descriptor = new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,
                    
                    Amount = ADDITIONAL_ENTITY_LIST_POOL_CAPACITY
                },
                AllocationDelegate = valueAllocationDelegate
            };
            
            return new DefaultECSEntityHierarchyManager(
                RepositoriesFactory.BuildDictionaryRepository<ushort, IReadOnlyHierarchyNode<Entity>>(),
                new Queue<ushort>(),
                (handle) => { return ++handle; },
                //() => AllocationsFactory.FuncAllocationDelegate<IReadOnlyHierarchyNode<Entity>>(
                //    () =>
                //    {
                //        return new HierarchyNode<Entity>(
                //            new List<IReadOnlyHierarchyNode<Entity>>());
                //    }),
                new PoolWithCleanup<IReadOnlyHierarchyNode<Entity>>(
                    StackPoolFactory.BuildStackPool<IReadOnlyHierarchyNode<Entity>>(
                        initialAllocationCommand,
                        additionalAllocationCommand,
                        loggerResolver)),
                loggerResolver?.GetLogger<DefaultECSEntityHierarchyManager>());
        }

        #endregion
        
        #region Entity relations manager

        public static DefaultECSEntityRelationsManager BuildDefaultECSEntityRelationsManager(
            ILoggerResolver loggerResolver = null)
        {
            Func<IReadOnlyDirectedNamedGraphNode<Entity>> valueAllocationDelegate =
                () => AllocationsFactory.FuncAllocationDelegate<
                    IReadOnlyDirectedNamedGraphNode<Entity>,
                    DirectedNamedGraphNode<Entity>>(
                    () =>
                    {
                        return new DirectedNamedGraphNode<Entity>(
                            RepositoriesFactory
                                .BuildDictionaryRepository<string, IReadOnlyDirectedNamedGraphNode<Entity>>(),
                            new List<RelationDTO<Entity>>());
                    });

            var initialAllocationCommand = new AllocationCommand<IReadOnlyDirectedNamedGraphNode<Entity>>
            {
                Descriptor = new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                    Amount = INITIAL_ENTITY_LIST_POOL_CAPACITY
                },
                AllocationDelegate = valueAllocationDelegate
            };
            
            var additionalAllocationCommand = new AllocationCommand<IReadOnlyDirectedNamedGraphNode<Entity>>
            {
                Descriptor = new AllocationCommandDescriptor
                {
                    Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,
                    
                    Amount = ADDITIONAL_ENTITY_LIST_POOL_CAPACITY
                },
                AllocationDelegate = valueAllocationDelegate
            };
            
            return new DefaultECSEntityRelationsManager(
                RepositoriesFactory.BuildDictionaryRepository<ushort, IReadOnlyDirectedNamedGraphNode<Entity>>(),
                new Queue<ushort>(),
                (handle) => { return ++handle; },
                new PoolWithCleanup<IReadOnlyDirectedNamedGraphNode<Entity>>(
                    StackPoolFactory.BuildStackPool<IReadOnlyDirectedNamedGraphNode<Entity>>(
                        initialAllocationCommand,
                        additionalAllocationCommand,
                        loggerResolver)),
                loggerResolver?.GetLogger<DefaultECSEntityRelationsManager>());
        }

        #endregion
        
        #region Prototypes repository

        public static DefaultECSPrototypesRepository BuildDefaultECSPrototypesRepository()
        {
            return new DefaultECSPrototypesRepository(
                new World(),
                RepositoriesFactory.BuildDictionaryRepository<string, Entity>());
        }

        #endregion

        #region Entity worlds repository

        public static IDefaultECSEntityWorldsRepository BuildDefaultECSEntityWorldsRepository(
            ILoggerResolver loggerResolver = null)
        {
            var worldsRepository = RepositoriesFactory.BuildDictionaryRepository<string, World>();

            var worldControllersRepository = RepositoriesFactory.BuildDictionaryRepository<World, IDefaultECSEntityWorldController>();

            ILogger logger =
                loggerResolver?.GetLogger<DefaultECSEntityWorldsRepository>()
                ?? null;

            return new DefaultECSEntityWorldsRepository(
                worldsRepository,
                worldControllersRepository,
                logger);
        }

        #endregion

        #region Subaddress manager

        public static SubaddressManager BuildSubaddressManager(
            ILogger logger = null)
        {
            return new SubaddressManager(
                RepositoriesFactory.BuildDictionaryOneToOneMap<string, ushort>(),
                logger);
        }

        #endregion
        
        #region Event entity builder

        public static DefaultECSEventEntityBuilder<TEntityID> BuildDefaultECSEventEntityBuilder<TEntityID>(
            World world)
        {
            return new DefaultECSEventEntityBuilder<TEntityID>(
                world);
        }

        #endregion
    }
}