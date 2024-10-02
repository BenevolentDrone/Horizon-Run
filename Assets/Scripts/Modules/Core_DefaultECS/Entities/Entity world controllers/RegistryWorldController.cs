using HereticalSolutions.Entities;

using HereticalSolutions.Logging;

using DefaultEcs;


using TWorld = DefaultEcs.World;

using TPrototypeID = System.String;

using TEntityID = System.Guid;

using TEntity = DefaultEcs.Entity;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
    public class RegistryWorldController
        : IEntityWorldController<TWorld, TEntity>,
          IEntityWorldControllerWithPrototypes<TWorld, TPrototypeID, TEntity>,
          IEntityWorldControllerWithPrototypesAndIDs<TWorld, TPrototypeID, TEntityID, TEntity>
    {
        private readonly IEntityPrototypeRepositoryWithWorld<TWorld, TPrototypeID, TEntity> prototypeRepository;

        private readonly ComponentCloner componentCloner;

        private readonly ILogger logger;

        public RegistryWorldController(
            TWorld world,
            IEntityPrototypeRepositoryWithWorld<TWorld, TPrototypeID, TEntity> prototypeRepository,
            ComponentCloner componentCloner,
            ILogger logger = null)
        {
            World = world;

            this.prototypeRepository = prototypeRepository;
            
            this.componentCloner = componentCloner;

            this.logger = logger;
        }

        #region IEntityWorldController

        public TWorld World { get; private set; }

        public bool TrySpawnEntity(
            out TEntity entity)
        {
            entity = World.CreateEntity();

            return true;
        }

        public bool TrySpawnAndResolveEntity(
            object source,
            out TEntity entity)
        {
            //There's no use in resolving in registry world (for now)
            return TrySpawnEntity(
                out entity);
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
                return false;

            entity.Set<DespawnComponent>();

            return true;
        }

        #endregion

        #region IEntityWorldControllerWithPrototypes

        public IEntityPrototypeRepositoryWithWorld<TWorld, TPrototypeID, TEntity> PrototypeRepository { get => prototypeRepository; }


        public bool TrySpawnEntityFromPrototype(
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
            TPrototypeID prototypeID,
            object source,
            out TEntity entity)
        {
            //There's no use in resolving in registry world (for now)
            return TrySpawnEntityFromPrototype(
                prototypeID,
                out entity);
        }

        public bool TrySpawnAndResolveEntityFromPrototype(
            TPrototypeID prototypeID,
            TEntity @override,
            object source,
            out TEntity entity)
        {
            //There's no use in resolving in registry world (for now)
            return TrySpawnEntityFromPrototype(
                prototypeID,
                @override,
                out entity);
        }

        #endregion

        #region IEntityWorldControllerWithPrototypesAndIDs

        public bool TrySpawnEntityWithIDFromPrototype(
            TPrototypeID prototypeID,
            TEntityID entityID,
            out TEntity entity)
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

            entity.Set<GUIDComponent>(
                new GUIDComponent
                {
                    GUID = entityID
                });

            return true;
        }
        
        public bool TrySpawnEntityWithIDFromPrototype(
            TPrototypeID prototypeID,
            TEntityID entityID,
            TEntity @override,
            out TEntity entity)
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

            entity.Set<GUIDComponent>(
                new GUIDComponent
                {
                    GUID = entityID
                });

            return true;
        }

        public bool TrySpawnAndResolveEntityWithIDFromPrototype(
            TPrototypeID prototypeID,
            TEntityID entityID,
            object source,
            out TEntity entity)
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

            entity.Set<GUIDComponent>(
                new GUIDComponent
                {
                    GUID = entityID
                });

            return true;
        }
        
        public bool TrySpawnAndResolveEntityWithIDFromPrototype(
            TPrototypeID prototypeID,
            TEntityID entityID,
            TEntity @override,
            object source,
            out TEntity entity)
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

            entity.Set<GUIDComponent>(
                new GUIDComponent
                {
                    GUID = entityID
                });

            return true;
        }

        #endregion
    }
}