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
            ILoggerBuilder loggerBuilder = LoggerFactory.BuildLoggerBuilder();

            var loggerResolver = loggerBuilder
                .NewLogger()
                .ToggleAllowedByDefault(allowedByDefault)
                .AddWrapperBelow(
                    LoggerFactory.BuildLoggerWrapperWithSourceTypePrefix())
                .AddWrapperBelow(
                    LoggerFactory.BuildLoggerWrapperWithLogTypePrefix())
                .AddWrapperBelow(
                    LoggerFactory.BuildLoggerWrapperWithTimestampPrefix(false))
                .AddSink(
                    LoggerFactoryUnity.BuildUnityDebugLogSink())
                .Build();

            Container
                .Bind<ILoggerResolver>()
                .FromInstance(loggerResolver)
                .AsCached();
        }
    }
}