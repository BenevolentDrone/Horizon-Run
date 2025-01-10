using System;

using HereticalSolutions.Entities;

using HereticalSolutions.Time;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	public class SpawnerWithLoopedTimerInitializationSystem
		: IEntityInitializationSystem
	{
		private readonly ITimerManager simulationTimerManager;

		private readonly IEventEntityBuilder<Entity, Guid> eventEntityBuilder;

		private readonly ILoggerResolver loggerResolver;

		private readonly ILogger logger;

		public SpawnerWithLoopedTimerInitializationSystem(
			ITimerManager simulationTimerManager,
			IEventEntityBuilder<Entity, Guid> eventEntityBuilder,
			ILoggerResolver loggerResolver = null,
			ILogger logger = null)
		{
			this.simulationTimerManager = simulationTimerManager;

			this.eventEntityBuilder = eventEntityBuilder;

			this.loggerResolver = loggerResolver;

			this.logger = logger;
		}

		//Required by ISystem
		public bool IsEnabled { get; set; } = true;

		public void Update(Entity entity)
		{
			if (!IsEnabled)
				return;

			if (!entity.Has<SpawnerComponent>())
				return;

			if (!entity.Has<LoopedTimerComponent>())
				return;

			ref var loopedTimerComponent = ref entity.Get<LoopedTimerComponent>();


			//Create a timer
			if (!simulationTimerManager.CreateTimer(
				out var timerHandle,
				out var timer))
			{
				throw new Exception(
					logger.TryFormatException<SpawnerWithLoopedTimerInitializationSystem>(
						$"ERROR CREATING TIMER FROM TIME MANAGER {simulationTimerManager.ID}"));
			}

			loopedTimerComponent.TimerHandle = timerHandle;

			timer.Reset();

			timer.Repeat = true;

			timer.FlushTimeElapsedOnRepeat = false;

			Entity entityClosure = entity;

			INonAllocSubscription timerTickSubscription = DelegateWrapperFactory.BuildSubscriptionSingleArgGeneric<IRuntimeTimer>(
				(timerArg) => OnTick(entityClosure),
				loggerResolver);

			timer.OnFinish.Subscribe(
				(INonAllocSubscriptionHandler<
					INonAllocSubscribable,
					IInvokableSingleArgGeneric<IRuntimeTimer>>)
					timerTickSubscription);

			timer.Start(
				loopedTimerComponent.Timeout);
		}

		private void OnTick(Entity spawnerEntity)
		{
			eventEntityBuilder
				.NewEvent(out var eventEntity)
				.AddressedToWorldLocalEntity(
					eventEntity,
					spawnerEntity)
				.WithData<SpawnerEmittedEventComponent>(
					eventEntity,
					new SpawnerEmittedEventComponent());
		}

		public void Dispose()
		{
		}
	}
}