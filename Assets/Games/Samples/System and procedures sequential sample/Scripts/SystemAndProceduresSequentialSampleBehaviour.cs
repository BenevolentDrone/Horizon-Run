using System;
using System.Globalization;

using HereticalSolutions.Persistence;

using HereticalSolutions.Systems;
using HereticalSolutions.Systems.Factories;

using HereticalSolutions.Logging;
using HereticalSolutions.Logging.Factories;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

namespace HereticalSolutions.Samples.SystemAndProceduresSequentialSample
{
	public class SystemAndProceduresSequentialSampleBehaviour : MonoBehaviour
	{
		private bool isQuittingApplication;

		private ILoggerResolver loggerResolver;

		private ILogger logger;

		private ISerializer fileSinkSerializer;

		private Action delegateSystem;

		void Start()
		{
			#region Initiate logger resolver and logger itself

			string dateTimeNow = DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture);

			dateTimeNow = dateTimeNow.Replace('T', '_');

			dateTimeNow = dateTimeNow.Replace(':', '-');

			string logFileName = dateTimeNow;


			ILoggerBuilder loggerBuilder = LoggersFactory.BuildLoggerBuilder();

			loggerResolver = loggerBuilder

				.NewLogger()

				.ToggleAllowedByDefault(
					true)

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
						false))

				// File sink

				.Branch();

			var branch = loggerBuilder.CurrentLogger;

			var fileSink = LoggersFactory.BuildFileSink(
				new FileAtApplicationDataPathSettings()
				{
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
						true,
						true,
						true))

				// Sink

				.AddSink(
					LoggersFactoryUnity.BuildUnityDebugLogSink());

			//Open stream

			var streamStrategy = fileSinkSerializer.Context.SerializationStrategy as IStrategyWithStream;

			streamStrategy?.InitializeAppend();

			logger = loggerResolver.GetLogger<SystemAndProceduresSequentialSampleBehaviour>();

			#endregion

			#region Catch logs

			Application.logMessageReceivedThreaded -= ReceivedLog;
			Application.logMessageReceivedThreaded += ReceivedLog;

			#endregion

			var delegateSystemBuilder = SystemFactory.BuildDelegateSystemBuilder(
				loggerResolver);

			//Add the process that lasts from start to finish
			delegateSystemBuilder.AddStageNodesBetweenStartAndFinish(
				"SAMPLE PROCESS");

			delegateSystemBuilder.TryGetStageNode(
				SystemBuilderExtensions.GetStageFinishNodeID("SAMPLE PROCESS"),
				out var sampleProcessFinishNode);


			//Add the main thread process
			delegateSystemBuilder.AddStageNodesAfterStageStart<Action, Action>(
				"MAIN THREAD PROCESS",
				"SAMPLE PROCESS",
				false);


			//Add the thread 1 process
			delegateSystemBuilder.AddStageNodesAfterStageStart<Action, Action>(
				"THREAD 1 PROCESS",
				"SAMPLE PROCESS",
				true);

			delegateSystemBuilder.TryGetStageNode(
				SystemBuilderExtensions.GetStageFinishNodeID("THREAD 1 PROCESS"),
				out var thread1FinishNode);

			delegateSystemBuilder.TryLinkNodes(
				thread1FinishNode,
				sampleProcessFinishNode);


			//Add the thread 2 process
			delegateSystemBuilder.AddStageNodesAfterStageStart<Action, Action>(
				"THREAD 2 PROCESS",
				"SAMPLE PROCESS",
				true);

			delegateSystemBuilder.TryGetStageNode(
				SystemBuilderExtensions.GetStageFinishNodeID("THREAD 2 PROCESS"),
				out var thread2FinishNode);

			delegateSystemBuilder.TryLinkNodes(
				thread2FinishNode,
				sampleProcessFinishNode);

			
			//Add the main thread procedures
			delegateSystemBuilder.TryAddAfterStage(
				SystemBuilderExtensions.GetStageStartNodeID("MAIN THREAD PROCESS"),
				SystemFactory.BuildProcedureNode<Action>(
					() =>
					{
						logger.Log(
							GetType(),
							"MAIN THREAD PROCEDURE 1 DELEGATE INVOKED");
					}));

			delegateSystemBuilder.TryAddBeforeStage(
				SystemBuilderExtensions.GetStageFinishNodeID("MAIN THREAD PROCESS"),
				SystemFactory.BuildProcedureNode<Action>(
					() =>
					{
						logger.Log(
							GetType(),
							"MAIN THREAD PROCEDURE 2 DELEGATE INVOKED");
					}));

			//Add the thread 1 procedures
			var thread1Procedure1 = SystemFactory.BuildProcedureNode<Action>(
				() =>
				{
					logger.Log(
						GetType(),
						"THREAD 1 PROCEDURE 1 DELEGATE INVOKED");
				});

			delegateSystemBuilder.TryAddAfterStage(
				SystemBuilderExtensions.GetStageStartNodeID("THREAD 1 PROCESS"),
				thread1Procedure1);

			var thread1Procedure2 = SystemFactory.BuildProcedureNode<Action>(
				() =>
				{
					logger.Log(
						GetType(),
						"THREAD 1 PROCEDURE 2 DELEGATE INVOKED");
				});

			delegateSystemBuilder.TryAddAfterNode(
				thread1Procedure1,
				thread1Procedure2);

			var thread1Procedure3 = SystemFactory.BuildProcedureNode<Action>(
				() =>
				{
					logger.Log(
						GetType(),
						"THREAD 1 PROCEDURE 3 DELEGATE INVOKED");
				});

			delegateSystemBuilder.TryAddAfterNode(
				thread1Procedure2,
				thread1Procedure3);

			var thread1Procedure4 = SystemFactory.BuildProcedureNode<Action>(
				() =>
				{
					logger.Log(
						GetType(),
						"THREAD 1 PROCEDURE 4 DELEGATE INVOKED");
				});

			delegateSystemBuilder.TryAddAfterNode(
				thread1Procedure3,
				thread1Procedure4);

			var thread1Procedure5 = SystemFactory.BuildProcedureNode<Action>(
				() =>
				{
					logger.Log(
						GetType(),
						"THREAD 1 PROCEDURE5 DELEGATE INVOKED");
				});

			delegateSystemBuilder.TryAddAfterNode(
				thread1Procedure4,
				thread1Procedure5);


			//Add the thread 2 procedures
			var thread2Procedure1 = SystemFactory.BuildProcedureNode<Action>(
				() =>
				{
					logger.Log(
						GetType(),
						"THREAD 2 PROCEDURE 1 DELEGATE INVOKED");
				});

			delegateSystemBuilder.TryAddBeforeNode(
				thread2FinishNode,
				thread2Procedure1);

			var thread2Procedure2 = SystemFactory.BuildProcedureNode<Action>(
				() =>
				{
					logger.Log(
						GetType(),
						"THREAD 2 PROCEDURE 2 DELEGATE INVOKED");
				});

			delegateSystemBuilder.TryAddBeforeNode(
				thread2FinishNode,
				thread2Procedure2);


			//Link nodes from different threads for additional assertions
			delegateSystemBuilder.TryLinkNodes(
				thread1Procedure1,
				thread2Procedure2);

			delegateSystemBuilder.TryLinkNodes(
				thread2Procedure1,
				thread1Procedure2);

			delegateSystemBuilder.TryLinkNodes(
				thread1Procedure5,
				thread2Procedure2);

			//Validate the system
			if (!delegateSystemBuilder.ValidateSystem())
			{
				logger.LogError(
					GetType(),
					"Failed to validate the system");

				return;
			}

			//Build the system
			if (!delegateSystemBuilder.BuildSystem(
				out var delegateSystem))
			{
				logger.LogError(
					GetType(),
					"Failed to build the system");

				return;
			}

			Perform();
		}

		[ContextMenu("Perform")]
		private void Perform()
		{
			logger.Log(
				"START");

			//Run the system
			delegateSystem?.Invoke();

			logger.Log(
				"FINISH");
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

					logger?.Log<Application>(
						log);

					break;

				case LogType.Warning:

					logger?.LogWarning<Application>(
						log);

					break;

				case LogType.Error:

					logger?.LogError<Application>(
						log);

					break;

				case LogType.Assert:

					logger?.Log<Application>(
						log);

					break;

				case LogType.Exception:

					logger?.LogError<Application>(
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
			Application.logMessageReceivedThreaded -= ReceivedLog;
		}
	}
}