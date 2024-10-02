using System;

using HereticalSolutions.Delegates.Subscriptions;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Time;

namespace HereticalSolutions.Pools
{
    public class RuntimeTimerWithPushableSubscriptionMetadata
        : IContainsRuntimeTimer,
          IPushableOnTimerFinish,
          ICleanuppable,
          IDisposable
    {
        public RuntimeTimerWithPushableSubscriptionMetadata()
        {
            RuntimeTimer = null;

            PushSubscription = null;

            Duration = 0f;

            TimerHandle = 0;
        }

        #region IContainsRuntimeTimer

        public IRuntimeTimer RuntimeTimer { get; set; }

        #endregion

        #region IPushOnTimerFinish

        public float Duration { get; set; }

        public SubscriptionSingleArgGeneric<IRuntimeTimer> PushSubscription { get; set; }

        public ushort TimerHandle { get; set; }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (RuntimeTimer is ICleanuppable)
                (RuntimeTimer as ICleanuppable).Cleanup();

            if (PushSubscription is ICleanuppable)
                (PushSubscription as ICleanuppable).Cleanup();
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