using System;

using HereticalSolutions.Time;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
    public class SensorWithTimerSystem : AEntitySetSystem<float>
    {
        private readonly ITimerManager simulationTimerManager;

        private readonly ILogger logger;

        public SensorWithTimerSystem(
            World world,
            ITimerManager simulationTimerManager,
            ILogger logger)
            : base(
                world
                    .GetEntities()
                    .With<SensorWithTimerComponent>()
                    .With<LoopedTimerComponent>()
                    .AsSet())
        {
            this.simulationTimerManager = simulationTimerManager;

            this.logger = logger;
        }
        
        protected override void Update(
            float deltaTime,
            in Entity entity)
        {
            var sensorWithTimerComponent = entity.Get<SensorWithTimerComponent>();

            var loopedTimerComponent = entity.Get<LoopedTimerComponent>();


            if (!simulationTimerManager.TryGetTimer(
                loopedTimerComponent.TimerHandle,
                out var timer))
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"FAILED TO GET TIMER WITH ID {loopedTimerComponent.TimerHandle}"));
            }

            if (!sensorWithTimerComponent.Active
                && timer.State == ETimerState.STARTED)
            {
                timer.Reset();
            }

            if (sensorWithTimerComponent.Active
                && timer.State != ETimerState.STARTED)
            {
                timer.Reset();

                timer.Start(
                    loopedTimerComponent.Timeout);
            }
        }

    }
}