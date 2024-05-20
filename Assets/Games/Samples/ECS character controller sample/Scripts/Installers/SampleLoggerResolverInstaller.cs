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
                .AddOrWrap(
                    LoggersFactoryUnity.BuildUnityDebugLogger())
                .AddOrWrap(
                    LoggersFactory.BuildLoggerWrapperWithLogTypePrefix(
                        loggerBuilder.CurrentLogger))
                .AddOrWrap(
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