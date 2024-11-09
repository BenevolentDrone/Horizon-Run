using System;
using System.Threading.Tasks;

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

		private Func<Task> delegateSystem;


		void Start()
		{
			#region Initiate logger resolver and logger itself

			//string dateTimeNow = DateTime.Now.ToString("s", CultureInfo.InvariantCulture);
			//
			//dateTimeNow = dateTimeNow.Replace('T', '_');
			//
			//dateTimeNow = dateTimeNow.Replace(':', '-');
			//
			//string logFileName = dateTimeNow;

			ILoggerBuilder loggerBuilder = LoggersFactory.BuildLoggerBuilder();

			loggerBuilder
				.ToggleAllowedByDefault(
					true)

				// Output

				.AddSink(
					LoggersFactoryUnity.BuildUnityDebugLogSink())

				//Toggling

				.Wrap(
					LoggersFactory.BuildLoggerWrapperWithToggling(
						loggerBuilder.CurrentLogger,
						true,
						true,
						true,
						true))

				// Recursion prevention prefix

				.Wrap(
					LoggersFactory.BuildLoggerWrapperWithRecursionPreventionPrefix(
						loggerBuilder.CurrentLogger))

				// Logging to file
				
				//.Branch(
				//	new[]
				//	{
				//		LoggersFactory.BuildFileSink(
				//			$"{Application.dataPath}/../",
				//			$"Runtime logs/{logFileName}.log",
				//			(ILoggerResolver)loggerBuilder)
				//	})

				//Prefixes

				.Wrap(
					LoggersFactory.BuildLoggerWrapperWithTimestampPrefix(
						false,
						loggerBuilder.CurrentLogger))
				.Wrap(
					LoggersFactory.BuildLoggerWrapperWithLogTypePrefix(
						loggerBuilder.CurrentLogger))
				.Wrap(
					LoggersFactory.BuildLoggerWrapperWithSourceTypePrefix(
						loggerBuilder.CurrentLogger))
				.Wrap(
					LoggersFactory.BuildLoggerWrapperWithThreadIndexPrefix(
						loggerBuilder.CurrentLogger))

				//Thread safety

				.Wrap(
					LoggersFactory.BuildLoggerWrapperWithSemaphoreSlim(
						loggerBuilder.CurrentLogger))

				//Recursion prevention gate

				.Wrap(
					LoggersFactory.BuildLoggerWrapperWithRecursionPreventionGate(
						loggerBuilder.CurrentLogger));

			loggerResolver = (ILoggerResolver)loggerBuilder;

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
		}
	}
}