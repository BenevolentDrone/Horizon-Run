using System;

using DefaultEcs;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Entities
{
    public class DefaultECSWorldController
        <TEntityID,
        TEntityIDComponent,
        TWorldIdentityComponent,
        TResolveWorldIdentityComponent>
        : IDefaultECSEntityWorldController,
          IPrototypeCompliantWorldController<World, Entity>,
          IEntityIDCompliantWorldController<TEntityID, Entity>,
          IRegistryCompliantWorldController<Entity>,
          IContainsEntityInitializationSystems<IDefaultECSEntityInitializationSystem>
    {
        #region Delegates

        #region ID component

        private readonly Func<TEntityIDComponent, TEntityID> getEntityIDFromIDComponentDelegate;

        private readonly Func<TEntityID, TEntityIDComponent> createIDComponentDelegate;

        #endregion

        #region World identity component

        private readonly Func<TWorldIdentityComponent, Entity> getEntityFromWorldIdentityComponentDelegate;

        private readonly Func<TWorldIdentityComponent, string> getPrototypeIDFromWorldIdentityComponentDelegate;

        private readonly Func<string, Entity, TWorldIdentityComponent> createWorldIdentityComponentDelegate;

        #endregion

        #region Resolve world identity component

        private readonly Func<object, TResolveWorldIdentityComponent> createResolveWorldIdentityComponentDelegate;

        #endregion

        #endregion

        private readonly IPrototypesRepository<World, Entity> prototypeRepository;
        
        private readonly ComponentCloner componentCloner;

        #region Systems

        private IDefaultECSEntityInitializationSystem resolveSystems;

        private IDefaultECSEntityInitializationSystem initializationSystems;

        private IDefaultECSEntityInitializationSystem deinitializationSystems;

        #endregion

        private readonly ILogger logger;
        

        public DefaultECSWorldController(
            World world,

            Func<TEntityIDComponent, TEntityID> getEntityIDFromIDComponentDelegate,
            Func<TEntityID, TEntityIDComponent> createIDComponentDelegate,

            Func<TWorldIdentityComponent, Entity> getEntityFromWorldIdentityComponentDelegate,
            Func<TWorldIdentityComponent, string> getPrototypeIDFromWorldIdentityComponentDelegate,
            Func<string, Entity, TWorldIdentityComponent> createWorldIdentityComponentDelegate,

            Func<object, TResolveWorldIdentityComponent> createResolveWorldIdentityComponentDelegate,

            IPrototypesRepository<World, Entity> prototypeRepository,
            ComponentCloner componentCloner,
            ILogger logger = null)
        {
            World = world;


            this.getEntityIDFromIDComponentDelegate = getEntityIDFromIDComponentDelegate;

            this.createIDComponentDelegate = createIDComponentDelegate;


            this.getEntityFromWorldIdentityComponentDelegate = getEntityFromWorldIdentityComponentDelegate;

            this.getPrototypeIDFromWorldIdentityComponentDelegate = getPrototypeIDFromWorldIdentityComponentDelegate;

            this.createWorldIdentityComponentDelegate = createWorldIdentityComponentDelegate;


            this.createResolveWorldIdentityComponentDelegate = createResolveWorldIdentityComponentDelegate;


            this.prototypeRepository = prototypeRepository;
            
            this.componentCloner = componentCloner;

            this.logger = logger;


            resolveSystems = null;

            initializationSystems = null;

            deinitializationSystems = null;
        }

        #region IContainsEntityInitializationSystems

        public IDefaultECSEntityInitializationSystem EntityResolveSystems { get => resolveSystems; }

        public IDefaultECSEntityInitializationSystem EntityInitializationSystems { get => initializationSystems; }

        public IDefaultECSEntityInitializationSystem EntityDeinitializationSystems { get => deinitializationSystems; }

        public void Initialize(
            IDefaultECSEntityInitializationSystem resolveSystems,
            IDefaultECSEntityInitializationSystem initializationSystems,
            IDefaultECSEntityInitializationSystem deinitializationSystems)
        {
            this.resolveSystems = resolveSystems;

            this.initializationSystems = initializationSystems;

            this.deinitializationSystems = deinitializationSystems;
        }

        #endregion

        #region IWorldController

        public World World { get; private set; }

        public bool TrySpawnEntity(
            out Entity entity)
        {
            entity = World.CreateEntity();

            //Process freshly spawned entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }

        public bool TrySpawnAndResolveEntity(
            object source,
            out Entity entity)
        {
            entity = World.CreateEntity();

            //Mark entity as in need of resolving and provide a source as a payload to the component
            entity.Set<TResolveWorldIdentityComponent>(
                createResolveWorldIdentityComponentDelegate.Invoke(source));

            //Process freshly spawned entity with resolve systems
            resolveSystems?.Update(entity);

            //Don't need it anymore. Bye!
            entity.Remove<TResolveWorldIdentityComponent>();

            //Process freshly resolved entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }

        public void DespawnEntity(
            Entity entity)
        {
            if (entity == default)
                return;

            if (entity.World != World)
                logger?.LogError<DefaultECSWorldController<TEntityID, TEntityIDComponent, TWorldIdentityComponent, TResolveWorldIdentityComponent>>(
                    $"ATTEMPT TO DESPAWN ENTITY FROM THE WRONG WORLD");

            if (entity.Has<DespawnComponent>())
                return;

            //Mark the entity for despawn
            entity.Set<DespawnComponent>();

            //Process the entity on its way to be despawned with deinitialization systems
            deinitializationSystems?.Update(entity);
        }

        #endregion

        #region IPrototypeCompliantWorldController

        public IPrototypesRepository<World, Entity> PrototypeRepository { get => prototypeRepository; }

        public bool TrySpawnEntityFromPrototype(
            string prototypeID,
            out Entity entity)
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
            string prototypeID,
            Entity @override,
            out Entity entity)
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
            string prototypeID,
            object source,
            out Entity entity)
        {
            if (!TryClonePrototypeEntityToWorld(
                prototypeID,
                out entity))
            {
                return false;
            }


            //Mark entity as in need of resolving and provide a source as a payload to the component
            entity.Set<TResolveWorldIdentityComponent>(
                createResolveWorldIdentityComponentDelegate.Invoke(source));

            //Process freshly spawned entity with resolve systems
            resolveSystems?.Update(entity);

            //Don't need it anymore. Bye!
            entity.Remove<TResolveWorldIdentityComponent>();


            //Process freshly resolved entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }
        
        public bool TrySpawnAndResolveEntityFromPrototype(
            string prototypeID,
            Entity @override,
            object source,
            out Entity entity)
        {
            if (!TryClonePrototypeEntityToWorld(
                prototypeID,
                @override,
                out entity))
            {
                return false;
            }


            //Mark entity as in need of resolving and provide a source as a payload to the component
            entity.Set<TResolveWorldIdentityComponent>(
                createResolveWorldIdentityComponentDelegate.Invoke(source));

            //Process freshly spawned entity with resolve systems
            resolveSystems?.Update(entity);

            //Don't need it anymore. Bye!
            entity.Remove<TResolveWorldIdentityComponent>();


            //Process freshly resolved entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }

        #endregion

        #region IEntituyIDCompliantWorldController

        public bool TrySpawnEntityWithIDFromPrototype(
            string prototypeID,
            TEntityID entityID,
            out Entity entity)
        {
            /*
            if (!TrySpawnEntityFromPrototype(
                prototypeID,
                out entity))
            {
                return false;
            }
            */

            if (!TryClonePrototypeEntityToWorld(
                prototypeID,
                out entity))
            {
                return false;
            }

            entity.Set<TEntityIDComponent>(
                createIDComponentDelegate.Invoke(entityID));
            

            //Process freshly spawned entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }
        
        public bool TrySpawnEntityWithIDFromPrototype(
            string prototypeID,
            TEntityID entityID,
            Entity @override,
            out Entity entity)
        {
            if (!TryClonePrototypeEntityToWorld(
                prototypeID,
                @override,
                out entity))
            {
                return false;
            }

            entity.Set<TEntityIDComponent>(
                createIDComponentDelegate.Invoke(entityID));
            

            //Process freshly spawned entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }

        public bool TrySpawnAndResolveEntityWithIDFromPrototype(
            string prototypeID,
            TEntityID entityID,
            object source,
            out Entity entity)
        {
            /*
            if (!TrySpawnAndResolveEntityFromPrototype(
                prototypeID,
                source,
                out entity))
            {
                return false;
            }
            */

            if (!TryClonePrototypeEntityToWorld(
                prototypeID,
                out entity))
            {
                return false;
            }

            entity.Set<TEntityIDComponent>(
                createIDComponentDelegate.Invoke(entityID));


            //Mark entity as in need of resolving and provide a source as a payload to the component
            entity.Set<TResolveWorldIdentityComponent>(
                createResolveWorldIdentityComponentDelegate.Invoke(source));

            //Process freshly spawned entity with resolve systems
            resolveSystems?.Update(entity);

            //Don't need it anymore. Bye!
            entity.Remove<TResolveWorldIdentityComponent>();


            //Process freshly resolved entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }
        
        public bool TrySpawnAndResolveEntityWithIDFromPrototype(
            string prototypeID,
            TEntityID entityID,
            Entity @override,
            object source,
            out Entity entity)
        {
            if (!TryClonePrototypeEntityToWorld(
                prototypeID,
                @override,
                out entity))
            {
                return false;
            }

            entity.Set<TEntityIDComponent>(
                createIDComponentDelegate.Invoke(entityID));


            //Mark entity as in need of resolving and provide a source as a payload to the component
            entity.Set<TResolveWorldIdentityComponent>(
                createResolveWorldIdentityComponentDelegate.Invoke(source));

            //Process freshly spawned entity with resolve systems
            resolveSystems?.Update(entity);

            //Don't need it anymore. Bye!
            entity.Remove<TResolveWorldIdentityComponent>();


            //Process freshly resolved entity with initialization systems
            initializationSystems?.Update(entity);

            return true;
        }

        #endregion

        #region IRegistryCompliantWorldController

        public bool TryGetEntityFromRegistry(
            Entity registryEntity,
            out Entity localEntity)
        {
            if (!registryEntity.Has<TWorldIdentityComponent>())
            {
                localEntity = default;

                return false;
            }

            ref var entityIdentityComponent = ref registryEntity.Get<TWorldIdentityComponent>();

            localEntity = getEntityFromWorldIdentityComponentDelegate.Invoke(entityIdentityComponent);

            return true;
        }

        public bool TrySpawnEntityFromRegistry(
            Entity registryEntity,
            out Entity localEntity)
        {
            localEntity = default;

            if (!registryEntity.Has<TWorldIdentityComponent>())
            {
                return false;
            }

            //Get the target ID from the registry entity

            //var guid = registryEntity.Get<GUIDComponent>().GUID;

            var entityID = getEntityIDFromIDComponentDelegate.Invoke(
                registryEntity.Get<TEntityIDComponent>());


            //Get the prototype ID from the registry entity
            var entityIdentityComponent = registryEntity.Get<TWorldIdentityComponent>();

            var prototypeID = getPrototypeIDFromWorldIdentityComponentDelegate.Invoke(entityIdentityComponent);


            if (!TrySpawnEntityWithIDFromPrototype(
                prototypeID,
                entityID,
                out localEntity))
            {
                return false;
            }

            //And now let's link registry entity to the one we just created
            registryEntity.Set<TWorldIdentityComponent>(
                createWorldIdentityComponentDelegate.Invoke(
                    prototypeID,
                    localEntity));

            return true;
        }
        
        public bool TrySpawnEntityFromRegistry(
            Entity registryEntity,
            Entity overrideEntity,
            out Entity localEntity)
        {
            localEntity = default;

            if (!registryEntity.Has<TWorldIdentityComponent>())
            {
                return false;
            }

            //Get the target ID from the registry entity

            var entityID = getEntityIDFromIDComponentDelegate.Invoke(
                registryEntity.Get<TEntityIDComponent>());


            //Get the prototype ID from the registry entity
            var entityIdentityComponent = registryEntity.Get<TWorldIdentityComponent>();

            var prototypeID = getPrototypeIDFromWorldIdentityComponentDelegate.Invoke(entityIdentityComponent);


            if (!TrySpawnEntityWithIDFromPrototype(
                prototypeID,
                entityID,
                overrideEntity,
                out localEntity))
            {
                return false;
            }

            //And now let's link registry entity to the one we just created
            registryEntity.Set<TWorldIdentityComponent>(
                createWorldIdentityComponentDelegate.Invoke(
                    prototypeID,
                    localEntity));

            return true;
        }

        public bool TrySpawnAndResolveEntityFromRegistry(
            Entity registryEntity,
            object source,
            out Entity localEntity)
        {
            localEntity = default;

            if (!registryEntity.Has<TWorldIdentityComponent>())
            {
                return false;
            }

            //Get the target ID from the registry entity

            //var guid = registryEntity.Get<GUIDComponent>().GUID;

            var entityID = getEntityIDFromIDComponentDelegate.Invoke(
                registryEntity.Get<TEntityIDComponent>());


            //Get the prototype ID from the registry entity
            var entityIdentityComponent = registryEntity.Get<TWorldIdentityComponent>();

            var prototypeID = getPrototypeIDFromWorldIdentityComponentDelegate.Invoke(entityIdentityComponent);


            if (!TrySpawnAndResolveEntityWithIDFromPrototype(
                prototypeID,
                entityID,
                source,
                out localEntity))
            {
                return false;
            }

            //And now let's link registry entity to the one we just created
            registryEntity.Set<TWorldIdentityComponent>(
                createWorldIdentityComponentDelegate.Invoke(
                    prototypeID,
                    localEntity));

            return true;
        }
        
        public bool TrySpawnAndResolveEntityFromRegistry(
            Entity registryEntity,
            Entity overrideEntity,
            object source,
            out Entity localEntity)
        {
            localEntity = default;

            if (!registryEntity.Has<TWorldIdentityComponent>())
            {
                return false;
            }

            //Get the target ID from the registry entity

            //var guid = registryEntity.Get<GUIDComponent>().GUID;

            var entityID = getEntityIDFromIDComponentDelegate.Invoke(
                registryEntity.Get<TEntityIDComponent>());


            //Get the prototype ID from the registry entity
            var entityIdentityComponent = registryEntity.Get<TWorldIdentityComponent>();

            var prototypeID = getPrototypeIDFromWorldIdentityComponentDelegate.Invoke(entityIdentityComponent);


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
            registryEntity.Set<TWorldIdentityComponent>(
                createWorldIdentityComponentDelegate.Invoke(
                    prototypeID,
                    localEntity));

            return true;
        }

        public bool TrySpawnEntityFromPrototypeAndLinkToRegistry(
            Entity registryEntity,
            string prototypeID,
            out Entity localEntity)
        {
            localEntity = default;

            //If there's already an entity of this world linked to the registry entity, we're done here
            if (registryEntity.Has<TWorldIdentityComponent>())
            {
                return false;
            }

            //Get the target ID from the registry entity

            //var guid = registryEntity.Get<GUIDComponent>().GUID;

            var entityID = getEntityIDFromIDComponentDelegate.Invoke(
                registryEntity.Get<TEntityIDComponent>());

            if (!TrySpawnEntityWithIDFromPrototype(
                prototypeID,
                entityID,
                out localEntity))
            {
                return false;
            }

            //And now let's link registry entity to the one we just created
            registryEntity.Set<TWorldIdentityComponent>(
                createWorldIdentityComponentDelegate.Invoke(
                    prototypeID,
                    localEntity));

            return true;
        }

        public bool TrySpawnAndResolveEntityFromPrototypeAndLinkToRegistry(
            Entity registryEntity,
            string prototypeID,
            object source,
            out Entity localEntity)
        {
            localEntity = default;

            //If there's already an entity of this world linked to the registry entity, we're done here
            if (registryEntity.Has<TWorldIdentityComponent>())
            {
                return false;
            }

            //Get the target ID from the registry entity

            //var guid = registryEntity.Get<GUIDComponent>().GUID;

            var entityID = getEntityIDFromIDComponentDelegate.Invoke(
                registryEntity.Get<TEntityIDComponent>());


            if (!TrySpawnAndResolveEntityWithIDFromPrototype(
                prototypeID,
                entityID,
                source,
                out localEntity))
            {
                return false;
            }

            //And now let's link registry entity to the one we just created
            registryEntity.Set<TWorldIdentityComponent>(
                createWorldIdentityComponentDelegate.Invoke(
                    prototypeID,
                    localEntity));

            return true;
        }

        public void DespawnEntityAndUnlinkFromRegistry(
            Entity registryEntity)
        {
            if (!registryEntity.Has<TWorldIdentityComponent>())
                return;

            ref var entityIdentityComponent = ref registryEntity.Get<TWorldIdentityComponent>();

            var localEntity = getEntityFromWorldIdentityComponentDelegate.Invoke(entityIdentityComponent);

            DespawnEntity(localEntity);

            registryEntity.Remove<TWorldIdentityComponent>();
        }

        public bool TryReplaceEntityFromPrototypeAndUpdateRegistry(
            Entity registryEntity,
            string prototypeID,
            out Entity localEntity)
        {
            bool alreadyHasIdentityComponent = registryEntity.Has<TWorldIdentityComponent>();

            if (alreadyHasIdentityComponent)
            {
                ref var entityIdentityComponent = ref registryEntity.Get<TWorldIdentityComponent>();

                var previousEntity = getEntityFromWorldIdentityComponentDelegate.Invoke(entityIdentityComponent);

                DespawnEntity(previousEntity);
            }

            //Get the target ID from the registry entity

            //var guid = registryEntity.Get<GUIDComponent>().GUID;

            var entityID = getEntityIDFromIDComponentDelegate.Invoke(
                registryEntity.Get<TEntityIDComponent>());

            if (!TrySpawnEntityWithIDFromPrototype(
                prototypeID,
                entityID,
                out localEntity))
            {
                registryEntity.Remove<TWorldIdentityComponent>();

                return false;
            }

            //And now let's link registry entity to the one we just created
            registryEntity.Set<TWorldIdentityComponent>(
                createWorldIdentityComponentDelegate.Invoke(
                    prototypeID,
                    localEntity));

            return true;
        }

        public bool TryReplaceAndResolveEntityFromPrototypeAndUpdateRegistry(
            Entity registryEntity,
            string prototypeID,
            object source,
            out Entity localEntity)
        {
            bool alreadyHasIdentityComponent = registryEntity.Has<TWorldIdentityComponent>();

            if (alreadyHasIdentityComponent)
            {
                ref var entityIdentityComponent = ref registryEntity.Get<TWorldIdentityComponent>();

                var previousEntity = getEntityFromWorldIdentityComponentDelegate.Invoke(entityIdentityComponent);

                DespawnEntity(previousEntity);
            }

            //Get the target ID from the registry entity

            //var guid = registryEntity.Get<GUIDComponent>().GUID;

            var entityID = getEntityIDFromIDComponentDelegate.Invoke(
                registryEntity.Get<TEntityIDComponent>());

            if (!TrySpawnAndResolveEntityWithIDFromPrototype(
                prototypeID,
                entityID,
                source,
                out localEntity))
            {
                registryEntity.Remove<TWorldIdentityComponent>();

                return false;
            }

            //And now let's link registry entity to the one we just created
            registryEntity.Set<TWorldIdentityComponent>(
                createWorldIdentityComponentDelegate.Invoke(
                    prototypeID,
                    localEntity));

            return true;
        }

        #endregion

        private bool TryClonePrototypeEntityToWorld(
            string prototypeID,
            out Entity entity)
        {
            entity = default(Entity);

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
            string prototypeID,
            Entity @override,
            out Entity entity)
        {
            entity = default(Entity);

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