using System;
using System.Globalization;
using System.Threading.Tasks;

using HereticalSolutions.Persistence;

using HereticalSolutions.Systems;
using HereticalSolutions.Systems.Factories;

using HereticalSolutions.Logging;
using HereticalSolutions.Logging.Factories;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

namespace HereticalSolutions.Samples.SystemAndProceduresAsyncSample
{
	public class SystemAndProceduresAsyncSampleBehaviour : MonoBehaviour
	{
		private bool isQuittingApplication;

		private ILoggerResolver loggerResolver;

		private ILogger logger;

		private ISerializer fileSinkSerializer;

		private Func<Task> delegateSystem;


		void Start()
		{
			#region Initiate logger resolver and logger itself

			string dateTimeNow = DateTime.UtcNow.ToString("s", CultureInfo.InvariantCulture);

			dateTimeNow = dateTimeNow.Replace('T', '_');

			dateTimeNow = dateTimeNow.Replace(':', '-');

			string logFileName = dateTimeNow;


			ILoggerBuilder loggerBuilder = LoggerFactory.BuildLoggerBuilder();

			var loggerResolver = loggerBuilder

					.NewLogger()

					.ToggleAllowedByDefault(
						true)

					//Wrappers

					.AddWrapperBelow(
						LoggerFactory.BuildProxyWrapper())

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
					LoggerFactory.BuildLoggerWrapperWithRecursionPreventionGate())

				//Thread safety

				.AddWrapperBelow(
					LoggerFactory.BuildLoggerWrapperWithSemaphoreSlim())

				//Prefixes

				.AddWrapperBelow(
					LoggerFactory.BuildLoggerWrapperWithThreadIndexPrefix())
				.AddWrapperBelow(
					LoggerFactory.BuildLoggerWrapperWithSourceTypePrefix())
				.AddWrapperBelow(
					LoggerFactory.BuildLoggerWrapperWithLogTypePrefix())
				.AddWrapperBelow(
					LoggerFactory.BuildLoggerWrapperWithTimestampPrefix(
						false))

				// File sink

				.Branch();

			var branch = loggerBuilder.CurrentLogger;

			var fileSink = LoggerFactory.BuildFileSink(
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
					LoggerFactory.BuildLoggerWrapperWithRecursionPreventionPrefix())

				//Toggling

				.AddWrapperBelow(
					LoggerFactory.BuildLoggerWrapperWithToggling(
						true,
						true,
						true,
						true))

				// Sink

				.AddSink(
					LoggerFactoryUnity.BuildUnityDebugLogSink());

			//Open stream

			var streamStrategy = fileSinkSerializer.Context.SerializationStrategy as IStrategyWithStream;

			streamStrategy?.InitializeAppend();

			logger = loggerResolver.GetLogger<SystemAndProceduresAsyncSampleBehaviour>();

			#endregion

			#region Catch logs

			Application.logMessageReceivedThreaded -= ReceivedLog;
			Application.logMessageReceivedThreaded += ReceivedLog;

			#endregion

			var asyncSystemBuilder = SystemFactory.BuildAsyncSystemBuilder(
				loggerResolver);

			//Add the process that lasts from start to finish
			asyncSystemBuilder.AddStageNodesBetweenStartAndFinish(
				"SAMPLE PROCESS");

			asyncSystemBuilder.TryGetStageNode(
				SystemBuilderExtensions.GetStageFinishNodeID("SAMPLE PROCESS"),
				out var sampleProcessFinishNode);


			//Add the main thread process
			asyncSystemBuilder.AddStageNodesAfterStageStart<Func<Task>, Func<Task>>(
				"MAIN THREAD PROCESS",
				"SAMPLE PROCESS",
				false);


			//Add the thread 1 process
			asyncSystemBuilder.AddStageNodesAfterStageStart<Func<Task>, Func<Task>>(
				"THREAD 1 PROCESS",
				"SAMPLE PROCESS",
				true);

			asyncSystemBuilder.TryGetStageNode(
				SystemBuilderExtensions.GetStageFinishNodeID("THREAD 1 PROCESS"),
				out var thread1FinishNode);

			asyncSystemBuilder.TryLinkNodes(
				thread1FinishNode,
				sampleProcessFinishNode);


			//Add the thread 2 process
			asyncSystemBuilder.AddStageNodesAfterStageStart<Func<Task>, Func<Task>>(
				"THREAD 2 PROCESS",
				"SAMPLE PROCESS",
				true);

			asyncSystemBuilder.TryGetStageNode(
				SystemBuilderExtensions.GetStageFinishNodeID("THREAD 2 PROCESS"),
				out var thread2FinishNode);

			asyncSystemBuilder.TryLinkNodes(
				thread2FinishNode,
				sampleProcessFinishNode);


