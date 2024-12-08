using System;
using System.Collections.Generic;
using System.Threading;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Persistence;
using HereticalSolutions.Persistence.Factories;

namespace HereticalSolutions.Logging.Factories
{
    public static class LoggersFactory
    {
        public static LoggerBuilder BuildLoggerBuilder()
        {
            return new LoggerBuilder();
        }

        public static SharedLoggerResolver BuildSharedLoggerResolver(
            ILogger rootLogger,
            IRepository<Type, bool> explicitLogSourceRules,
            bool allowedByDefault)
        {
            return new SharedLoggerResolver(
                rootLogger,
                explicitLogSourceRules,
                allowedByDefault);
        }

        #region Wrappers

        public static ProxyWrapper BuildProxyWrapper()
        {
            return new ProxyWrapper();
        }

        public static CompositeLoggerWrapper BuildCompositeLoggerWrapper()
        {
            return new CompositeLoggerWrapper(
                new List<ILogger>());
        }

        public static LoggerWrapperWithSemaphoreSlim BuildLoggerWrapperWithSemaphoreSlim()
        {
            return new LoggerWrapperWithSemaphoreSlim(
                new SemaphoreSlim(1, 1));
        }

        public static LoggerWrapperWithSourceTypePrefix BuildLoggerWrapperWithSourceTypePrefix()
        {
            return new LoggerWrapperWithSourceTypePrefix();
        }

        public static LoggerWrapperWithLogTypePrefix BuildLoggerWrapperWithLogTypePrefix()
        {
            return new LoggerWrapperWithLogTypePrefix();
        }
        
        public static LoggerWrapperWithThreadIndexPrefix BuildLoggerWrapperWithThreadIndexPrefix()
        {
            return new LoggerWrapperWithThreadIndexPrefix();
        }
        
        public static LoggerWrapperWithTimestampPrefix BuildLoggerWrapperWithTimestampPrefix(
            bool utc)
        {
            return new LoggerWrapperWithTimestampPrefix(
                utc);
        }
        
        public static LoggerWrapperWithRecursionPreventionPrefix BuildLoggerWrapperWithRecursionPreventionPrefix()
        {
            return new LoggerWrapperWithRecursionPreventionPrefix();
        }
        
        public static LoggerWrapperWithRecursionPreventionGate BuildLoggerWrapperWithRecursionPreventionGate()
        {
            return new LoggerWrapperWithRecursionPreventionGate();
        }

        public static LoggerWrapperWithToggling BuildLoggerWrapperWithToggling(
            bool active = true,
            bool logsActive = true,
            bool warningsActive = true,
            bool errorsActive = true)
        {
            return new LoggerWrapperWithToggling(
                active,
                logsActive,
                warningsActive,
                errorsActive);
        }

        #endregion

        #region Sinks

        public static ConsoleSink BuildConsoleSink()
        {
            return new ConsoleSink();
        }

        public static FileSink BuildFileSink(
            IPathSettings pathSettings,
            ILoggerResolver loggerResolver)
        {
            var serializerBuilder = PersistenceFactory.BuildSerializerBuilder(
                loggerResolver);

            var serializer = serializerBuilder
                .NewSerializer()
                .From<IPathSettings>(
                    pathSettings)
                .ToObject()
                .AsTextStream()
                .WithAppend()
                .Build();
                
            var result = new FileSink(
                serializer);

            return result;
        }

        #endregion
    }
}