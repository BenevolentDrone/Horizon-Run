using System;

using HereticalSolutions.Delegates;

using HereticalSolutions.Persistence;

using HereticalSolutions.Repositories;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Time
{
    public class RuntimeTimer
        : ITimer,
          IRuntimeTimer,
          IRuntimeTimerContext,
          ITimerWithState,
          IRenameableTimer,
          ITickable,
          IVisitable,
          ICleanuppable,
          IDisposable
    {
        private readonly IReadOnlyRepository<ETimerState, ITimerStrategy<IRuntimeTimerContext, float>> strategyRepository;

        private readonly ILogger logger;

        private ITimerStrategy<IRuntimeTimerContext, float> currentStrategy;
        

        public RuntimeTimer(
            string id,
            float defaultDuration,

            IPublisherSingleArgGeneric<IRuntimeTimer> onStartAsPublisher,
            INonAllocSubscribable onStartAsSubscribable,

            IPublisherSingleArgGeneric<IRuntimeTimer> onStartRepeatedAsPublisher,
            INonAllocSubscribable onStartRepeatedAsSubscribable,
            
            IPublisherSingleArgGeneric<IRuntimeTimer> onFinishAsPublisher,
            INonAllocSubscribable onFinishAsSubscribable,
            
            IPublisherSingleArgGeneric<IRuntimeTimer> onFinishRepeatedAsPublisher,
            INonAllocSubscribable onFinishRepeatedAsSubscribable,

            IReadOnlyRepository<ETimerState, ITimerStrategy<IRuntimeTimerContext, float>> strategyRepository,

            ILogger logger = null)
        {
            ID = id;

            CurrentTimeElapsed = 0f;

            CurrentDuration = DefaultDuration = defaultDuration;


            OnStartAsPublisher = onStartAsPublisher;

            OnStart = onStartAsSubscribable;
            
            
            OnStartRepeatedAsPublisher = onStartRepeatedAsPublisher;
            
            OnStartRepeated = onStartRepeatedAsSubscribable;

            
            OnFinishAsPublisher = onFinishAsPublisher;

            OnFinish = onFinishAsSubscribable;
            
            
            OnFinishRepeatedAsPublisher = onFinishRepeatedAsPublisher;
            
            OnFinishRepeated = onFinishRepeatedAsSubscribable;


            this.strategyRepository = strategyRepository;

            this.logger = logger;

            SetState(ETimerState.INACTIVE);
        }

        #region IRuntimeTimerContext

        /// <summary>
        /// Gets or sets the current time elapsed
        /// </summary>
        public float CurrentTimeElapsed { get; set; }

        /// <summary>
        /// Gets or sets the current duration of the timer
        /// </summary>
        public float CurrentDuration { get; set; }

        /// <summary>
        /// Gets the default duration of the timer
        /// </summary>
        public float DefaultDuration { get; private set; }

        /// <summary>
        /// Gets the publisher for the OnStart event
        /// </summary>
        public IPublisherSingleArgGeneric<IRuntimeTimer> OnStartAsPublisher { get; private set; }
        
        public IPublisherSingleArgGeneric<IRuntimeTimer> OnStartRepeatedAsPublisher { get; private set; }

        /// <summary>
        /// Gets the publisher for the OnFinish event
        /// </summary>
        public IPublisherSingleArgGeneric<IRuntimeTimer> OnFinishAsPublisher { get; private set; }
        
        public IPublisherSingleArgGeneric<IRuntimeTimer> OnFinishRepeatedAsPublisher { get; private set; }

        #endregion

        #region ITimer

        #region IRenameableTimer

        /// <summary>
        /// Gets the ID of the timer
        /// </summary>
        public string ID { get; set; }

        #endregion

        /// <summary>
        /// Gets the state of the timer
        /// </summary>
        public ETimerState State { get; private set; }

        /// <summary>
        /// Gets the progress of the timer
        /// </summary>
        public float Progress => currentStrategy.GetProgress(this);

        /// <summary>
        /// Gets or sets a value indicating whether the timer should accumulate time when paused
        /// </summary>
        public bool Accumulate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the timer should repeat after finishing
        /// </summary>
        public bool Repeat { get; set; }

        public bool FlushTimeElapsedOnRepeat { get; set; }

        public bool FireRepeatCallbackOnFinish { get; set; }
        
        /// <summary>
        /// Resets the timer
        /// </summary>
        public void Reset()
        {
            currentStrategy.Reset(this);
        }

        /// <summary>
        /// Starts the timer
        /// </summary>
        public void Start()
        {
            currentStrategy.Start(this);
        }

        /// <summary>
        /// Pauses the timer
        /// </summary>
        public void Pause()
        {
            currentStrategy.Pause(this);
        }

        /// <summary>
        /// Resumes the timer
        /// </summary>
        public void Resume()
        {
            currentStrategy.Resume(this);
        }

        /// <summary>
        /// Aborts the timer
        /// </summary>
        public void Abort()
        {
            currentStrategy.Abort(this);
        }

        /// <summary>
        /// Finishes the timer
        /// </summary>
        public void Finish()
        {
            currentStrategy.Finish(this);
        }

        #endregion

        #region IRuntimeTimer

        /// <summary>
        /// Gets the time elapsed since the timer started
        /// </summary>
        public float TimeElapsed => currentStrategy.GetTimeElapsed(this);

        /// <summary>
        /// Gets the remaining time until the timer finishes
        /// </summary>
        public float Countdown => currentStrategy.GetCountdown(this);

        /// <summary>
        /// Resets the timer with the specified duration
        /// </summary>
        /// <param name="duration">The new duration of the timer.</param>
        public void Reset(float duration)
        {
            Reset();
            CurrentDuration = duration;
        }

        /// <summary>
        /// Starts the timer with the specified duration
        /// </summary>
        /// <param name="duration">The duration of the timer.</param>
        public void Start(float duration)
        {
            CurrentDuration = duration;
            Start();
        }

        /// <summary>
        /// Resumes the timer with the specified duration
        /// </summary>
        /// <param name="duration">The duration to set for the timer.</param>
        public void Resume(float duration)
        {
            CurrentDuration = duration;
            Resume();
        }

        /// <summary>
        /// Gets the subscribable for the OnStart event
        /// </summary>
        public INonAllocSubscribable OnStart { get; private set; }

        public INonAllocSubscribable OnStartRepeated { get; private set; }
        
        /// <summary>
        /// Gets the subscribable for the OnFinish event
        /// </summary>
        public INonAllocSubscribable OnFinish { get; private set; }

        public INonAllocSubscribable OnFinishRepeated { get; private set; }
        
        #endregion

        #region ITickable

        /// <summary>
        /// Performs a tick on the timer
        /// </summary>
        /// <param name="delta">The time since the last tick.</param>
        public void Tick(float delta)
        {
            currentStrategy.Tick(this, delta);
        }

        #endregion

        #region ITimerWithState

        /// <summary>
        /// Sets the state of the timer
        /// </summary>
        /// <param name="state">The state to set.</param>
        public void SetState(ETimerState state)
        {
            State = state;
            // Strategies should exist for all states in enum therefore Get
            currentStrategy = strategyRepository.Get(state);
        }

        #endregion

        #region IVisitable

        public bool AcceptSave(
            ISaveVisitor visitor,
            ref object dto)
        {
            return visitor.VisitSave<RuntimeTimer>(
                ref dto,
                this);
        }

        public bool AcceptPopulate(
            IPopulateVisitor visitor,
            object dto)
        {
            return visitor.VisitPopulate<RuntimeTimer>(
                dto,
                this);
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (OnStartAsPublisher is ICleanuppable)
                (OnStartAsPublisher as ICleanuppable).Cleanup();

            if (OnStartRepeatedAsPublisher is ICleanuppable)
                (OnStartRepeatedAsPublisher as ICleanuppable).Cleanup();
            
            if (OnFinishAsPublisher is ICleanuppable)
                (OnFinishAsPublisher as ICleanuppable).Cleanup();
            
            if (OnFinishRepeatedAsPublisher is ICleanuppable)
                (OnFinishRepeatedAsPublisher as ICleanuppable).Cleanup();

            if (strategyRepository is ICleanuppable)
                (strategyRepository as ICleanuppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (OnStartAsPublisher is IDisposable)
                (OnStartAsPublisher as IDisposable).Dispose();
            
            if (OnStartRepeatedAsPublisher is IDisposable)
                (OnStartRepeatedAsPublisher as IDisposable).Dispose();

            if (OnFinishAsPublisher is IDisposable)
                (OnFinishAsPublisher as IDisposable).Dispose();

            if (OnFinishRepeatedAsPublisher is IDisposable)
                (OnFinishRepeatedAsPublisher as IDisposable).Dispose();
            
            if (strategyRepository is IDisposable)
                (strategyRepository as IDisposable).Dispose();
        }

        #endregion
    }
}