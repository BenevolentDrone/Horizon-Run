using System;
using System.IO;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
    [SerializationStrategy]
    public class TextFileStrategy
        : ISerializationStrategy,
          IStrategyWithIODestination,
          IStrategyWithFilter
    {
        private readonly ILogger logger;

        public string FullPath { get; private set; }

        public TextFileStrategy(
            string fullPath,
            ILogger logger = null)
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
                savePath))
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
                savePath))
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

            if (!IOHelpers.FileExists(
                savePath))
            {
                return false;
            }

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

            if (!IOHelpers.FileExists(
                savePath))
            {
                return false;
            }

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

            if (!IOHelpers.FileExists(
                savePath))
            {
                return false;
            }

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

            if (!IOHelpers.FileExists(
                savePath))
            {
                return false;
            }

            string contents = value.CastFromTo<object, string>();

            File.AppendAllText(savePath, contents);

            return true;
        }

        #endregion

        #endregion

        #region IStrategyWithIODestination

        public void EnsureIOTargetDestinationExists()
        {
            IOHelpers.EnsureDirectoryExists(
                FullPath,
                logger,
                GetType());
        }

        public bool IOTargetExists()
        {
            return IOHelpers.FileExists(
                FullPath,
                logger,
                GetType());
        }

        public void CreateIOTarget()
        {
            string savePath = FullPath;

            IOHelpers.EnsureDirectoryExists(
                FullPath,
                logger,
                GetType());

            if (!IOHelpers.FileExists(
                savePath,
                logger,
                GetType()))
            {
                File.Create(
                    FullPath);
            }
        }

        public void EraseIOTarget()
        {
            string savePath = FullPath;

            if (IOHelpers.FileExists(
                savePath,
                logger,
                GetType()))
            {
                File.Delete(savePath);
            }
        }

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