			//Add the main thread procedures
			asyncSystemBuilder.TryAddAfterStage(
				SystemBuilderExtensions.GetStageStartNodeID("MAIN THREAD PROCESS"),
				SystemFactory.BuildProcedureNode<Func<Task>>(
					CommonProcedures.CreateTaskFactoryFromAction(
						() =>
						{
							logger.Log(
								GetType(),
								"MAIN THREAD PROCEDURE 1 DELEGATE INVOKED");
						})));

			asyncSystemBuilder.TryAddBeforeStage(
				SystemBuilderExtensions.GetStageFinishNodeID("MAIN THREAD PROCESS"),
				SystemFactory.BuildProcedureNode<Func<Task>>(
					CommonProcedures.CreateTaskFactoryFromAction(
						() =>
						{
							logger.Log(
								GetType(),
								"MAIN THREAD PROCEDURE 2 DELEGATE INVOKED");
						})));

			//Add the thread 1 procedures
			var thread1Procedure1 = SystemFactory.BuildProcedureNode<Func<Task>>(
				CommonProcedures.CreateTaskFactoryFromAction(
					() =>
					{
						logger.Log(
							GetType(),
							"THREAD 1 PROCEDURE 1 DELEGATE INVOKED");
					}));

			asyncSystemBuilder.TryAddAfterStage(
				SystemBuilderExtensions.GetStageStartNodeID("THREAD 1 PROCESS"),
				thread1Procedure1);

			var thread1Procedure2 = SystemFactory.BuildProcedureNode<Func<Task>>(
				CommonProcedures.CreateTaskFactoryFromAction(
					() =>
					{
						logger.Log(
							GetType(),
							"THREAD 1 PROCEDURE 2 DELEGATE INVOKED");
					}));

			asyncSystemBuilder.TryAddAfterNode(
				thread1Procedure1,
				thread1Procedure2);

			var thread1Procedure3 = SystemFactory.BuildProcedureNode<Func<Task>>(
				CommonProcedures.CreateTaskFactoryFromAction(
					() =>
					{
						logger.Log(
							GetType(),
							"THREAD 1 PROCEDURE 3 DELEGATE INVOKED");
					}));

			asyncSystemBuilder.TryAddAfterNode(
				thread1Procedure2,
				thread1Procedure3);

			var thread1Procedure4 = SystemFactory.BuildProcedureNode<Func<Task>>(
				CommonProcedures.CreateTaskFactoryFromAction(
					() =>
					{
						logger.Log(
							GetType(),
							"THREAD 1 PROCEDURE 4 DELEGATE INVOKED");
					}));

			asyncSystemBuilder.TryAddAfterNode(
				thread1Procedure3,
				thread1Procedure4);

			var thread1Procedure5 = SystemFactory.BuildProcedureNode<Func<Task>>(
				CommonProcedures.CreateTaskFactoryFromAction(
					() =>
					{
						logger.Log(
							GetType(),
							"THREAD 1 PROCEDURE 5 DELEGATE INVOKED");
					}));

			asyncSystemBuilder.TryAddAfterNode(
				thread1Procedure4,
				thread1Procedure5);


			//Add the thread 2 procedures
			var thread2Procedure1 = SystemFactory.BuildProcedureNode<Func<Task>>(
				CommonProcedures.CreateTaskFactoryFromAction(
					() =>
					{
						logger.Log(
							GetType(),
							"THREAD 2 PROCEDURE 1 DELEGATE INVOKED");
					}));

			asyncSystemBuilder.TryAddBeforeNode(
				thread2FinishNode,
				thread2Procedure1);

			var thread2Procedure2 = SystemFactory.BuildProcedureNode<Func<Task>>(
				CommonProcedures.CreateTaskFactoryFromAction(
					() =>
					{
						logger.Log(
							GetType(),
							"THREAD 2 PROCEDURE 2 DELEGATE INVOKED");
					}));

			asyncSystemBuilder.TryAddBeforeNode(
				thread2FinishNode,
				thread2Procedure2);


			//Link nodes from different threads for additional assertions
			asyncSystemBuilder.TryLinkNodes(
				thread1Procedure1,
				thread2Procedure2);

			asyncSystemBuilder.TryLinkNodes(
				thread2Procedure1,
				thread1Procedure2);

			asyncSystemBuilder.TryLinkNodes(
				thread1Procedure5,
				thread2Procedure2);

			//Validate the system
			if (!asyncSystemBuilder.ValidateSystem())
			{
				logger.LogError(
					GetType(),
					"Failed to validate the system");

				return;
			}

			//Build the system
			if (!asyncSystemBuilder.BuildSystem(
				out delegateSystem))
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
			CommonProcedures.WaitForSync(
				delegateSystem?.Invoke());

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

			if (fileSinkSerializer != null)
			{
				var streamStrategy = fileSinkSerializer.Context.SerializationStrategy as IStrategyWithStream;

				streamStrategy?.FinalizeAppend();
			}
		}
	}
}