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

            loggerBuilder
                .ToggleAllowedByDefault(allowedByDefault)
                .AddSink(
                    LoggersFactoryUnity.BuildUnityDebugLogSink())
                .Wrap(
                    LoggersFactory.BuildLoggerWrapperWithLogTypePrefix(
                        loggerBuilder.CurrentLogger))
                .Wrap(
                    LoggersFactory.BuildLoggerWrapperWithSourceTypePrefix(
                        loggerBuilder.CurrentLogger));

            var loggerResolver = (ILoggerResolver)loggerBuilder;

            Container
                .Bind<ILoggerResolver>()
                .FromInstance(loggerResolver)
                .AsCached();
        }
    }
}