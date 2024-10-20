using System;
using System.Globalization;

using HereticalSolutions.Systems;
using HereticalSolutions.Systems.Factories;

using HereticalSolutions.Logging;
using HereticalSolutions.Logging.Factories;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

namespace HereticalSolutions.Samples.SystemAndProceduresSample
{
	public class SystemAndProceduresSampleBehaviour : MonoBehaviour
	{
		private bool isQuittingApplication;

		private ILoggerResolver loggerResolver;

		private ILogger logger;

		void Start()
		{
			#region Initiate logger resolver and logger itself

			string dateTimeNow = DateTime.Now.ToString("s", CultureInfo.InvariantCulture);

			dateTimeNow = dateTimeNow.Replace('T', ' ');

			dateTimeNow = dateTimeNow.Replace(':', '-');

			string logFileName = dateTimeNow;

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

				.Branch(
					new[]
					{
						LoggersFactory.BuildFileSink(
							$"{Application.dataPath}/../",
							$"Runtime logs/{logFileName}.log",
							(ILoggerResolver)loggerBuilder)
					})

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

			logger = loggerResolver.GetLogger<SystemAndProceduresSampleBehaviour>();

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
							"MAIN THREAD PROCEDURE 1 DELEGATE INVOKED");
					}));

			delegateSystemBuilder.TryAddBeforeStage(
				SystemBuilderExtensions.GetStageFinishNodeID("MAIN THREAD PROCESS"),
				SystemFactory.BuildProcedureNode<Action>(
					() =>
					{
						logger.Log(
							"MAIN THREAD PROCEDURE 2 DELEGATE INVOKED");
					}));

			//Add the thread 1 procedures
			var thread1Procedure1 = SystemFactory.BuildProcedureNode<Action>(
				() =>
				{
					logger.Log(
						"THREAD 1 PROCEDURE 1 DELEGATE INVOKED");
				});

			delegateSystemBuilder.TryAddAfterStage(
				SystemBuilderExtensions.GetStageStartNodeID("THREAD 1 PROCESS"),
				thread1Procedure1);

			var thread1Procedure2 = SystemFactory.BuildProcedureNode<Action>(
				() =>
				{
					logger.Log(
						"THREAD 1 PROCEDURE 2 DELEGATE INVOKED");
				});

			delegateSystemBuilder.TryAddAfterNode(
				thread1Procedure1,
				thread1Procedure2);

			var thread1Procedure3 = SystemFactory.BuildProcedureNode<Action>(
				() =>
				{
					logger.Log(
						"THREAD 1 PROCEDURE 3 DELEGATE INVOKED");
				});

			delegateSystemBuilder.TryAddAfterNode(
				thread1Procedure2,
				thread1Procedure3);

			var thread1Procedure4 = SystemFactory.BuildProcedureNode<Action>(
				() =>
				{
					logger.Log(
						"THREAD 1 PROCEDURE 4 DELEGATE INVOKED");
				});

			delegateSystemBuilder.TryAddAfterNode(
				thread1Procedure3,
				thread1Procedure4);

			var thread1Procedure5 = SystemFactory.BuildProcedureNode<Action>(
				() =>
				{
					logger.Log(
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
						"THREAD 2 PROCEDURE 1 DELEGATE INVOKED");
				});

			delegateSystemBuilder.TryAddBeforeNode(
				thread2FinishNode,
				thread2Procedure1);

			var thread2Procedure2 = SystemFactory.BuildProcedureNode<Action>(
				() =>
				{
					logger.Log(
						"THREAD 2 PROCEDURE 2 DELEGATE INVOKED");
				});

			delegateSystemBuilder.TryAddBeforeNode(
				thread2FinishNode,
				thread2Procedure2);


			//Link nodes from different threads for additional assertions
			delegateSystemBuilder.TryLinkNodes(
				thread1Procedure5,
				thread2Procedure2);

			//Validate the system
			if (!delegateSystemBuilder.ValidateSystem())
			{
				logger.LogError(
					"Failed to validate the system");

				return;
			}

			//Build the system
			if (!delegateSystemBuilder.BuildSystem(
				out var delegateSystem))
			{
				logger.LogError(
					"Failed to build the system");

				return;
			}

			//Run the system
			delegateSystem?.Invoke();
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