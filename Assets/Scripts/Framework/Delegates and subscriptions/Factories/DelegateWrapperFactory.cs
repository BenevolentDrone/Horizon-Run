using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

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
            return new DelegateWrapperNoArgs(
                @delegate);
        }
        
        public static DelegateWrapperSingleArgGeneric<TValue> BuildDelegateWrapperSingleArgGeneric<TValue>(
            Action<TValue> @delegate,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<DelegateWrapperSingleArgGeneric<TValue>>();

            return new DelegateWrapperSingleArgGeneric<TValue>(
                @delegate,
                logger);
        }
        
        public static DelegateWrapperMultipleArgs BuildDelegateWrapperMultipleArgs(
            Action<object[]> @delegate)
        {
            return new DelegateWrapperMultipleArgs(
                @delegate);
        }

        #endregion

        #region Concurrent delegate wrappers

        public static ConcurrentDelegateWrapperNoArgs BuildConcurrentDelegateWrapperNoArgs(
            Action @delegate)
        {
            return new ConcurrentDelegateWrapperNoArgs(
                @delegate);
        }

        public static ConcurrentDelegateWrapperSingleArgGeneric<TValue>
            BuildConcurrentDelegateWrapperSingleArgGeneric<TValue>(
            Action<TValue> @delegate,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<ConcurrentDelegateWrapperSingleArgGeneric<TValue>>();

            return new ConcurrentDelegateWrapperSingleArgGeneric<TValue>(
                @delegate,
                logger);
        }

        public static ConcurrentDelegateWrapperMultipleArgs BuildConcurrentDelegateWrapperMultipleArgs(
            Action<object[]> @delegate)
        {
            return new ConcurrentDelegateWrapperMultipleArgs(
                @delegate);
        }

        #endregion

        #region Task wrappers

        public static TaskWrapperNoArgs BuildTaskWrapperNoArgs(
            Func<AsyncExecutionContext, Task> taskFactory)
        {
            return new TaskWrapperNoArgs(
                taskFactory);
        }

        public static TaskWrapperSingleArgGeneric<TValue> BuildTaskWrapperSingleArgGeneric<TValue>(
            Func<TValue, AsyncExecutionContext, Task> taskFactory,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<TaskWrapperSingleArgGeneric<TValue>>();

            return new TaskWrapperSingleArgGeneric<TValue>(
                taskFactory,
                logger);
        }

        public static TaskWrapperMultipleArgs BuildTaskWrapperMultipleArgs(
            Func<object[], AsyncExecutionContext, Task> taskFactory)
        {
            return new TaskWrapperMultipleArgs(
                taskFactory);
        }

        #endregion
    }
}