using System;

using DefaultEcs;
using DefaultEcs.System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Entities
{
    public class DefaultECSRegistryWorldController<TEntityID, TEntityIDComponent>
        : IDefaultECSEntityWorldController,
          IPrototypeCompliantWorldController<World, Entity>,
          IEntityIDCompliantWorldController<TEntityID, Entity>
    {
        #region ID component

        private readonly Func<TEntityID, TEntityIDComponent> createIDComponentDelegate;

        #endregion

        private readonly IPrototypesRepository<World, Entity> prototypeRepository;

        private readonly ComponentCloner componentCloner;

        private readonly ILogger logger;

        public DefaultECSRegistryWorldController(
            World world,

            Func<TEntityID, TEntityIDComponent> createIDComponentDelegate,

            IPrototypesRepository<World, Entity> prototypeRepository,
            ComponentCloner componentCloner,
            ILogger logger = null)
        {
            World = world;


            this.createIDComponentDelegate = createIDComponentDelegate;


            this.prototypeRepository = prototypeRepository;
            
            this.componentCloner = componentCloner;

            this.logger = logger;
        }
        
        #region IWorldController
        
        public World World { get; private set; }

        public bool TrySpawnEntity(
            out Entity entity)
        {
            entity = World.CreateEntity();

            return true;
        }

        public bool TrySpawnAndResolveEntity(
            object source,
            out Entity entity)
        {
            //There's no use in resolving in registry world (for now)
            return TrySpawnEntity(
                out entity);
        }

        public void DespawnEntity(
            Entity entity)
        {
            if (entity == default)
                return;

            if (entity.World != World)
                logger?.LogError<DefaultECSRegistryWorldController<TEntityID, TEntityIDComponent>>(
                    $"ATTEMPT TO DESPAWN ENTITY FROM THE WRONG WORLD");

            if (entity.Has<DespawnComponent>())
                return;

            entity.Set<DespawnComponent>();
        }

        #endregion

        #region IPrototypeCompliantWorldController

        public IPrototypesRepository<World, Entity> PrototypeRepository { get => prototypeRepository; }

        public bool TrySpawnEntityFromPrototype(
            string prototypeID,
            out Entity entity)
        {
            entity = default(Entity);

            if (string.IsNullOrEmpty(prototypeID))
            {
                logger?.LogError<DefaultECSRegistryWorldController<TEntityID, TEntityIDComponent>>(
                    $"INVALID PROTOTYPE ID");

                return false;
            }

            if (!prototypeRepository.TryGetPrototype(
                prototypeID,
                out var prototypeEntity))
            {
                logger?.LogError<DefaultECSRegistryWorldController<TEntityID, TEntityIDComponent>>(
                    $"NO PROTOTYPE REGISTERED BY ID {prototypeID}");

                return false;
            }

            entity = prototypeEntity.CopyTo(
                World,
                componentCloner);
            
            entity.Set<PrototypeInstanceComponent>(
                new PrototypeInstanceComponent
                {
                    PrototypeID = prototypeID
                });

            return true;
        }
        
        public bool TrySpawnEntityFromPrototype(
            string prototypeID,
            Entity @override,
            out Entity entity)
        {
            entity = default(Entity);

            if (string.IsNullOrEmpty(prototypeID))
            {
                logger?.LogError<DefaultECSRegistryWorldController<TEntityID, TEntityIDComponent>>(
                    $"INVALID PROTOTYPE ID");

                return false;
            }

            if (!prototypeRepository.TryGetPrototype(
                    prototypeID,
                    out var prototypeEntity))
            {
                logger?.LogError<DefaultECSRegistryWorldController<TEntityID, TEntityIDComponent>>(
                    $"NO PROTOTYPE REGISTERED BY ID {prototypeID}");

                return false;
            }

            entity = prototypeEntity.CopyTo(
                World,
                componentCloner);
            
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

        public bool TrySpawnAndResolveEntityFromPrototype(
            string prototypeID,
            object source,
            out Entity entity)
        {
            //There's no use in resolving in registry world (for now)
            return TrySpawnEntityFromPrototype(
                prototypeID,
                out entity);
        }

        public bool TrySpawnAndResolveEntityFromPrototype(
            string prototypeID,
            Entity @override,
            object source,
            out Entity entity)
        {
            //There's no use in resolving in registry world (for now)
            return TrySpawnEntityFromPrototype(
                prototypeID,
                @override,
                out entity);
        }

        #endregion

        #region IEntityIDCompliantWorldController

        public bool TrySpawnEntityWithIDFromPrototype(
            string prototypeID,
            TEntityID entityID,
            out Entity entity)
        {
            if (!TrySpawnEntityFromPrototype(
                prototypeID,
                out entity))
            {
                return false;
            }

            //ref GUIDComponent guidComponent = ref entity.Get<GUIDComponent>();
            //
            //guidComponent.GUID = guid;

            entity.Set<TEntityIDComponent>(
                createIDComponentDelegate.Invoke(entityID));

            return true;
        }
        
        public bool TrySpawnEntityWithIDFromPrototype(
            string prototypeID,
            TEntityID entityID,
            Entity @override,
            out Entity entity)
        {
            if (!TrySpawnEntityFromPrototype(
                prototypeID,
                @override,
                out entity))
            {
                return false;
            }

            //ref GUIDComponent guidComponent = ref entity.Get<GUIDComponent>();
            //
            //guidComponent.GUID = guid;

            entity.Set<TEntityIDComponent>(
                createIDComponentDelegate.Invoke(entityID));

            return true;
        }

        public bool TrySpawnAndResolveEntityWithIDFromPrototype(
            string prototypeID,
            TEntityID entityID,
            object source,
            out Entity entity)
        {
            if (!TrySpawnAndResolveEntityFromPrototype(
                prototypeID,
                source,
                out entity))
            {
                return false;
            }

            //ref GUIDComponent guidComponent = ref entity.Get<GUIDComponent>();
            //
            //guidComponent.GUID = guid;

            entity.Set<TEntityIDComponent>(
                createIDComponentDelegate.Invoke(entityID));

            return true;
        }
        
        public bool TrySpawnAndResolveEntityWithIDFromPrototype(
            string prototypeID,
            TEntityID entityID,
            Entity @override,
            object source,
            out Entity entity)
        {
            if (!TrySpawnAndResolveEntityFromPrototype(
                prototypeID,
                @override,
                source,
                out entity))
            {
                return false;
            }

            //ref GUIDComponent guidComponent = ref entity.Get<GUIDComponent>();
            //
            //guidComponent.GUID = guid;

            entity.Set<TEntityIDComponent>(
                createIDComponentDelegate.Invoke(entityID));

            return true;
        }

        #endregion
    }
}