using System;
using System.Globalization;

using HereticalSolutions.Delegates.Pinging;
using HereticalSolutions.Delegates.Broadcasting;
using HereticalSolutions.Delegates.Subscriptions;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Decorators;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.AssetImport;

using HereticalSolutions.Entities;

using HereticalSolutions.Logging;
using HereticalSolutions.Logging.Factories;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Modules.Core_DefaultECS.DI
{
    public class LoggerResolverInstaller : MonoInstaller
    {
        [SerializeField]
        private LoggingSettingsScriptable loggingSettings;


        private ILogger cachedLogger;

        private IDumpable cachedDumpableLogger;


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

            //Remember that the log traverses wrappers in a bottom to top order
            //While prefixes are added in a top to bottom order
            loggerBuilder
                .ToggleAllowedByDefault(
                    loggingSettings.BasicLoggingSettings.EnableLoggingByDefault)

                // Output

                .AddOrWrap(
                    LoggersFactoryUnity.BuildUnityDebugLogger(
                        false,
                        false,
                        true))

                // Recursion prevention prefix

                .AddOrWrap(
                    LoggersFactory.BuildLoggerWrapperWithRecursionPreventionPrefix(
                        loggerBuilder.CurrentLogger))

                // Logging to file

                .AddOrWrap(
                    LoggersFactory.BuildLoggerWrapperWithFileDump(
                        (loggingSettings.LoggingEnvironmentSettings.GetLogsFolderFromEnvironment)
                            ? System.Environment.GetEnvironmentVariable(
                                loggingSettings.LoggingEnvironmentSettings.LogsFolderEnvironmentKey)
                            : $"{Application.dataPath}/../",
                        $"Runtime logs/{logFileName}.log",
                        (ILoggerResolver)loggerBuilder,
                        loggerBuilder.CurrentLogger))

                //Prefixes

                .AddOrWrap(
                    LoggersFactory.BuildLoggerWrapperWithTimestampPrefix(
                        loggingSettings.BasicLoggingSettings.LogTimeInUtc,
                        loggerBuilder.CurrentLogger))
                .AddOrWrap(
                    LoggersFactory.BuildLoggerWrapperWithLogTypePrefix(
                        loggerBuilder.CurrentLogger))
                .AddOrWrap(
                    LoggersFactory.BuildLoggerWrapperWithSourceTypePrefix(
                        loggerBuilder.CurrentLogger))
                .AddOrWrap(
                    LoggersFactory.BuildLoggerWrapperWithThreadIndexPrefix(
                        loggerBuilder.CurrentLogger))

                //Thread safety

                .AddOrWrap(
                    LoggersFactory.BuildLoggerWrapperWithSemaphoreSlim(
                        loggerBuilder.CurrentLogger))

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

                .AddOrWrap(
                    LoggersFactory.BuildLoggerWrapperWithRecursionPreventionGate(
                        loggerBuilder.CurrentLogger))

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
                    false);

            var loggerResolver = (ILoggerResolver)loggerBuilder;

            Container
                .Bind<ILoggerResolver>()
                .FromInstance(loggerResolver)
                .AsCached();

            cachedLogger = loggerBuilder.CurrentLogger;

            #region Dumpable

            cachedDumpableLogger = null;

            var currentLogger = loggerBuilder.CurrentLogger;

            do
            {
                if (currentLogger is IDumpable dumpableLogger)
                {
                    cachedDumpableLogger = (IDumpable)currentLogger;

                    break;
                }

                if (currentLogger is not ILoggerWrapper loggerWrapper)
                {
                    cachedDumpableLogger = null;

                    break;
                }

                currentLogger = loggerWrapper.InnerLogger;
            }
            while (true);

            if (cachedDumpableLogger != null)
            {
                Container
                    .Bind<IDumpable>()
                    .FromInstance(cachedDumpableLogger)
                    .AsCached();
            }

            #endregion

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

                    cachedLogger.Log<Application>(
                        log);

                    break;

                case LogType.Warning:

                    cachedLogger.LogWarning<Application>(
                        log);

                    break;

                case LogType.Error:

                    cachedLogger.LogError<Application>(
                        log);

                    break;

                case LogType.Assert:

                    cachedLogger.Log<Application>(
                        log);

                    break;

                case LogType.Exception:

                    cachedLogger.LogError<Application>(
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

            cachedDumpableLogger?.Dump();
        }
    }
}