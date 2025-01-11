using System;

using HereticalSolutions.Delegates.Subscriptions;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Factories
{
    public static class SubscriptionFactory
    {
        #region Subscriptions

        public static SubscriptionNoArgs BuildSubscriptionNoArgs(
            Action @delegate,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<SubscriptionNoArgs>();

            return new SubscriptionNoArgs(
                @delegate,
                logger);
        }
        
        public static SubscriptionSingleArgGeneric<TValue> BuildSubscriptionSingleArgGeneric<TValue>(
            Action<TValue> @delegate,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<SubscriptionSingleArgGeneric<TValue>>();

            return new SubscriptionSingleArgGeneric<TValue>(
                @delegate,
                logger);
        }
        
        public static SubscriptionMultipleArgs BuildSubscriptionMultipleArgs(
            Action<object[]> @delegate,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<SubscriptionMultipleArgs>();

            return new SubscriptionMultipleArgs(
                @delegate,
                logger);
        }

        #endregion
    }
}