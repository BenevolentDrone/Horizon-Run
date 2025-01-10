using System;

using HereticalSolutions.Delegates.Wrappers;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Factories
{
    public static class DelegateWrapperFactory
    {
        #region Delegate wrappers

        public static DelegateWrapperNoArgs BuildDelegateWrapperNoArgs(
            Action @delegate)
        {
            return new DelegateWrapperNoArgs(@delegate);
        }
        
        public static IInvokableSingleArg BuildDelegateWrapperSingleArg<TValue>(
            Action<TValue> @delegate,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<DelegateWrapperSingleArgGeneric<TValue>>()
                ?? null;

            return new DelegateWrapperSingleArgGeneric<TValue>(
                @delegate,
                logger);
        }
        
        public static DelegateWrapperSingleArgGeneric<TValue> BuildDelegateWrapperSingleArgGeneric<TValue>(
            Action<TValue> @delegate,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<DelegateWrapperSingleArgGeneric<TValue>>()
                ?? null;

            return new DelegateWrapperSingleArgGeneric<TValue>(
                @delegate,
                logger);
        }
        
        public static DelegateWrapperMultipleArgs BuildDelegateWrapperMultipleArgs(
            Action<object[]> @delegate)
        {
            return new DelegateWrapperMultipleArgs(@delegate);
        }

        #endregion
    }
}