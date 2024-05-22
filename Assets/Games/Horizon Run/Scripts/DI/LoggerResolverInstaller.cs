using System;
using System.Globalization;

using HereticalSolutions.Delegates.Pinging;
using HereticalSolutions.Delegates.Broadcasting;
using HereticalSolutions.Delegates.Subscriptions;

using HereticalSolutions.Pools.Decorators;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.AssetImport;

using HereticalSolutions.Entities;

using HereticalSolutions.Logging;
using HereticalSolutions.Logging.Factories;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.HorizonRun.DI
{
    public class LoggerResolverInstaller : MonoInstaller
    {
        [SerializeField]
        private bool enableLoggingByDefault = true;

        private IDumpable dumpable;

        public override void InstallBindings()
        {
            //Courtesy of https://stackoverflow.com/questions/114983/given-a-datetime-object-how-do-i-get-an-iso-8601-date-in-string-format
            //Read comments carefully

            string dateTimeNow = DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture);

            dateTimeNow = dateTimeNow.Replace('T', '_');
            
            dateTimeNow = dateTimeNow.Replace(':', '-');

            string logFileName = dateTimeNow;
            
            ILoggerBuilder loggerBuilder = LoggersFactory.BuildLoggerBuilder();

            loggerBuilder
                .ToggleAllowedByDefault(enableLoggingByDefault)

                .AddOrWrap(
                    LoggersFactoryUnity.BuildUnityDebugLogger())
                .AddOrWrap(
                    LoggersFactory.BuildLoggerWrapperWithFileDump(
#if UNITY_EDITOR                        
                        $"{Application.dataPath}/../",
#else
                        //Turns out if i leave it at Application.dataPath then the "Runtime logs" folder gets created
                        //inside the _Data folder
                        $"{Application.dataPath}/../",
#endif
                        $"Runtime logs/{logFileName}.log",
                        (ILoggerResolver)loggerBuilder,
                        loggerBuilder.CurrentLogger))
                .AddOrWrap(
                    LoggersFactory.BuildLoggerWrapperWithLogTypePrefix(
                        loggerBuilder.CurrentLogger))
                .AddOrWrap(
                    LoggersFactory.BuildLoggerWrapperWithSourceTypePrefix(
                        loggerBuilder.CurrentLogger))

                .ToggleLogSource(
                    typeof(NonAllocPinger),
                    false)
                .ToggleLogSource(
                    typeof(NonAllocBroadcasterGeneric<>),
                    false)
                .ToggleLogSource(
                    typeof(SubscriptionSingleArgGeneric<>),
                    false)

                .ToggleLogSource(
                    typeof(PreallocatedResourceStorageHandle<>),
                    false)
                .ToggleLogSource(
                    typeof(ResourceImporterFromScriptable),
                    false)
                .ToggleLogSource(
                    typeof(AssetImportManager),
                    false)

                .ToggleLogSource(
                    typeof(ResolveEntitiesOnSceneInstaller),
                    false)
                .ToggleLogSource(
                    typeof(SupplyAndMergePool<>),
                    false)
                .ToggleLogSource(
                    typeof(ResizableNonAllocPool<>),
                    false)
                
                .ToggleLogSource(
                    typeof(EntityPrototypeImportInstaller),
                    false)

                .ToggleLogSource(
                    typeof(DefaultECSEntityManager<>),
                    false)
                
                .ToggleLogSource(
                    typeof(DefaultECSEntityListManager),
                    false)
                
                .ToggleLogSource(
                    typeof(SystemsInstaller),
                    false)
                
                .ToggleLogSource(
                    typeof(HierarchyDeinitializationSystem<,>),
                    false);

            var loggerResolver = (ILoggerResolver)loggerBuilder;

            Container
                .Bind<ILoggerResolver>()
                .FromInstance(loggerResolver)
                .AsCached();


            dumpable = null;
            
            var currentLogger = loggerBuilder.CurrentLogger;

            do
            {
                if (currentLogger is IDumpable dumpableLogger)
                {
                    dumpable = (IDumpable)currentLogger;
                    
                    break;
                }

                if (currentLogger is not ILoggerWrapper loggerWrapper)
                {
                    dumpable = null;
                    
                    break;
                }

                currentLogger = loggerWrapper.InnerLogger;
            }
            while (true);

            if (dumpable != null)
            {
                Container
                    .Bind<IDumpable>()
                    .FromInstance(dumpable)
                    .AsCached();
            }
        }
        
        private void OnDestroy()
        {
            dumpable?.Dump();
        }
    }
}