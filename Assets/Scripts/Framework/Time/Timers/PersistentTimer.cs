using System;

using HereticalSolutions.Delegates;

using HereticalSolutions.Persistence;

using HereticalSolutions.Repositories;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Time
{
    public class PersistentTimer
        : ITimer,
          IPersistentTimer,
          IPersistentTimerContext,
          ITimerWithState,
          IRenameableTimer,
          ITickable,
          IVisitable,
          ICleanuppable,
          IDisposable
    {
        private readonly IReadOnlyRepository<ETimerState, ITimerStrategy<IPersistentTimerContext, TimeSpan>> strategyRepository;

        private readonly ILogger logger;

        private ITimerStrategy<IPersistentTimerContext, TimeSpan> currentStrategy;

        public PersistentTimer(
            string id,
            TimeSpan defaultDurationSpan,

            IPublisherSingleArgGeneric<IPersistentTimer> onStartAsPublisher,
            INonAllocSubscribable onStartAsSubscribable,

            IPublisherSingleArgGeneric<IPersistentTimer> onStartRepeatedAsPublisher,
            INonAllocSubscribable onStartRepeatedAsSubscribable,
            
            IPublisherSingleArgGeneric<IPersistentTimer> onFinishAsPublisher,
            INonAllocSubscribable onFinishAsSubscribable,

            IPublisherSingleArgGeneric<IPersistentTimer> onFinishRepeatedAsPublisher,
            INonAllocSubscribable onFinishRepeatedAsSubscribable,
            
            IReadOnlyRepository<ETimerState, ITimerStrategy<IPersistentTimerContext, TimeSpan>> strategyRepository,

            ILogger logger)
        {
            ID = id;

            StartTime = default(DateTime);

            EstimatedFinishTime = default(DateTime);

            SavedProgress = default(TimeSpan);

            CurrentDurationSpan = DefaultDurationSpan = defaultDurationSpan;


            OnStartAsPublisher = onStartAsPublisher;

            OnStart = onStartAsSubscribable;

            
            OnStartRepeatedAsPublisher = onStartAsPublisher;

            OnStartRepeated = onStartAsSubscribable;
            

            OnFinishAsPublisher = onFinishAsPublisher;

            OnFinish = onFinishAsSubscribable;

            
            OnFinishRepeatedAsPublisher = onFinishAsPublisher;

            OnFinishRepeated = onFinishAsSubscribable;

            
            this.strategyRepository = strategyRepository;

            this.logger = logger;

            
            SetState(ETimerState.INACTIVE);
        }

        #region IPersistentTimerContext

        #region Variables

        /// <summary>
        /// Gets or sets the start time of the timer
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets the estimated finish time of the timer
        /// </summary>
        public DateTime EstimatedFinishTime { get; set; }

        /// <summary>
        /// Gets or sets the saved progress of the timer
        /// </summary>
        public TimeSpan SavedProgress { get; set; }

        #endregion

        #region Duration

        /// <summary>
        /// Gets or sets the current duration span of the timer
        /// </summary>
        public TimeSpan CurrentDurationSpan { get; set; }

        /// <summary>
        /// Gets or sets the default duration span of the timer
        /// </summary>
        public TimeSpan DefaultDurationSpan { get; set; }

        #endregion

        #region Publishers

        /// <summary>
        /// Gets the publisher for the "on start" event of the timer
        /// </summary>
        public IPublisherSingleArgGeneric<IPersistentTimer> OnStartAsPublisher { get; private set; }
        
        public IPublisherSingleArgGeneric<IPersistentTimer> OnStartRepeatedAsPublisher { get; private set; }

        /// <summary>
        /// Gets the subscribable for the "on finish" event of the timer
        /// </summary>
        public IPublisherSingleArgGeneric<IPersistentTimer> OnFinishAsPublisher { get; private set; }
        
        public IPublisherSingleArgGeneric<IPersistentTimer> OnFinishRepeatedAsPublisher { get; private set; }

        #endregion

        #endregion

        #region ITimer

        #region IRenameableTimer

        /// <summary>
        /// Gets the ID of the timer
        /// </summary>
        public string ID { get; set; }

        #endregion 

        #region Timer state

        /// <summary>
        /// Gets the state of the timer
        /// </summary>
        public ETimerState State { get; private set; }

        #endregion

        #region Progress

        /// <summary>
        /// Gets the progress of the timer
        /// </summary>
        public float Progress
        {
            get => currentStrategy.GetProgress(this);
        }

        #endregion

        #region Controls

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

        #endregion

        #region IPersistentTimer

        #region Countdown and Time elapsed

        /// <summary>
        /// Gets the time elapsed span of the timer
        /// </summary>
        public TimeSpan TimeElapsedSpan
        {
            get => currentStrategy.GetTimeElapsed(this);
        }

        /// <summary>
        /// Gets the countdown span of the timer
        /// </summary>
        public TimeSpan CountdownSpan
        {
            get => currentStrategy.GetCountdown(this);
        }

        #endregion

        #region Controls

        /// <summary>
        /// Resets the timer with a specified duration span
        /// </summary>
        /// <param name="durationSpan">The duration span to set for the timer.</param>
        public void Reset(TimeSpan durationSpan)
        {
            Reset();

            CurrentDurationSpan = durationSpan;
        }

        /// <summary>
        /// Starts the timer with a specified duration span
        /// </summary>
        /// <param name="durationSpan">The duration span to set for the timer.</param>
        public void Start(TimeSpan durationSpan)
        {
            CurrentDurationSpan = durationSpan;

            Start();
        }

        #endregion

        #region Callbacks

        /// <summary>
        /// Gets the subscribable for the "on start" event of the timer
        /// </summary>
        public INonAllocSubscribable OnStart { get; private set; }
        
        public INonAllocSubscribable OnStartRepeated { get; private set; }

        /// <summary>
        /// Gets the subscribable for the "on finish" event of the timer
        /// </summary>
        public INonAllocSubscribable OnFinish { get; private set; }
        
        public INonAllocSubscribable OnFinishRepeated { get; private set; }

        #endregion

        #endregion

        #region ITickable

        /// <summary>
        /// Ticks the timer
        /// </summary>
        /// <param name="delta">The time difference.</param>
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

            // Strategies should exist for all states in enum, therefore get the corresponding strategy
            currentStrategy = strategyRepository.Get(state);
        }

        #endregion

        #region IVisitable

        public bool AcceptSave(
            ISaveVisitor visitor,
            ref object dto)
        {
            return visitor.VisitSave<PersistentTimer>(
                ref dto,
                this);
        }

        public bool AcceptPopulate(
            IPopulateVisitor visitor,
            object dto)
        {
            return visitor.VisitPopulate<PersistentTimer>(
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