using System;

using HereticalSolutions.Delegates.Subscriptions;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Time;

namespace HereticalSolutions.Pools
{
    public class RuntimeTimerWithPushSubscriptionMetadata
        : IContainsRuntimeTimer,
          IPushOnTimerFinish,
          ICleanUppable,
          IDisposable
    {
        public RuntimeTimerWithPushSubscriptionMetadata()
        {
            RuntimeTimer = null;

            PushSubscription = null;

            Duration = 0f;

            TimerID = -1;
        }

        #region IContainsRuntimeTimer

        public IRuntimeTimer RuntimeTimer { get; set; }

        #endregion

        #region IPushOnTimerFinish

        public float Duration { get; set; }

        public SubscriptionSingleArgGeneric<IRuntimeTimer> PushSubscription { get; set; }

        public int TimerID { get; set; }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (RuntimeTimer is ICleanUppable)
                (RuntimeTimer as ICleanUppable).Cleanup();

            if (PushSubscription is ICleanUppable)
                (PushSubscription as ICleanUppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (RuntimeTimer is IDisposable)
                (RuntimeTimer as IDisposable).Dispose();

            if (PushSubscription is IDisposable)
                (PushSubscription as IDisposable).Dispose();
        }

        #endregion
    }
}