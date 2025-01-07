using System;
using System.Globalization;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Subscriptions;

using HereticalSolutions.Pools;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.AssetImport;

using HereticalSolutions.Logging;
using HereticalSolutions.Logging.Factories;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using Zenject;
using HereticalSolutions.Persistence;

namespace HereticalSolutions.Modules.Core_DefaultECS.DI
{
    public class LoggerResolverInstaller : MonoInstaller
    {
        [SerializeField]
        private LoggingSettingsScriptable loggingSettings;


        private ILogger rootLogger;

        private ISerializer fileSinkSerializer;


        private bool catchingLogs;

        private bool isQuittingApplication;

        public override void InstallBindings()
        {
            //Courtesy of https://stackoverflow.com/questions/114983/given-a-datetime-object-how-do-i-get-an-iso-8601-date-in-string-format
            //Read comments carefully

            string dateTimeNow = DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture);

            dateTimeNow = dateTimeNow.Replace('T', '_');

            dateTimeNow = dateTimeNow.Replace(':', '-');

            string logFileName = dateTimeNow;

            catchingLogs = loggingSettings.SendDebugLogsToLogger;

            ILoggerBuilder loggerBuilder = LoggersFactory.BuildLoggerBuilder();

            var loggerResolver = loggerBuilder

                .NewLogger()

                .ToggleAllowedByDefault(
                    loggingSettings.BasicLoggingSettings.EnableLoggingByDefault)

                //Log sources

                //TODO: extract to file
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
                    typeof(IManagedTypeResourceManager<,>),
                    false)

                .ToggleLogSource(
                    typeof(ResolveEntitiesOnSceneInstaller),
                    false)
                .ToggleLogSource(
                    typeof(PackedArrayManagedPool<>),
                    false)
                .ToggleLogSource(
                    typeof(AppendablePackedArrayManagedPool<>),
                    false)
                .ToggleLogSource(
                    typeof(StackManagedPool<>),
                    false)

                //.ToggleLogSource(
                //    typeof(EntityPrototypeImportInstaller),
                //    false)

                .ToggleLogSource(
                    typeof(EntityManager),
                    false)

                .ToggleLogSource(
                    typeof(EntityListManager),
                    false)

                //.ToggleLogSource(
                //    typeof(TimeSynchronizationBehaviour),
                //    false)
                //.ToggleLogSource(
                //    typeof(SimulationBehaviour),
                //    false)

                //.ToggleLogSource(
                //    typeof(PreventFromProcessingEventSystem<,>),
                //    false)
                //.ToggleLogSource(
                //    typeof(PreventFromProcessingUnlessOriginatedFromServerEventComponent<,>),
                //    false)
                //.ToggleLogSource(
                //    typeof(ReplicateToClientsEventSystem<,>),
                //    false)
                //.ToggleLogSource(
                //    typeof(ReplicateToClientsWithFilterEventSystem<,>),
                //    false)
                //.ToggleLogSource(
                //    typeof(ReplicateToServerEventSystem<,>),
                //    false)
                //.ToggleLogSource(
                //    typeof(ReplicateToServerWithFilterEventSystem<,>),
                //    false)
                //.ToggleLogSource(
                //    typeof(NetworkEntityManager),
                //    false)
                //.ToggleLogSource(
                //    typeof(SystemsInstaller),
                //    false)

                .ToggleLogSource(
                    typeof(HierarchyDeinitializationSystem<,>),
                    false)

                //Wrappers

                .AddWrapperBelow(
                    LoggersFactory.BuildProxyWrapper())

                .Build(); //Preemptively build the logger resolver so that it can be already injected

            loggerBuilder

                //Recursion prevention gate

                //THIS ONE IS PLACED BEFORE THE THREAD SAFETY WRAPPER FOR A REASON
                //IMAGINE AN ERROR LOG GOING IN
                //THE SEMAPHORE IS LOCKED
                //THE LOG IS GOING THROUGH ALL OF THE WRAPPERS AND REACHES UNITY DEBUG LOG BOTTOM WRAPPER
                //THE ERROR IS LOGGED WITH Debug.LogError
                //THEN THE FUN STARTS
                //THIS INSTALLER IS SUBSCRIBED TO UNITYS LOGS
                //IT SENDS IT DOWN THE LOGGER
                //WHERE IT REACHES THE FUCKING SEMAPHORE
                //AND WAITS FOR IT TO SPIN
                //WHILE Debug.LogError IS ACTUALLY A BLOCKING CALL
                //SO IT WONT START GOING UP THE CHAIN OF DELEGATES AND SPIN THE SEMAPHORE UNTIL THE CALLBACK IS FINISHED
                //AND CALLBACK WONT FINISH AS IT WAITS FOR THE SEMAPHORE TO SPIN
                //MAKING A DEADLOCK
                //THE EASIEST WAY TO PREVENT THIS IS TO PERFORM A RECURSION GATE BEFORE THE SEMAPHORE

