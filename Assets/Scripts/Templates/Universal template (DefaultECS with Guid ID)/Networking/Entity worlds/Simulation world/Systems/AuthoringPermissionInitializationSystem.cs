using System;

using HereticalSolutions.Entities;

using HereticalSolutions.Networking.ECS;

using DefaultEcs;

namespace HereticalSolutions.Templates.Universal.Networking
{
    public class AuthoringPermissionInitializationSystem
        : IDefaultECSEntityInitializationSystem
    {
        private readonly UniversalTemplateEntityManager entityManager;
        public AuthoringPermissionInitializationSystem(
            UniversalTemplateEntityManager entityManager)
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