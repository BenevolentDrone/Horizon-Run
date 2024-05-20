using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

using World = DefaultEcs.World;

using Entity = DefaultEcs.Entity;

namespace HereticalSolutions.Entities
{
    public class DefaultECSEntityManager<TEntityID>
        : IEntityManager<World, TEntityID, Entity>,
          IContainsEntityWorlds<World, IDefaultECSEntityWorldController>
    {
        private readonly Func<TEntityID> allocateIDDelegate;

        private readonly IRepository<TEntityID, Entity> registryEntitiesRepository;

        private readonly IReadOnlyEntityWorldsRepository<World, IDefaultECSEntityWorldController> entityWorldsRepository;

        //TODO: ensure that it's what this class needs
        private readonly IReadOnlyList<World> childEntityWorlds;

        private readonly ILogger logger;

        public DefaultECSEntityManager(
            Func<TEntityID> allocateIDDelegate,
            IRepository<TEntityID, Entity> registryEntitiesRepository,
            IReadOnlyEntityWorldsRepository<World, IDefaultECSEntityWorldController> entityWorldsRepository,
            IReadOnlyList<World> childEntityWorlds,
            ILogger logger = null)
        {
            this.allocateIDDelegate = allocateIDDelegate;

            this.registryEntitiesRepository = registryEntitiesRepository;

            this.entityWorldsRepository = entityWorldsRepository;

            this.childEntityWorlds = childEntityWorlds;

            this.logger = logger;
        }

        #region IEntityManager

        #region IReadOnlyEntityRepository

        public bool HasEntity(TEntityID entityID)
        {
            return registryEntitiesRepository.Has(entityID);
        }

        public Entity GetRegistryEntity(TEntityID entityID)
        {
            if (!registryEntitiesRepository.TryGet(
                entityID,
                out var result))
                return default(Entity);

            return result;
        }

        public Entity GetEntity(
            TEntityID entityID,
            string worldID)
        {
            if (!registryEntitiesRepository.TryGet(
                entityID,
                out var registryEntity))
                return default(Entity);

            var worldController = entityWorldsRepository.GetWorldController(
                worldID);

            var registryCompliantWorldController = worldController as IRegistryCompliantWorldController<Entity>;

            if (registryCompliantWorldController == null)
                return default(Entity);

            if (!registryCompliantWorldController.TryGetEntityFromRegistry(
                registryEntity,
                out var localEntity))
                return default(Entity);

            return localEntity;
        }

        /*
        public EntityDescriptor<TEntityID>[] AllRegistryEntities
        {
            get
            {
                var keys = registryEntitiesRepository.Keys;
                
                var result = new EntityDescriptor<TEntityID>[keys.Count()];

                int index = 0;
                
                foreach (var key in keys)
                {
                    result[index] = new EntityDescriptor<TEntityID>
                    {
                        ID = key,
                        
                        PrototypeID = registryEntitiesRepository.Get(key).Get<RegistryEntityComponent>().PrototypeID
                    };
                }

                return result;
            }
        }
        */

        public IEnumerable<Entity> AllRegistryEntities => registryEntitiesRepository.Values;
        
        public IEnumerable<TEntityID> AllAllocatedIDs => registryEntitiesRepository.Keys;

        #endregion

        #region Spawn entity
        
        public TEntityID SpawnEntity(
            string prototypeID,
            EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT)
        {
            var newID = AllocateID();
            
            if (!SpawnEntityInAllRelevantWorlds(
                    newID,
                    prototypeID,
                    null,
                    authoringPreset))
                return default(TEntityID);

            logger?.Log<DefaultECSEntityManager<Entity>>(
                $"SPAWNED ENTITY WITH ID {newID} AND PROTOTYPE ID {prototypeID}");
            
            return newID;
        }

        public TEntityID SpawnEntity(
            string prototypeID,
            PrototypeOverride<Entity>[] overrides,
            EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT)
        {
            var newID = AllocateID();
            
            if (!SpawnEntityInAllRelevantWorlds(
                    newID,
                    prototypeID,
                    overrides,
                    authoringPreset))
                return default(TEntityID);

            logger?.Log<DefaultECSEntityManager<Entity>>(
                $"SPAWNED ENTITY WITH ID {newID} AND PROTOTYPE ID {prototypeID}");
            
            return newID;
        }

        public bool SpawnEntity(
            TEntityID entityID,
            string prototypeID,
            EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT)
        {
            return SpawnEntityInAllRelevantWorlds(
                entityID,
                prototypeID,
                null,
                authoringPreset);
        }

        public bool SpawnEntity(
            TEntityID entityID,
            string prototypeID,
            PrototypeOverride<Entity>[] overrides,
            EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT)
        {
            return SpawnEntityInAllRelevantWorlds(
                entityID,
                prototypeID,
                overrides,
                authoringPreset);
        }

        public Entity SpawnWorldLocalEntity(
            string prototypeID,
            string worldID)
        {
            if (!entityWorldsRepository.HasWorld(worldID))
                return default;
            
            var worldController = (IPrototypeCompliantWorldController<World, Entity>)
                entityWorldsRepository.GetWorldController(worldID);

            if (worldController == null)
                return default;
            
            worldController.TrySpawnEntityFromPrototype(
                prototypeID,
                out var localEntity);
            
            logger?.Log<DefaultECSEntityManager<Entity>>(
                $"SPAWNED WORLD LOCAL ENTITY {localEntity} AT WORLD {worldID} WITH PROTOTYPE ID {prototypeID}");

            return localEntity;
        }

        public Entity SpawnWorldLocalEntity(
            string prototypeID,
            Entity @override,
            string worldID)
        {
            if (!entityWorldsRepository.HasWorld(worldID))
                return default;
            
            var worldController = (IPrototypeCompliantWorldController<World, Entity>)
                entityWorldsRepository.GetWorldController(worldID);

            if (worldController == null)
                return default;
            
            worldController.TrySpawnEntityFromPrototype(
                prototypeID,
                @override,
                out var localEntity);
            
            logger?.Log<DefaultECSEntityManager<Entity>>(
                $"SPAWNED WORLD LOCAL ENTITY {localEntity} AT WORLD {worldID} WITH PROTOTYPE ID {prototypeID}");

            return localEntity;
        }

        public Entity SpawnWorldLocalEntity(
            string prototypeID,
            World world)
        {
            if (!entityWorldsRepository.HasWorld(world))
                return default;
            
            var worldController = (IPrototypeCompliantWorldController<World, Entity>)
                entityWorldsRepository.GetWorldController(world);

            if (worldController == null)
                return default;

            worldController.TrySpawnEntityFromPrototype(
                prototypeID,
                out var localEntity);

            logger?.Log<DefaultECSEntityManager<Entity>>(
                $"SPAWNED WORLD LOCAL ENTITY {localEntity} AT WORLD {world} WITH PROTOTYPE ID {prototypeID}");
            
            return localEntity;
        }

        public Entity SpawnWorldLocalEntity(
            string prototypeID,
            Entity @override,
            World world)
        {
            if (!entityWorldsRepository.HasWorld(world))
                return default;
            
            var worldController = (IPrototypeCompliantWorldController<World, Entity>)
                entityWorldsRepository.GetWorldController(world);

            if (worldController == null)
                return default;

            worldController.TrySpawnEntityFromPrototype(
                prototypeID,
                @override,
                out var localEntity);

            logger?.Log<DefaultECSEntityManager<Entity>>(
                $"SPAWNED WORLD LOCAL ENTITY {localEntity} AT WORLD {world} WITH PROTOTYPE ID {prototypeID}");
            
            return localEntity;
        }

        #endregion

        #region Resolve entity

        public TEntityID ResolveEntity(
            object source,
            string prototypeID,
            EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT)
        {
            var newID = AllocateID();

            if (!SpawnAndResolveEntityInAllRelevantWorlds(
                newID,
                prototypeID,
                null,
                source,
                authoringPreset))
                return default(TEntityID);

            logger?.Log<DefaultECSEntityManager<Entity>>(
                $"RESOLVED ENTITY WITH ID {newID} AND PROTOTYPE ID {prototypeID} FROM SOURCE",
                new object[]
                {
                    source
                });
            
            return newID;
        }

        public TEntityID ResolveEntity(
            object source,
            string prototypeID,
            PrototypeOverride<Entity>[] overrides,
            EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT)
        {
            var newID = AllocateID();

            if (!SpawnAndResolveEntityInAllRelevantWorlds(
                    newID,
                    prototypeID,
                    overrides,
                    source,
                    authoringPreset))
                return default(TEntityID);

            logger?.Log<DefaultECSEntityManager<Entity>>(
                $"RESOLVED ENTITY WITH ID {newID} AND PROTOTYPE ID {prototypeID} FROM SOURCE",
                new object[]
                {
                    source
                });
            
            return newID;
        }

        public bool ResolveEntity(
            TEntityID entityID,
            object source,
            string prototypeID,
            EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT)
        {
            return SpawnAndResolveEntityInAllRelevantWorlds(
                entityID,
                prototypeID,
                null,
                source,
                authoringPreset);
        }

        public bool ResolveEntity(
            TEntityID entityID,
            object source,
            string prototypeID,
            PrototypeOverride<Entity>[] overrides,
            EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT)
        {
            return SpawnAndResolveEntityInAllRelevantWorlds(
                entityID,
                prototypeID,
                overrides,
                source,
                authoringPreset);
        }

        public Entity ResolveWorldLocalEntity(
            string prototypeID,
            object source,
            string worldID)
        {
            if (!entityWorldsRepository.HasWorld(worldID))
                return default;
            
            var worldController = (IPrototypeCompliantWorldController<World, Entity>)
                entityWorldsRepository.GetWorldController(worldID);

            if (worldController == null)
                return default;

            worldController.TrySpawnAndResolveEntityFromPrototype(
                prototypeID,
                source,
                out var localEntity);
            
            logger?.Log<DefaultECSEntityManager<Entity>>(
                $"RESOLVED WORLD LOCAL ENTITY {localEntity} AT WORLD {worldID} WITH PROTOTYPE ID {prototypeID} FROM SOURCE",
                new object[]
                {
                    source
                });

            return localEntity;
        }

        public Entity ResolveWorldLocalEntity(
            string prototypeID,
            object source,
            Entity @override,
            string worldID)
        {
            if (!entityWorldsRepository.HasWorld(worldID))
                return default;
            
            var worldController = (IPrototypeCompliantWorldController<World, Entity>)
                entityWorldsRepository.GetWorldController(worldID);

            if (worldController == null)
                return default;

            worldController.TrySpawnAndResolveEntityFromPrototype(
                prototypeID,
                @override,
                source,
                out var localEntity);
            
            logger?.Log<DefaultECSEntityManager<Entity>>(
                $"RESOLVED WORLD LOCAL ENTITY {localEntity} AT WORLD {worldID} WITH PROTOTYPE ID {prototypeID} FROM SOURCE",
                new object[]
                {
                    source
                });

            return localEntity;
        }

        public Entity ResolveWorldLocalEntity(
            string prototypeID,
            object source,
            World world)
        {
            if (!entityWorldsRepository.HasWorld(world))
                return default;
            
            var worldController = (IPrototypeCompliantWorldController<World, Entity>)
                entityWorldsRepository.GetWorldController(world);

            if (worldController == null)
                return default;

            worldController.TrySpawnAndResolveEntityFromPrototype(
                prototypeID,
                source,
                out var localEntity);
            
            logger?.Log<DefaultECSEntityManager<Entity>>(
                $"RESOLVED WORLD LOCAL ENTITY {localEntity} AT WORLD {world} WITH PROTOTYPE ID {prototypeID} FROM SOURCE",
                new object[]
                {
                    source
                });

            return localEntity;
        }

        public Entity ResolveWorldLocalEntity(
            string prototypeID,
            object source,
            Entity @override,
            World world)
        {
            if (!entityWorldsRepository.HasWorld(world))
                return default;
            
            var worldController = (IPrototypeCompliantWorldController<World, Entity>)
                entityWorldsRepository.GetWorldController(world);

            if (worldController == null)
                return default;

            worldController.TrySpawnAndResolveEntityFromPrototype(
                prototypeID,
                @override,
                source,
                out var localEntity);
            
            logger?.Log<DefaultECSEntityManager<Entity>>(
                $"RESOLVED WORLD LOCAL ENTITY {localEntity} AT WORLD {world} WITH PROTOTYPE ID {prototypeID} FROM SOURCE",
                new object[]
                {
                    source
                });

            return localEntity;
        }

        #endregion

        #region Despawn entity

        public void DespawnEntity(TEntityID entityID)
        {
            if (!registryEntitiesRepository.TryGet(
                entityID,
                out var registryEntity))
            {
                logger?.LogError<DefaultECSEntityWorldsRepository>(
                    $"NO ENTITY REGISTERED BY ID {entityID}");

                return;
            }

            foreach (var entityWorld in childEntityWorlds)
            {
                var worldController = (IRegistryCompliantWorldController<Entity>)
                    entityWorldsRepository.GetWorldController(entityWorld);

                if (worldController == null)
                    continue;

                worldController.DespawnEntityAndUnlinkFromRegistry(
                    registryEntity);
            }

            var registryWorldController =
                entityWorldsRepository.GetWorldController(WorldConstants.REGISTRY_WORLD_ID);

            registryWorldController.DespawnEntity(
                registryEntity);

            registryEntitiesRepository.Remove(
                entityID);
            
            logger?.Log<DefaultECSEntityManager<Entity>>(
                $"DESPAWNED ENTITY {entityID}");
        }

        public void DespawnWorldLocalEntity(Entity entity)
        {
            if (entity == default)
                return;

            var world = entity.World;

            var worldController = entityWorldsRepository.GetWorldController(world);

            worldController.DespawnEntity(
                entity);
            
            logger?.Log<DefaultECSEntityManager<Entity>>(
                $"DESPAWNED WORLD LOCAL ENTITY {entity}");
        }

        #endregion

        #endregion

        #region IContainsEntityWorlds

        public IReadOnlyEntityWorldsRepository<World, IDefaultECSEntityWorldController> EntityWorldsRepository { get => entityWorldsRepository; }

        #endregion

        private TEntityID AllocateID()
        {
            /*
            Guid newGUID;

            do
            {
                newGUID = IDAllocationsFactory.BuildGUID();
            }
            while (registryEntitiesRepository.Has(newGUID));

            return newGUID;
            */

            return allocateIDDelegate.Invoke();
        }

        /*
        public void SpawnEntityFromServer(
            Guid guid,
            string prototypeID)
        {
            SpawnEntity(
                guid,
                prototypeID,
                EEntityAuthoringPresets.NETWORKING_CLIENT);
        }
        */

        private bool SpawnEntityInAllRelevantWorlds(
            TEntityID entityID,
            string prototypeID,
            PrototypeOverride<Entity>[] overrides,
            EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT)
        {
            var registryWorldController = (IEntityIDCompliantWorldController<TEntityID, Entity>)
                entityWorldsRepository.GetWorldController(WorldConstants.REGISTRY_WORLD_ID);
            
            if (!registryWorldController.TrySpawnEntityWithIDFromPrototype(
                prototypeID,
                entityID,
                out var registryEntity))
            {
                return false;
            }

            registryEntitiesRepository.Add(
                entityID,
                registryEntity);
            
            switch (authoringPreset)
            {
                case EEntityAuthoringPresets.NONE:
                    break;
                
                case EEntityAuthoringPresets.DEFAULT:
                case EEntityAuthoringPresets.NETWORKING_HOST: //TODO: change
                case EEntityAuthoringPresets.NETWORKING_CLIENT: //TODO: change
                case EEntityAuthoringPresets.NETWORKING_HOST_HEADLESS: //TODO: change
                    foreach (var entityWorld in childEntityWorlds)
                    {
                        var worldController = (IRegistryCompliantWorldController<Entity>)
                            entityWorldsRepository.GetWorldController(entityWorld);

                        if (worldController == null)
                            continue;

                        bool spawnWithOverrideAttempted = false;
                        
                        if (overrides != null)
                        {
                            for (int i = 0; i < overrides.Length; i++)
                            {
                                if (string.IsNullOrEmpty(overrides[i].WorldID))
                                    continue;

                                if (entityWorldsRepository.GetWorld(overrides[i].WorldID) == entityWorld)
                                {
                                    spawnWithOverrideAttempted = true;

                                    worldController.TrySpawnEntityFromRegistry(
                                        registryEntity,
                                        overrides[i].OverrideEntity,
                                        out var localEntity);

                                    break;
                                }
                            }
                        }
                            
                        if (!spawnWithOverrideAttempted)
                        {
                            worldController.TrySpawnEntityFromRegistry(
                                registryEntity,
                                out var localEntity);
                        }
                    }

                    break;
                
                default:
                    break;
            }

            return true;
        }

        /*
        private void RemoveEntityComponentFromRegistry<TEntity>(Entity registryEntity)
        {
            if (registryEntity.Has<TEntity>())
                registryEntity.Remove<TEntity>();
        }
        */

        private bool SpawnAndResolveEntityInAllRelevantWorlds(
            TEntityID entityID,
            string prototypeID,
            PrototypeOverride<Entity>[] overrides,
            object source,
            EEntityAuthoringPresets authoring = EEntityAuthoringPresets.DEFAULT)
        {
            var registryWorldController = (IEntityIDCompliantWorldController<TEntityID, Entity>)
                entityWorldsRepository.GetWorldController(WorldConstants.REGISTRY_WORLD_ID);

            if (!registryWorldController.TrySpawnEntityWithIDFromPrototype(
                    prototypeID,
                    entityID,
                    out var registryEntity))
            {
                return false;
            }

            registryEntitiesRepository.Add(
                entityID,
                registryEntity);
            
            switch (authoring)
            {
                case EEntityAuthoringPresets.NONE:
                    break;
                
                case EEntityAuthoringPresets.DEFAULT:
                case EEntityAuthoringPresets.NETWORKING_HOST: //TODO: change
                case EEntityAuthoringPresets.NETWORKING_CLIENT: //TODO: change
                case EEntityAuthoringPresets.NETWORKING_HOST_HEADLESS: //TODO: change
                    foreach (var entityWorld in childEntityWorlds)
                    {
                        var worldController = (IRegistryCompliantWorldController<Entity>)
                            entityWorldsRepository.GetWorldController(entityWorld);

                        if (worldController == null)
                            continue;
                        
                        bool spawnWithOverrideAttempted = false;

                        if (overrides != null)
                        {
                            for (int i = 0; i < overrides.Length; i++)
                            {
                                if (string.IsNullOrEmpty(overrides[i].WorldID))
                                    continue;

                                if (entityWorldsRepository.GetWorld(overrides[i].WorldID) == entityWorld)
                                {
                                    spawnWithOverrideAttempted = true;

                                    worldController.TrySpawnAndResolveEntityFromRegistry(
                                        registryEntity,
                                        overrides[i].OverrideEntity,
                                        source,
                                        out var localEntity);

                                    break;
                                }
                            }
                        }

                        if (!spawnWithOverrideAttempted)
                        {
                            worldController.TrySpawnAndResolveEntityFromRegistry(
                                registryEntity,
                                source,
                                out var localEntity);
                        }
                    }

                    break;
                
                default:
                    break;
            }

            return true;
        }
    }
}