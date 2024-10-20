using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
    /*
    public class CreateSensorEntityListInitializationSystem
        : IEntityInitializationSystem
    {
        private readonly DefaultECSEntityListManager entityListManager;

        public CreateSensorEntityListInitializationSystem(
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

            entityListManager.TryAllocate(
                out var listHandle,
                out var entityList);

            sensorComponent.EntityListHandle = listHandle;
        }

        public void Dispose()
        {
        }
    }
    */
}