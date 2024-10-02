using HereticalSolutions.Entities;

using HereticalSolutions.Logging;

using DefaultEcs;


using TWorld = DefaultEcs.World;

using TPrototypeID = System.String;

using TEntityID = System.Guid;

using TEntity = DefaultEcs.Entity;

using TSystem = HereticalSolutions.Modules.Core_DefaultECS.IEntityInitializationSystem;


namespace HereticalSolutions.Modules.Core_DefaultECS
{
    public class EntityWorldController
        : IEntityWorldController<TWorld, TEntity>,
          IEntityWorldControllerWithPrototypes<TWorld, TPrototypeID, TEntity>,
          IEntityWorldControllerWithPrototypesAndIDs<TWorld, TPrototypeID, TEntityID, TEntity>,
          IEntityWorldControllerWithRegistry<TWorld, TPrototypeID, TEntity>,
          IEntityWorldControllerWithLifeCycleSystems<TSystem, TWorld, TEntity>
    {
        private readonly IEntityPrototypeRepositoryWithWorld<TWorld, TPrototypeID, TEntity> prototypeRepository;
        
        private readonly ComponentCloner componentCloner;

        #region Systems

        private IEntityInitializationSystem resolveSystems;

        private IEntityInitializationSystem initializationSystems;

        private IEntityInitializationSystem deinitializationSystems;

        #endregion

        #region Delegates

        private readonly HasWorldIdentityComponentDelegate<TEntity> hasWorldIdentityComponentDelegate;

        private readonly GetWorldIdentityComponentDelegate<TPrototypeID, TEntity> getWorldIdentityComponentDelegate;

        private readonly SetWorldIdentityComponentDelegate<TPrototypeID, TEntity> setWorldIdentityComponentDelegate;

        private readonly RemoveWorldIdentityComponentDelegate<TEntity> removeWorldIdentityComponentDelegate;

        #endregion

        private readonly ILogger logger;
        

        public EntityWorldController(
            TWorld world,
            IEntityPrototypeRepositoryWithWorld<TWorld, TPrototypeID, TEntity> prototypeRepository,
            ComponentCloner componentCloner,

            HasWorldIdentityComponentDelegate<TEntity> hasWorldIdentityComponentDelegate,
            GetWorldIdentityComponentDelegate<TPrototypeID, TEntity> getWorldIdentityComponentDelegate,
            SetWorldIdentityComponentDelegate<TPrototypeID, TEntity> setWorldIdentityComponentDelegate,
            RemoveWorldIdentityComponentDelegate<TEntity> removeWorldIdentityComponentDelegate,

            ILogger logger = null)
        {
            World = world;


            this.prototypeRepository = prototypeRepository;
            
            this.componentCloner = componentCloner;


            this.hasWorldIdentityComponentDelegate = hasWorldIdentityComponentDelegate;

            this.getWorldIdentityComponentDelegate = getWorldIdentityComponentDelegate;

            this.setWorldIdentityComponentDelegate = setWorldIdentityComponentDelegate;

            this.removeWorldIdentityComponentDelegate = removeWorldIdentityComponentDelegate;


            this.logger = logger;


            resolveSystems = null;

            initializationSystems = null;

            deinitializationSystems = null;
        }

        #region IEntityWorldController

        public TWorld World { get; private set; }

        public bool TrySpawnEntity(
            out TEntity entity)
        {
            entity = World.CreateEntity();

            //Process freshly spawned entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }

        public bool TrySpawnAndResolveEntity(
            object source,
            out TEntity entity)
        {
            entity = World.CreateEntity();

            //Mark entity as in need of resolving and provide a source as a payload to the component
            entity.Set<ResolveComponent>(
                new ResolveComponent
                {
                    Source = source
                });

            //Process freshly spawned entity with resolve systems
            resolveSystems?.Update(entity);

            //Don't need it anymore. Bye!
            entity.Remove<ResolveComponent>();

            //Process freshly resolved entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }

        public bool DespawnEntity(
            TEntity entity)
        {
            if (entity == default)
                return false;

            if (entity.World != World)
            {
                logger?.LogError(
                    GetType(),
                    $"ATTEMPT TO DESPAWN ENTITY FROM THE WRONG WORLD");

                return false;
            }

            if (entity.Has<DespawnComponent>())
            {
                return false;
            }

            //Mark the entity for despawn
            entity.Set<DespawnComponent>();

            //Process the entity on its way to be despawned with deinitialization systems
            deinitializationSystems?.Update(entity);

            return true;
        }

        #endregion

        #region IEntityWorldControllerWithPrototypes

        public IEntityPrototypeRepositoryWithWorld<TWorld, TPrototypeID, TEntity> PrototypeRepository { get => prototypeRepository; }


        public bool TrySpawnEntityFromPrototype(
            TPrototypeID prototypeID,
            out TEntity entity)
        {
            if (!TryClonePrototypeEntityToWorld(
                prototypeID,
                out entity))
            {
                return false;
            }

            //Process freshly spawned entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }
        
        public bool TrySpawnEntityFromPrototype(
            TPrototypeID prototypeID,
            TEntity @override,
            out TEntity entity)
        {
            if (!TryClonePrototypeEntityToWorld(
                prototypeID,
                @override,
                out entity))
            {
                return false;
            }

            //Process freshly spawned entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }

        public bool TrySpawnAndResolveEntityFromPrototype(
            TPrototypeID prototypeID,
            object source,
            out TEntity entity)
        {
            if (!TryClonePrototypeEntityToWorld(
                prototypeID,
                out entity))
            {
                return false;
            }


            //Mark entity as in need of resolving and provide a source as a payload to the component
            entity.Set<ResolveComponent>(
                new ResolveComponent
                {
                    Source = source
                });

            //Process freshly spawned entity with resolve systems
            resolveSystems?.Update(entity);

            //Don't need it anymore. Bye!
            entity.Remove<ResolveComponent>();


            //Process freshly resolved entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }
        
        public bool TrySpawnAndResolveEntityFromPrototype(
            TPrototypeID prototypeID,
            TEntity @override,
            object source,
            out TEntity entity)
        {
            if (!TryClonePrototypeEntityToWorld(
                prototypeID,
                @override,
                out entity))
            {
                return false;
            }


            //Mark entity as in need of resolving and provide a source as a payload to the component
            entity.Set<ResolveComponent>(
                new ResolveComponent
                {
                    Source = source
                });

            //Process freshly spawned entity with resolve systems
            resolveSystems?.Update(entity);

            //Don't need it anymore. Bye!
            entity.Remove<ResolveComponent>();


            //Process freshly resolved entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }

        #endregion

        #region IEntityWorldControllerWithPrototypesAndIDs

        public bool TrySpawnEntityWithIDFromPrototype(
            TPrototypeID prototypeID,
            TEntityID entityID,
            out TEntity entity)
        {
            if (!TryClonePrototypeEntityToWorld(
                prototypeID,
                out entity))
            {
                return false;
            }

            entity.Set<GUIDComponent>(
                new GUIDComponent
                {
                    GUID = entityID
                });
            

            //Process freshly spawned entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }
        
        public bool TrySpawnEntityWithIDFromPrototype(
            TPrototypeID prototypeID,
            TEntityID entityID,
            TEntity @override,
            out TEntity entity)
        {
            if (!TryClonePrototypeEntityToWorld(
                prototypeID,
                @override,
                out entity))
            {
                return false;
            }

            entity.Set<GUIDComponent>(
                new GUIDComponent
                {
                    GUID = entityID
                });


            //Process freshly spawned entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }

        public bool TrySpawnAndResolveEntityWithIDFromPrototype(
            TPrototypeID prototypeID,
            TEntityID entityID,
            object source,
            out TEntity entity)
        {
            if (!TryClonePrototypeEntityToWorld(
                prototypeID,
                out entity))
            {
                return false;
            }

            entity.Set<GUIDComponent>(
                new GUIDComponent
                {
                    GUID = entityID
                });


            //Mark entity as in need of resolving and provide a source as a payload to the component
            entity.Set<ResolveComponent>(
                new ResolveComponent
                {
                    Source = source
                });

            //Process freshly spawned entity with resolve systems
            resolveSystems?.Update(entity);

            //Don't need it anymore. Bye!
            entity.Remove<ResolveComponent>();


            //Process freshly resolved entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }
        
        public bool TrySpawnAndResolveEntityWithIDFromPrototype(
            TPrototypeID prototypeID,
            TEntityID entityID,
            TEntity @override,
            object source,
            out TEntity entity)
        {
            if (!TryClonePrototypeEntityToWorld(
                prototypeID,
                @override,
                out entity))
            {
                return false;
            }

            entity.Set<GUIDComponent>(
                new GUIDComponent
                {
                    GUID = entityID
                });


            //Mark entity as in need of resolving and provide a source as a payload to the component
            entity.Set<ResolveComponent>(
                new ResolveComponent
                {
                    Source = source
                });

            //Process freshly spawned entity with resolve systems
            resolveSystems?.Update(entity);

            //Don't need it anymore. Bye!
            entity.Remove<ResolveComponent>();


            //Process freshly resolved entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }

        #endregion

        #region IEntityWorldControllerWithRegistry

        public bool TryGetEntityFromRegistry(
            TEntity registryEntity,
            out TEntity localEntity)
        {
            //if (!registryEntity.Has<TWorldIdentityComponent>())
            if (!hasWorldIdentityComponentDelegate.Invoke(registryEntity))
            {
                localEntity = default;

                return false;
            }

            //ref var entityIdentityComponent = ref registryEntity.Get<TWorldIdentityComponent>();
            //
            //localEntity = getEntityFromWorldIdentityComponentDelegate.Invoke(
            //    entityIdentityComponent);

            getWorldIdentityComponentDelegate.Invoke(
                registryEntity,
                out var prototypeID,
                out localEntity);

            return true;
        }

        public bool TrySpawnEntityFromRegistry(
            TEntity registryEntity,
            out TEntity localEntity)
        {
            localEntity = default;

            //if (!registryEntity.Has<TWorldIdentityComponent>())
            if (!hasWorldIdentityComponentDelegate.Invoke(
                registryEntity))
            {
                return false;
            }

            //Get the target ID from the registry entity
            var entityID = registryEntity.Get<GUIDComponent>().GUID;

            //Get the prototype ID from the registry entity

            //var entityIdentityComponent = registryEntity.Get<TWorldIdentityComponent>();
            //
            //var prototypeID = getPrototypeIDFromWorldIdentityComponentDelegate.Invoke(
            //    entityIdentityComponent);
            getWorldIdentityComponentDelegate.Invoke(
                registryEntity,
                out var prototypeID,
                out _);


            if (!TrySpawnEntityWithIDFromPrototype(
                prototypeID,
                entityID,
                out localEntity))
            {
                return false;
            }

            //And now let's link registry entity to the one we just created

            //registryEntity.Set<TWorldIdentityComponent>(
            //    createWorldIdentityComponentDelegate.Invoke(
            //        prototypeID,
            //        localEntity));
            setWorldIdentityComponentDelegate.Invoke(
                registryEntity,
                prototypeID,
                localEntity);

            return true;
        }
        
        public bool TrySpawnEntityFromRegistry(
            TEntity registryEntity,
            TEntity overrideEntity,
            out TEntity localEntity)
        {
            localEntity = default;

            //if (!registryEntity.Has<TWorldIdentityComponent>())
            if (!hasWorldIdentityComponentDelegate.Invoke(
                registryEntity))
            {
                return false;
            }

            //Get the target ID from the registry entity
            var entityID = registryEntity.Get<GUIDComponent>().GUID;

            //Get the prototype ID from the registry entity

            //var entityIdentityComponent = registryEntity.Get<TWorldIdentityComponent>();
            //
            //var prototypeID = getPrototypeIDFromWorldIdentityComponentDelegate.Invoke(
            //    entityIdentityComponent);
            getWorldIdentityComponentDelegate.Invoke(
                registryEntity,
                out var prototypeID,
                out _);


            if (!TrySpawnEntityWithIDFromPrototype(
                prototypeID,
                entityID,
                overrideEntity,
                out localEntity))
            {
                return false;
            }

            //And now let's link registry entity to the one we just created

            //registryEntity.Set<TWorldIdentityComponent>(
            //    createWorldIdentityComponentDelegate.Invoke(
            //        prototypeID,
            //        localEntity));
            setWorldIdentityComponentDelegate.Invoke(
                registryEntity,
                prototypeID,
                localEntity);

            return true;
        }

        public bool TrySpawnAndResolveEntityFromRegistry(
            TEntity registryEntity,
            object source,
            out TEntity localEntity)
        {
            localEntity = default;

            //if (!registryEntity.Has<TWorldIdentityComponent>())
            if (!hasWorldIdentityComponentDelegate.Invoke(
                registryEntity))
            {
                return false;
            }

            //Get the target ID from the registry entity
            var entityID = registryEntity.Get<GUIDComponent>().GUID;


            //Get the prototype ID from the registry entity

            //var entityIdentityComponent = registryEntity.Get<TWorldIdentityComponent>();
            //
            //var prototypeID = getPrototypeIDFromWorldIdentityComponentDelegate.Invoke(entityIdentityComponent);
            getWorldIdentityComponentDelegate.Invoke(
                registryEntity,
                out var prototypeID,
                out _);


            if (!TrySpawnAndResolveEntityWithIDFromPrototype(
                prototypeID,
                entityID,
                source,
                out localEntity))
            {
                return false;
            }

            //And now let's link registry entity to the one we just created

            //registryEntity.Set<TWorldIdentityComponent>(
            //    createWorldIdentityComponentDelegate.Invoke(
            //        prototypeID,
            //        localEntity));
            setWorldIdentityComponentDelegate.Invoke(
                registryEntity,
                prototypeID,
                localEntity);

            return true;
        }
        
        public bool TrySpawnAndResolveEntityFromRegistry(
            TEntity registryEntity,
            TEntity overrideEntity,
            object source,
            out TEntity localEntity)
        {
            localEntity = default;

            //if (!registryEntity.Has<TWorldIdentityComponent>())
            if (!hasWorldIdentityComponentDelegate.Invoke(
                registryEntity))
            {
                return false;
            }

            //Get the target ID from the registry entity
            var entityID = registryEntity.Get<GUIDComponent>().GUID;


            //Get the prototype ID from the registry entity

            //var entityIdentityComponent = registryEntity.Get<TWorldIdentityComponent>();
            //
            //var prototypeID = getPrototypeIDFromWorldIdentityComponentDelegate.Invoke(entityIdentityComponent);
            getWorldIdentityComponentDelegate.Invoke(
                registryEntity,
                out var prototypeID,
                out _);


            if (!TrySpawnAndResolveEntityWithIDFromPrototype(
                prototypeID,
                entityID,
                overrideEntity,
                source,
                out localEntity))
            {
                return false;
            }

            //And now let's link registry entity to the one we just created

            //registryEntity.Set<TWorldIdentityComponent>(
            //    createWorldIdentityComponentDelegate.Invoke(
            //        prototypeID,
            //        localEntity));
            setWorldIdentityComponentDelegate.Invoke(
                registryEntity,
                prototypeID,
                localEntity);

            return true;
        }

        public bool TrySpawnEntityFromPrototypeAndLinkToRegistry(
            TEntity registryEntity,
            TPrototypeID prototypeID,
            out TEntity localEntity)
        {
            localEntity = default;

            //If there's already an entity of this world linked to the registry entity, we're done here
            //if (registryEntity.Has<TWorldIdentityComponent>())
            if (hasWorldIdentityComponentDelegate.Invoke(
                registryEntity))
            {
                return false;
            }

            //Get the target ID from the registry entity
            var entityID = registryEntity.Get<GUIDComponent>().GUID;

            if (!TrySpawnEntityWithIDFromPrototype(
                prototypeID,
                entityID,
                out localEntity))
            {
                return false;
            }

            //And now let's link registry entity to the one we just created

            //registryEntity.Set<TWorldIdentityComponent>(
            //    createWorldIdentityComponentDelegate.Invoke(
            //        prototypeID,
            //        localEntity));
            setWorldIdentityComponentDelegate.Invoke(
                registryEntity,
                prototypeID,
                localEntity);

            return true;
        }

        public bool TrySpawnAndResolveEntityFromPrototypeAndLinkToRegistry(
            TEntity registryEntity,
            TPrototypeID prototypeID,
            object source,
            out TEntity localEntity)
        {
            localEntity = default;

            //If there's already an entity of this world linked to the registry entity, we're done here
            //if (registryEntity.Has<TWorldIdentityComponent>())
            if (hasWorldIdentityComponentDelegate.Invoke(
                registryEntity))
            {
                return false;
            }

            //Get the target ID from the registry entity
            var entityID = registryEntity.Get<GUIDComponent>().GUID;


            if (!TrySpawnAndResolveEntityWithIDFromPrototype(
                prototypeID,
                entityID,
                source,
                out localEntity))
            {
                return false;
            }

            //And now let's link registry entity to the one we just created

            //registryEntity.Set<TWorldIdentityComponent>(
            //    createWorldIdentityComponentDelegate.Invoke(
            //        prototypeID,
            //        localEntity));
            setWorldIdentityComponentDelegate.Invoke(
                registryEntity,
                prototypeID,
                localEntity);

            return true;
        }

        public void DespawnEntityAndUnlinkFromRegistry(
            TEntity registryEntity)
        {
            //if (!registryEntity.Has<TWorldIdentityComponent>())
            if (!hasWorldIdentityComponentDelegate.Invoke(
                registryEntity))
                return;

            //ref var entityIdentityComponent = ref registryEntity.Get<TWorldIdentityComponent>();
            //
            //var localEntity = getEntityFromWorldIdentityComponentDelegate.Invoke(entityIdentityComponent);
            getWorldIdentityComponentDelegate.Invoke(
                registryEntity,
                out var _,
                out var localEntity);

            DespawnEntity(localEntity);

            //registryEntity.Remove<TWorldIdentityComponent>();
            removeWorldIdentityComponentDelegate.Invoke(
                registryEntity);
        }

        public bool TryReplaceEntityFromPrototypeAndUpdateRegistry(
            TEntity registryEntity,
            TPrototypeID prototypeID,
            out TEntity localEntity)
        {
            bool alreadyHasIdentityComponent =
                //registryEntity.Has<TWorldIdentityComponent>();
                hasWorldIdentityComponentDelegate.Invoke(
                    registryEntity);

            if (alreadyHasIdentityComponent)
            {
                //ref var entityIdentityComponent = ref registryEntity.Get<TWorldIdentityComponent>();
                //
                //var previousEntity = getEntityFromWorldIdentityComponentDelegate.Invoke(entityIdentityComponent);
                getWorldIdentityComponentDelegate.Invoke(
                    registryEntity,
                    out var _,
                    out var previousEntity);

                DespawnEntity(previousEntity);
            }

            //Get the target ID from the registry entity
            var entityID = registryEntity.Get<GUIDComponent>().GUID;

            if (!TrySpawnEntityWithIDFromPrototype(
                prototypeID,
                entityID,
                out localEntity))
            {
                //registryEntity.Remove<TWorldIdentityComponent>();
                removeWorldIdentityComponentDelegate.Invoke(
                    registryEntity);

                return false;
            }

            //And now let's link registry entity to the one we just created

            //registryEntity.Set<TWorldIdentityComponent>(
            //    createWorldIdentityComponentDelegate.Invoke(
            //        prototypeID,
            //        localEntity));
            setWorldIdentityComponentDelegate.Invoke(
                registryEntity,
                prototypeID,
                localEntity);

            return true;
        }

        public bool TryReplaceAndResolveEntityFromPrototypeAndUpdateRegistry(
            TEntity registryEntity,
            TPrototypeID prototypeID,
            object source,
            out TEntity localEntity)
        {
            bool alreadyHasIdentityComponent =
                //registryEntity.Has<TWorldIdentityComponent>();
                hasWorldIdentityComponentDelegate.Invoke(
                    registryEntity);

            if (alreadyHasIdentityComponent)
            {
                //ref var entityIdentityComponent = ref registryEntity.Get<TWorldIdentityComponent>();
                //
                //var previousEntity = getEntityFromWorldIdentityComponentDelegate.Invoke(entityIdentityComponent);
                getWorldIdentityComponentDelegate.Invoke(
                    registryEntity,
                    out var _,
                    out var previousEntity);

                DespawnEntity(previousEntity);
            }

            //Get the target ID from the registry entity

            var entityID = registryEntity.Get<GUIDComponent>().GUID;

            if (!TrySpawnAndResolveEntityWithIDFromPrototype(
                prototypeID,
                entityID,
                source,
                out localEntity))
            {
                //registryEntity.Remove<TWorldIdentityComponent>();
                removeWorldIdentityComponentDelegate.Invoke(
                    registryEntity);

                return false;
            }

            //And now let's link registry entity to the one we just created

            //registryEntity.Set<TWorldIdentityComponent>(
            //    createWorldIdentityComponentDelegate.Invoke(
            //        prototypeID,
            //        localEntity));
            setWorldIdentityComponentDelegate.Invoke(
                registryEntity,
                prototypeID,
                localEntity);

            return true;
        }

        #endregion

        #region IEntityWorldControllerWithLifeCycleSystems

        public TSystem EntityResolveSystems
        {
            get => resolveSystems;
            set => resolveSystems = value;
        }

        public TSystem EntityInitializationSystems
        {
            get => initializationSystems;
            set => initializationSystems = value;
        }

        public TSystem EntityDeinitializationSystems
        {
            get => deinitializationSystems;
            set => deinitializationSystems = value;
        }

        #endregion

        private bool TryClonePrototypeEntityToWorld(
            TPrototypeID prototypeID,
            out TEntity entity)
        {
            entity = default(TEntity);

            if (string.IsNullOrEmpty(prototypeID))
            {
                logger?.LogError(
                    GetType(),
                    $"INVALID PROTOTYPE ID");

                return false;
            }

            if (!prototypeRepository.TryGetPrototype(
                prototypeID,
                out var prototypeEntity))
            {
                logger?.LogError(
                    GetType(),
                    $"NO PROTOTYPE REGISTERED BY ID {prototypeID}");

                return false;
            }

            if (prototypeEntity.Has<NestedPrototypeComponent>())
            {
                if (!TryClonePrototypeEntityToWorld(
                    prototypeEntity.Get<NestedPrototypeComponent>().BasePrototypeID,
                    out entity))
                {
                    return false;
                }
                
                componentCloner.Clone(
                    prototypeEntity,
                    entity);
                
                entity.Remove<NestedPrototypeComponent>();
            }
            else
            {
                entity = prototypeEntity.CopyTo(
                    World,
                    componentCloner);
            }
            
            entity.Set<PrototypeInstanceComponent>(
                new PrototypeInstanceComponent
                {
                    PrototypeID = prototypeID
                });

            return true;
        }
        
        private bool TryClonePrototypeEntityToWorld(
            TPrototypeID prototypeID,
            TEntity @override,
            out TEntity entity)
        {
            entity = default(TEntity);

            if (string.IsNullOrEmpty(prototypeID))
            {
                logger?.LogError(
                    GetType(),
                    $"INVALID PROTOTYPE ID");

                return false;
            }

            if (!prototypeRepository.TryGetPrototype(
                prototypeID,
                out var prototypeEntity))
            {
                logger?.LogError(
                    GetType(),
                    $"NO PROTOTYPE REGISTERED BY ID {prototypeID}");

                return false;
            }

            if (prototypeEntity.Has<NestedPrototypeComponent>())
            {
                if (!TryClonePrototypeEntityToWorld(
                    prototypeEntity.Get<NestedPrototypeComponent>().BasePrototypeID,
                    out entity))
                {
                    return false;
                }
                
                componentCloner.Clone(
                    prototypeEntity,
                    entity);
                
                entity.Remove<NestedPrototypeComponent>();
            }
            else
            {
                entity = prototypeEntity.CopyTo(
                    World,
                    componentCloner);
            }
            
            componentCloner.Clone(
                @override,
                entity);
            
            @override.Dispose();
            
            entity.Set<PrototypeInstanceComponent>(
                new PrototypeInstanceComponent
                {
                    PrototypeID = prototypeID
                });

            return true;
        }
    }
}