using HereticalSolutions.Logging;
using HereticalSolutions.Logging.Factories;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample.Installers
{
    public class SampleLoggerResolverInstaller : MonoInstaller
    {
        [SerializeField]
        private bool allowedByDefault = true;

        public override void InstallBindings()
        {
            ILoggerBuilder loggerBuilder = LoggersFactory.BuildLoggerBuilder();

            var loggerResolver = loggerBuilder
                .NewLogger()
                .ToggleAllowedByDefault(allowedByDefault)
                .AddWrapperBelow(
                    LoggersFactory.BuildLoggerWrapperWithSourceTypePrefix())
                .AddWrapperBelow(
                    LoggersFactory.BuildLoggerWrapperWithLogTypePrefix())
                .AddWrapperBelow(
                    LoggersFactory.BuildLoggerWrapperWithTimestampPrefix(false))
                .AddSink(
                    LoggersFactoryUnity.BuildUnityDebugLogSink())
                .Build();

            Container
                .Bind<ILoggerResolver>()
                .FromInstance(loggerResolver)
                .AsCached();
        }
    }
}