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
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<SubscriptionNoArgs>();

            return new SubscriptionNoArgs(
                @delegate,
                logger);
        }
        
        public static SubscriptionSingleArgGeneric<TValue> BuildSubscriptionSingleArgGeneric<TValue>(
            Action<TValue> @delegate,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<SubscriptionSingleArgGeneric<TValue>>();

            IInvokableSingleArgGeneric<TValue> invokable = DelegateWrapperFactory.BuildDelegateWrapperSingleArgGeneric(
                @delegate,
                loggerResolver);

            return new SubscriptionSingleArgGeneric<TValue>(
                invokable,
                logger);
        }
        
        public static SubscriptionMultipleArgs BuildSubscriptionMultipleArgs(
            Action<object[]> @delegate,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<SubscriptionMultipleArgs>();

            return new SubscriptionMultipleArgs(
                @delegate,
                logger);
        }

        #endregion

        #region Concurrent subscriptions

        public static ConcurrentSubscriptionNoArgs BuildConcurrentSubscriptionNoArgs(
            Action @delegate,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ConcurrentSubscriptionNoArgs>();

            return new ConcurrentSubscriptionNoArgs(
                @delegate,
                logger);
        }

        public static ConcurrentSubscriptionSingleArgGeneric<TValue> BuildConcurrentSubscriptionSingleArgGeneric<TValue>(
            Action<TValue> @delegate,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ConcurrentSubscriptionSingleArgGeneric<TValue>>();

            IInvokableSingleArgGeneric<TValue> invokable = DelegateWrapperFactory.BuildDelegateWrapperSingleArgGeneric(
                @delegate,
                loggerResolver);

            return new ConcurrentSubscriptionSingleArgGeneric<TValue>(
                invokable,
                logger);
        }

        public static ConcurrentSubscriptionMultipleArgs BuildConcurrentSubscriptionMultipleArgs(
            Action<object[]> @delegate,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ConcurrentSubscriptionMultipleArgs>();

            return new ConcurrentSubscriptionMultipleArgs(
                @delegate,
                logger);
        }

        #endregion
    }
}