using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Time;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.AllocationCallbacks
{
    public class SetDurationAndPushSubscriptionCallback<T>
        : IAllocationCallback<IPoolElementFacade<T>>
    {
        public float Duration { get; set; }

        private ILoggerResolver loggerResolver;

        private ILogger logger;

        public SetDurationAndPushSubscriptionCallback(
            ILoggerResolver loggerResolver,
            ILogger logger,
            float duration = 0f)
        {
            Duration = duration;

            this.loggerResolver = loggerResolver;

            this.logger = logger;
        }

        public void OnAllocated(IPoolElementFacade<T> poolElementFacade)
        {
            IPoolElementFacadeWithMetadata<T> facadeWithMetadata =
                poolElementFacade as IPoolElementFacadeWithMetadata<T>;

            if (facadeWithMetadata == null)
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "POOL ELEMENT FACADE HAS NO METADATA"));
            }
            
            var metadata = (RuntimeTimerWithPushableSubscriptionMetadata)
                facadeWithMetadata.Metadata.Get<IContainsRuntimeTimer>();

            if (metadata == null)
            {
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        "POOL ELEMENT FACADE HAS NO TIMER METADATA"));
            }
            
            metadata.Duration = Duration;

            Action<IRuntimeTimer> pushDelegate = (timer) =>
            {
                poolElementFacade.Push();
            };

            var pushSubscription = DelegateWrapperFactory.BuildSubscriptionSingleArgGeneric<IRuntimeTimer>(
                pushDelegate,
                loggerResolver);

            metadata.PushSubscription = pushSubscription;
        }
    }
}