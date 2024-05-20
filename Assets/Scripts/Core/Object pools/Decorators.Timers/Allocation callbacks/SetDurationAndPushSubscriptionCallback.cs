using System;

using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Synchronization;

using HereticalSolutions.Time;
using HereticalSolutions.Time.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.AllocationCallbacks
{
    public class SetDurationAndPushSubscriptionCallback<T>
        : IAllocationCallback<T>
    {
        public float Duration { get; set; }

        private ILoggerResolver loggerResolver;

        public SetDurationAndPushSubscriptionCallback(
            float duration = 0f,
            ILoggerResolver loggerResolver = null)
        {
            Duration = duration;

            this.loggerResolver = loggerResolver;
        }

        public void OnAllocated(IPoolElement<T> currentElement)
        {
            var metadata = (RuntimeTimerWithPushSubscriptionMetadata)
                currentElement.Metadata.Get<IContainsRuntimeTimer>();


            metadata.Duration = Duration;


            Action<IRuntimeTimer> pushDelegate = (timer) =>
            {
                currentElement.Push();
            };

            var pushSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<IRuntimeTimer>(
                pushDelegate,
                loggerResolver);

            metadata.PushSubscription = pushSubscription;
        }
    }
}