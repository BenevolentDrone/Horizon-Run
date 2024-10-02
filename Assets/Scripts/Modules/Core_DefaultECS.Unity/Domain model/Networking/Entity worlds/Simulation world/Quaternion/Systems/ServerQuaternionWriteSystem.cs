using HereticalSolutions.Logging;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    public class ServerQuaternionWriteSystem : ISystem<float>
    {
        private readonly EntitySet serverQuaternionEntitiesSet;
        
        public bool IsEnabled { get; set; } = true;
        
        public ServerQuaternionWriteSystem(
            World world,
            ILogger logger = null)
        {
            serverQuaternionEntitiesSet = world
                .GetEntities()
                .With<QuaternionComponent>()
                .With<ServerQuaternionComponent>()
                .AsSet();
        }

        public void Update(
            float deltaTime)
        {
            foreach (Entity entity in serverQuaternionEntitiesSet.GetEntities())
            {
                ref var quaternionComponent = ref entity.Get<QuaternionComponent>();

                ref var serverQuaternionComponent = ref entity.Get<ServerQuaternionComponent>();

                if (serverQuaternionComponent.Dirty)
                {
                    quaternionComponent.Quaternion = serverQuaternionComponent.ServerQuaternion;
                    
                    serverQuaternionComponent.Dirty = false;
                }
            }
        }
        
        public void Dispose()
        {
        }
    }
}