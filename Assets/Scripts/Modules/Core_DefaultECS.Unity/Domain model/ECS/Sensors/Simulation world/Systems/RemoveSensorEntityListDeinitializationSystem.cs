using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
    public class RemoveSensorEntityListDeinitializationSystem
        : IEntityInitializationSystem
    {
        private readonly DefaultECSEntityListManager entityListManager;

        public RemoveSensorEntityListDeinitializationSystem(
            DefaultECSEntityListManager entityListManager)
        {
            this.entityListManager = entityListManager;
        }

        //Required by ISystem
        public bool IsEnabled { get; set; } = true;

        public void Update(Entity entity)
        {
            if (!IsEnabled)
                return;

            if (!entity.Has<SensorComponent>())
                return;

            ref var sensorComponent = ref entity.Get<SensorComponent>();
            
            if (sensorComponent.EntityListHandle == 0)
                return;

            entityListManager.Remove(
                sensorComponent.EntityListHandle);
        }

        public void Dispose()
        {
        }
    }
}