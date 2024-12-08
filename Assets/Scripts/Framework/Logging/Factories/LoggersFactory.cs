using System;
using System.Collections.Generic;
using System.Threading;

using HereticalSolutions.Persistence;
using HereticalSolutions.Persistence.Factories;

using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Logging.Factories
{
    public static class LoggersFactory
    {
        public static LoggerBuilder BuildLoggerBuilder()
        {
            return new LoggerBuilder(
                RepositoriesFactory.BuildDictionaryRepository<Type, bool>());
        }

        public static ConsoleSink BuildConsoleSink()
        {
            return new ConsoleSink();
        }

        public static CompositeLoggerWrapper BuildCompositeLoggerWrapper(
            IEnumerable<ILogger> innerLoggers)
        {
            return new CompositeLoggerWrapper(innerLoggers);
        }

        public static LoggerWrapperWithSemaphoreSlim BuildLoggerWrapperWithSemaphoreSlim(
            ILogger innerLogger)
        {
            return new LoggerWrapperWithSemaphoreSlim(
                new SemaphoreSlim(1, 1),
                innerLogger);
        }

        public static LoggerWrapperWithSourceTypePrefix BuildLoggerWrapperWithSourceTypePrefix(
            ILogger innerLogger)
        {
            return new LoggerWrapperWithSourceTypePrefix(innerLogger);
        }

        public static LoggerWrapperWithLogTypePrefix BuildLoggerWrapperWithLogTypePrefix(
            ILogger innerLogger)
        {
            return new LoggerWrapperWithLogTypePrefix(innerLogger);
        }
        
        public static LoggerWrapperWithThreadIndexPrefix BuildLoggerWrapperWithThreadIndexPrefix(
            ILogger innerLogger)
        {
            return new LoggerWrapperWithThreadIndexPrefix(innerLogger);
        }
        
        public static LoggerWrapperWithTimestampPrefix BuildLoggerWrapperWithTimestampPrefix(
            bool utc,
            ILogger innerLogger)
        {
            return new LoggerWrapperWithTimestampPrefix(
                utc,
                innerLogger);
        }
        
        public static LoggerWrapperWithRecursionPreventionPrefix BuildLoggerWrapperWithRecursionPreventionPrefix(
            ILogger innerLogger)
        {
            return new LoggerWrapperWithRecursionPreventionPrefix(innerLogger);
        }
        
        public static LoggerWrapperWithRecursionPreventionGate BuildLoggerWrapperWithRecursionPreventionGate(
            ILogger innerLogger)
        {
            return new LoggerWrapperWithRecursionPreventionGate(innerLogger);
        }

        public static LoggerWrapperWithToggling BuildLoggerWrapperWithToggling(
            ILogger innerLogger,
            bool active = true,
            bool logsActive = true,
            bool warningsActive = true,
            bool errorsActive = true)
        {
            return new LoggerWrapperWithToggling(
                innerLogger,
                active,
                logsActive,
                warningsActive,
                errorsActive);
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
    }
}