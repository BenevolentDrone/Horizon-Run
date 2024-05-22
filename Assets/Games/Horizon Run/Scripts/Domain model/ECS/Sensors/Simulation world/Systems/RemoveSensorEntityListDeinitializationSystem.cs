using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.HorizonRun
{
    public class RemoveSensorEntityListDeinitializationSystem
        : IDefaultECSEntityInitializationSystem
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

            entityListManager.RemoveList(
                sensorComponent.EntityListHandle);
        }

        public void Dispose()
        {
        }
    }
}