                .AddWrapperBelow(
                    LoggersFactory.BuildLoggerWrapperWithRecursionPreventionGate())

                //Thread safety

                .AddWrapperBelow(
                    LoggersFactory.BuildLoggerWrapperWithSemaphoreSlim())

                //Prefixes

                .AddWrapperBelow(
                    LoggersFactory.BuildLoggerWrapperWithThreadIndexPrefix())
                .AddWrapperBelow(
                    LoggersFactory.BuildLoggerWrapperWithSourceTypePrefix())
                .AddWrapperBelow(
                    LoggersFactory.BuildLoggerWrapperWithLogTypePrefix())
                .AddWrapperBelow(
                    LoggersFactory.BuildLoggerWrapperWithTimestampPrefix(
                        loggingSettings.BasicLoggingSettings.LogTimeInUtc))

                // File sink

                .Branch();

            var branch = loggerBuilder.CurrentLogger;

            var fileSink = LoggersFactory.BuildFileSink(
                new FileAtApplicationDataPathSettings()
                {
                    //(loggingSettings.LoggingEnvironmentSettings.GetLogsFolderFromEnvironment)
                    //    ? System.Environment.GetEnvironmentVariable(
                    //        loggingSettings.LoggingEnvironmentSettings.LogsFolderEnvironmentKey)
                    //    : $"{Application.dataPath}/../",
                    //$"Runtime logs/{logFileName}.log",

                    RelativePath = $"../Runtime logs/{logFileName}.log"
                },
                loggerResolver);

            fileSinkSerializer = fileSink.Serializer;

            loggerBuilder.AddSink(
                fileSink);

            loggerBuilder.CurrentLogger = branch;

            // Recursion prevention prefix

            loggerBuilder

                .AddWrapperBelow(
                    LoggersFactory.BuildLoggerWrapperWithRecursionPreventionPrefix())

                //Toggling

                .AddWrapperBelow(
                    LoggersFactory.BuildLoggerWrapperWithToggling(
                        true,
                        false,
                        false,
                        true))

                // Sink

                .AddSink(
                    LoggersFactoryUnity.BuildUnityDebugLogSink());

                //Open stream

                var streamStrategy = fileSinkSerializer.Context.SerializationStrategy as IStrategyWithStream;

                streamStrategy?.InitializeAppend();

            Container
                .Bind<ILoggerResolver>()
                .FromInstance(loggerResolver)
                .AsCached();

            rootLogger = loggerBuilder.RootLogger;

            #region Catch logs

            if (catchingLogs)
            {
                Application.logMessageReceivedThreaded -= ReceivedLog;
                Application.logMessageReceivedThreaded += ReceivedLog;
            }

            #endregion
        }

        private void ReceivedLog(
            string logString,
            string stackTrace,
            LogType logType)
        {
#if UNITY_EDITOR
            if (isQuittingApplication)
                return;
#endif

            string log = string.IsNullOrEmpty(stackTrace)
                ? logString
                : $"{logString}\n{stackTrace}";

            switch (logType)
            {
                case LogType.Log:

                    rootLogger.Log<Application>(
                        log);

                    break;

                case LogType.Warning:

                    rootLogger.LogWarning<Application>(
                        log);

                    break;

                case LogType.Error:

                    rootLogger.LogError<Application>(
                        log);

                    break;

                case LogType.Assert:

                    rootLogger.Log<Application>(
                        log);

                    break;

                case LogType.Exception:

                    rootLogger.LogError<Application>(
                        log);

                    break;
            }


        }

#if UNITY_EDITOR
#if UNITY_2018_1_OR_NEWER
        private void OnApplicationQuitting()
#else
		private void OnApplicationQuit()
#endif
        {
            isQuittingApplication = true;
        }
#endif

        private void OnDestroy()
        {
            if (catchingLogs)
            {
                Application.logMessageReceivedThreaded -= ReceivedLog;
            }

            if (fileSinkSerializer != null)
            {
                var streamStrategy = fileSinkSerializer.Context.SerializationStrategy as IStrategyWithStream;
    
                streamStrategy?.FinalizeAppend();
            }
        }
    }
}