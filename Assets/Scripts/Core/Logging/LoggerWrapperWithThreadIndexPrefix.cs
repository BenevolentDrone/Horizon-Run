using System;
using System.Threading;

namespace HereticalSolutions.Logging
{
    public class LoggerWrapperWithThreadIndexPrefix
        : ILogger,
          ILoggerWrapper
    {
        private readonly ILogger innerLogger;

        public LoggerWrapperWithThreadIndexPrefix(
            ILogger innerLogger)
        {
            this.innerLogger = innerLogger;
        }

        #region ILoggerWrapper

        public ILogger InnerLogger { get => innerLogger; }

        #endregion

        #region ILogger

        #region Log

        public void Log(
            string value)
        {
            value = FormatLogWithThreadIndex(
                value);
            
            innerLogger.Log(value);
        }

        public void Log<TSource>(
            string value)
        {
            value = FormatLogWithThreadIndex(
                value);

            innerLogger.Log<TSource>(value);
        }

        public void Log(
            Type logSource,
            string value)
        {
            value = FormatLogWithThreadIndex(
                value);

            innerLogger.Log(
                logSource,
                value);
        }

        public void Log(
            string value,
            object[] arguments)
        {
            value = FormatLogWithThreadIndex(
                value);
            
            innerLogger.Log(
                value,
                arguments);
        }

        public void Log<TSource>(
            string value,
            object[] arguments)
        {
            value = FormatLogWithThreadIndex(
                value);

            innerLogger.Log<TSource>(
                value,
                arguments);
        }

        public void Log(
            Type logSource,
            string value,
            object[] arguments)
        {
            value = FormatLogWithThreadIndex(
                value);

            innerLogger.Log(
                logSource,
                value,
                arguments);
        }

        #endregion

        #region Warning

        public void LogWarning(
            string value)
        {
            value = FormatLogWithThreadIndex(
                value);
            
            innerLogger.LogWarning(
                value);
        }

        public void LogWarning<TSource>(
            string value)
        {
            value = FormatLogWithThreadIndex(
                value);

            innerLogger.LogWarning<TSource>(value);
        }

        public void LogWarning(
            Type logSource,
            string value)
        {
            value = FormatLogWithThreadIndex(
                value);

            innerLogger.LogWarning(
                logSource,
                value);
        }

        public void LogWarning(
            string value,
            object[] arguments)
        {
            value = FormatLogWithThreadIndex(
                value);
            
            innerLogger.LogWarning(
                value,
                arguments);
        }

        public void LogWarning<TSource>(
            string value,
            object[] arguments)
        {
            value = FormatLogWithThreadIndex(
                value);

            innerLogger.LogWarning<TSource>(
                value,
                arguments);
        }

        public void LogWarning(
            Type logSource,
            string value,
            object[] arguments)
        {
            value = FormatLogWithThreadIndex(
                value);

            innerLogger.LogWarning(
                logSource,
                value,
                arguments);
        }

        #endregion

        #region Error

        public void LogError(
            string value)
        {
            value = FormatLogWithThreadIndex(
                value);
            
            innerLogger.LogError(
                value);
        }

        public void LogError<TSource>(
            string value)
        {
            value = FormatLogWithThreadIndex(
                value);

            innerLogger.LogError<TSource>(value);
        }

        public void LogError(
            Type logSource,
            string value)
        {
            value = FormatLogWithThreadIndex(
                value);

            innerLogger.LogError(
                logSource,
                value);
        }

        public void LogError(
            string value,
            object[] arguments)
        {
            value = FormatLogWithThreadIndex(
                value);
            
            innerLogger.LogError(
                value,
                arguments);
        }

        public void LogError<TSource>(
            string value,
            object[] arguments)
        {
            value = FormatLogWithThreadIndex(
                value);

            innerLogger.LogError<TSource>(
                value,
                arguments);
        }

        public void LogError(
            Type logSource,
            string value,
            object[] arguments)
        {
            value = FormatLogWithThreadIndex(
                value);

            innerLogger.LogError(
                logSource,
                value,
                arguments);
        }

        #endregion

        #region Exception

        public string FormatException(
            string value)
        {
            value = FormatLogWithThreadIndex(
                value);
            
            return innerLogger.FormatException(value);
        }

        public string FormatException<TSource>(
            string value)
        {
            value = FormatLogWithThreadIndex(
                value);

            return innerLogger.FormatException<TSource>(value);
        }

        public string FormatException(
            Type logSource,
            string value)
        {
            value = FormatLogWithThreadIndex(
                value);

            return innerLogger.FormatException(
                logSource,
                value);
        }

        #endregion

        #endregion

        private string FormatLogWithThreadIndex(
            string value)
        {
            return $"[THREAD {Thread.CurrentThread.ManagedThreadId}] {value}";
        }
    }
}