using System;

using HereticalSolutions.Entities;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
    public class ErasePreviousScanResultsOnSensorScanPerformedEventSystem : AEntitySetSystem<float>
    {
        private readonly DefaultECSEntityListManager entityListManager;
        
        private readonly ILogger logger;
        
        public ErasePreviousScanResultsOnSensorScanPerformedEventSystem(
            World world,
            DefaultECSEntityListManager entityListManager,
            ILogger logger = null)
            : base(
                world
                    .GetEntities()
                    .With<SensorScanPerformedEventComponent>()
                    .With<EventSourceWorldLocalEntityComponent<Entity>>()
                    .Without<EventProcessedComponent>()
                    .AsSet())
        {
            this.entityListManager = entityListManager;

            this.logger = logger;
        }

        protected override void Update(
            float deltaTime,
            in Entity entity)
        {
            var eventSourceEntityComponent = entity.Get<EventSourceWorldLocalEntityComponent<Entity>>();

            var sensorEntity = eventSourceEntityComponent.Source;
            
            var sensorComponent = sensorEntity.Get<SensorComponent>();
            
            
            var scanResults =  entityListManager.Get(
                sensorComponent.EntityListHandle);

            if (scanResults == null)
            {
                return;
            }
            
            scanResults.Clear();
        }
    }
}