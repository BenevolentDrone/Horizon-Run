using System;

using HereticalSolutions.Entities;

using HereticalSolutions.Networking.ECS;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Networking
{
    public class AuthoringPermissionInitializationSystem
        : IEntityInitializationSystem
    {
        private readonly EntityManager entityManager;
        public AuthoringPermissionInitializationSystem(
            EntityManager entityManager)
        {
            this.entityManager = entityManager;
        }

        //Required by ISystem
        public bool IsEnabled { get; set; } = true;

        public void Update(Entity entity)
        {
            if (!IsEnabled)
                return;

            if (!entity.Has<GUIDComponent>())
                return;
            
            var entityID = entity.Get<GUIDComponent>().GUID;


            var registryEntity = entityManager.GetRegistryEntity(
                entityID);

            if (!registryEntity.Has<AuthoringPermissionComponent>())
            {
                return;
            }

            ref var authoringPermissionComponent = ref registryEntity.Get<AuthoringPermissionComponent>();

            authoringPermissionComponent.PlayerSlot = byte.MaxValue;
        }

        public void Dispose()
        {
        }
    }
}