using System;
using System.Threading;

namespace HereticalSolutions.Logging
{
    public class LoggerWrapperWithSemaphoreSlim
        : ILogger,
          ILoggerWrapper
    {
        private readonly SemaphoreSlim semaphore;
        
        private ILogger innerLogger;

        public LoggerWrapperWithSemaphoreSlim(
            SemaphoreSlim semaphore)
        {
            this.semaphore = semaphore;
            
            innerLogger = null;
        }

        #region ILoggerWrapper

        public ILogger InnerLogger
        {
            get => innerLogger;
            set => innerLogger = value;
        }

        #endregion

        #region ILogger

        #region Log

        public void Log(
            string value)
        {
            semaphore.Wait();
            
            innerLogger?.Log(value);

            semaphore.Release();
        }

        public void Log<TSource>(
            string value)
        {
            semaphore.Wait();

            innerLogger?.Log<TSource>(value);
            
            semaphore.Release();
        }

        public void Log(
            Type logSource,
            string value)
        {
            semaphore.Wait();

            innerLogger?.Log(
                logSource,
                value);
            
            semaphore.Release();
        }

        public void Log(
            string value,
            object[] arguments)
        {
            semaphore.Wait();
            
            innerLogger?.Log(
                value,
                arguments);
            
            semaphore.Release();
        }

        public void Log<TSource>(
            string value,
            object[] arguments)
        {
            semaphore.Wait();

            innerLogger?.Log<TSource>(
                value,
                arguments);
            
            semaphore.Release();
        }

        public void Log(
            Type logSource,
            string value,
            object[] arguments)
        {
            semaphore.Wait();

            innerLogger?.Log(
                logSource,
                value,
                arguments);
            
            semaphore.Release();
        }

        #endregion

        #region Warning

        public void LogWarning(
            string value)
        {
            semaphore.Wait();
            
            innerLogger?.LogWarning(
                value);
            
            semaphore.Release();
        }

        public void LogWarning<TSource>(
            string value)
        {
            semaphore.Wait();

            innerLogger?.LogWarning<TSource>(value);
            
            semaphore.Release();
        }

        public void LogWarning(
            Type logSource,
            string value)
        {
            semaphore.Wait();

            innerLogger?.LogWarning(
                logSource,
                value);
            
            semaphore.Release();
        }

        public void LogWarning(
            string value,
            object[] arguments)
        {
            semaphore.Wait();
            
            innerLogger?.LogWarning(
                value,
                arguments);
            
            semaphore.Release();
        }

        public void LogWarning<TSource>(
            string value,
            object[] arguments)
        {
            semaphore.Wait();

            innerLogger?.LogWarning<TSource>(
                value,
                arguments);
            
            semaphore.Release();
        }

        public void LogWarning(
            Type logSource,
            string value,
            object[] arguments)
        {
            semaphore.Wait();

            innerLogger?.LogWarning(
                logSource,
                value,
                arguments);
            
            semaphore.Release();
        }

        #endregion

        #region Error

        public void LogError(
            string value)
        {
            semaphore.Wait();
            
            innerLogger?.LogError(
                value);
            
            semaphore.Release();
        }

        public void LogError<TSource>(
            string value)
        {
            semaphore.Wait();

            innerLogger?.LogError<TSource>(value);
            
            semaphore.Release();
        }

        public void LogError(
            Type logSource,
            string value)
        {
            semaphore.Wait();

            innerLogger?.LogError(
                logSource,
                value);
            
            semaphore.Release();
        }

        public void LogError(
            string value,
            object[] arguments)
        {
            semaphore.Wait();
            
            innerLogger?.LogError(
                value,
                arguments);
            
            semaphore.Release();
        }

        public void LogError<TSource>(
            string value,
            object[] arguments)
        {
            semaphore.Wait();

            innerLogger?.LogError<TSource>(
                value,
                arguments);
            
            semaphore.Release();
        }

        public void LogError(
            Type logSource,
            string value,
            object[] arguments)
        {
            semaphore.Wait();

            innerLogger?.LogError(
                logSource,
                value,
                arguments);
            
            semaphore.Release();
        }

        #endregion

        #region Exception

        public string FormatException(
            string value)
        {
            semaphore.Wait();
            
            var result = innerLogger.FormatException(value);
            
            semaphore.Release();
            
            return result;
        }

        public string FormatException<TSource>(
            string value)
        {
            semaphore.Wait();

            var result = innerLogger.FormatException<TSource>(value);
            
            semaphore.Release();
            
            return result;
        }

        public string FormatException(
            Type logSource,
            string value)
        {
            semaphore.Wait();

            var result = innerLogger.FormatException(
                logSource,
                value);
            
            semaphore.Release();
            
            return result;
        }

        #endregion

        #endregion
    }
}