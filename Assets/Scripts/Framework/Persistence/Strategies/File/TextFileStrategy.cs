using System;
using System.Threading.Tasks;
using System.IO;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
    [SerializationStrategy]
    public class TextFileStrategy
        : ISerializationStrategy,
          IStrategyWithFilter,
          IHasIODestination,
          IAsyncSerializationStrategy
    {
        private readonly ILogger logger;

        public string FullPath { get; private set; }

        public TextFileStrategy(
            string fullPath,
            ILogger logger)
        {
            FullPath = fullPath;

            this.logger = logger;
        }

        #region ISerializationStrategy

        #region Read

        public bool Read<TValue>(
            out TValue value)
        {
            AssertStrategyIsValid(
                typeof(TValue));

            string savePath = FullPath;

            string result = string.Empty;

            if (!IOHelpers.FileExists(
                savePath,
                logger))
            {
                value = result.CastFromTo<string, TValue>();

                return false;
            }

            result = File.ReadAllText(savePath);

            value = result.CastFromTo<string, TValue>();

            return true;
        }

        public bool Read(
            Type valueType,
            out object value)
        {
            AssertStrategyIsValid(
                valueType);

            string savePath = FullPath;

            string result = string.Empty;

            if (!IOHelpers.FileExists(
                savePath,
                logger))
            {
                value = result.CastFromTo<string, object>();

                return false;
            }

            result = File.ReadAllText(savePath);

            value = result.CastFromTo<string, object>();

            return true;
        }

        #endregion

        #region Write

        public bool Write<TValue>(
            TValue value)
        {
            AssertStrategyIsValid(
                typeof(TValue));

            string savePath = FullPath;

            string contents = value.CastFromTo<TValue, string>();

            File.WriteAllText(savePath, contents);

            return true;
        }

        public bool Write(
            Type valueType,
            object value)
        {
            AssertStrategyIsValid(
                valueType);

            string savePath = FullPath;

            string contents = value.CastFromTo<object, string>();

            File.WriteAllText(savePath, contents);

            return true;
        }

        #endregion

        #region Append

        public bool Append<TValue>(
            TValue value)
        {
            AssertStrategyIsValid(
                typeof(TValue));

            string savePath = FullPath;

            string contents = value.CastFromTo<TValue, string>();

            File.AppendAllText(savePath, contents);

            return true;
        }

        public bool Append(
            Type valueType,
            object value)
        {
            AssertStrategyIsValid(
                valueType);

            string savePath = FullPath;

            string contents = value.CastFromTo<object, string>();

            File.AppendAllText(savePath, contents);

            return true;
        }

        #endregion

        #endregion

        #region IStrategyWithFilter

        public bool AllowsType<TValue>()
        {
            return typeof(TValue) == typeof(string);
        }

        public bool AllowsType(
            Type valueType)
        {
            return valueType == typeof(string);
        }

        #endregion

        #region IStrategyWithIODestination

        public void EnsureIODestinationExists()
        {
            IOHelpers.EnsureDirectoryExists(
                FullPath,
                logger);
        }

        public bool IODestinationExists()
        {
            return IOHelpers.FileExists(
                FullPath,
                logger);
        }

        public void CreateIODestination()
        {
            string savePath = FullPath;

            IOHelpers.EnsureDirectoryExists(
                FullPath,
                logger);

            if (!IOHelpers.FileExists(
                savePath,
                logger))
            {
                File.Create(
                    FullPath);
            }
        }

        public void EraseIODestination()
        {
            string savePath = FullPath;

            if (IOHelpers.FileExists(
                savePath,
                logger))
            {
                File.Delete(savePath);
            }
        }

        #endregion

        #region IAsyncSerializationStrategy

        #region Read

        public async Task<(bool, TValue)> ReadAsync<TValue>(

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            AssertStrategyIsValid(
                typeof(TValue));

            string savePath = FullPath;

            TValue value = default;

            if (!IOHelpers.FileExists(
                savePath,
                logger))
            {
                return (false, value);
            }

            string result = await File.ReadAllTextAsync(
                savePath);

            value = result.CastFromTo<string, TValue>();

            return (true, value);
        }

        public async Task<(bool, object)> ReadAsync(
            Type valueType,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            AssertStrategyIsValid(
                valueType);

            string savePath = FullPath;

            object value = default;

            if (!IOHelpers.FileExists(
                savePath,
                logger))
            {
                return (false, value);
            }

            string result = await File.ReadAllTextAsync(
                savePath);

            value = result.CastFromTo<string, object>();

            return (true, value);
        }

        #endregion

        #region Write

        public async Task<bool> WriteAsync<TValue>(
            TValue value,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            AssertStrategyIsValid(
                typeof(TValue));

            string savePath = FullPath;

            string contents = value.CastFromTo<TValue, string>();

            await File.WriteAllTextAsync(
                savePath,
                contents);

            return true;
        }

        public async Task<bool> WriteAsync(
            Type valueType,
            object value,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            AssertStrategyIsValid(
                valueType);

            string savePath = FullPath;

            string contents = value.CastFromTo<object, string>();

            await File.WriteAllTextAsync(
                savePath,
                contents);

            return true;
        }

        #endregion

        #region Append

        public async Task<bool> AppendAsync<TValue>(
            TValue value,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            AssertStrategyIsValid(
                typeof(TValue));

            string savePath = FullPath;

            string contents = value.CastFromTo<TValue, string>();

            await File.AppendAllTextAsync(
                savePath,
                contents);

            return true;
        }

        public async Task<bool> AppendAsync(
            Type valueType,
            object value,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            AssertStrategyIsValid(
                valueType);

            string savePath = FullPath;

            string contents = value.CastFromTo<object, string>();

            await File.AppendAllTextAsync(
                savePath,
                contents);

            return true;
        }

        #endregion

        #endregion

        private void AssertStrategyIsValid(
            Type valueType)
        {
            if (valueType != typeof(string))
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"INVALID VALUE TYPE: {valueType.Name}"));
        }
    }
}