using System;

using HereticalSolutions.Persistence;

using HereticalSolutions.Time;
using HereticalSolutions.Time.Factories;

using HereticalSolutions.Logging;
using HereticalSolutions.Logging.Factories;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

namespace HereticalSolutions.Samples.PersistentTimerWithSerializationSample
{
	public class PersistentTimerWithSerializationSampleBehaviour : MonoBehaviour
	{
		public FileAtAbsolutePathSettings FileAtAbsolutePathSettings;

		public FileAtRelativePathSettings FileAtRelativePathSettings;

		public FileAtTempPathSettings FileAtTempPathSettings;

		public FileAtApplicationDataPathSettings FileAtApplicationDataPathSettings;

		public FileAtPersistentDataPathSettings FileAtPersistentDataPathSettings;


		public IPersistentTimer PersistentTimer { get; private set; }

		private ITickable persistentTimerAsTickable => PersistentTimer as ITickable;

		public ISerializer Serializer { get; set; }

		//Loggers
		private ILoggerResolver loggerResolver;

		public ILoggerResolver LoggerResolver => loggerResolver;

		private ILogger logger;

		public ILogger Logger => logger;


		void Start()
		{
			#region Initiate logger resolver and logger itself

			ILoggerBuilder loggerBuilder = LoggersFactory.BuildLoggerBuilder();

			loggerResolver = loggerBuilder
				.NewLogger()
				.ToggleAllowedByDefault(true)
				//.ToggleLogSource(typeof(PersistentTimerWithSerializationSampleBehaviour), true)
				.AddWrapperBelow(
					LoggersFactory.BuildLoggerWrapperWithSourceTypePrefix())
				.AddWrapperBelow(
					LoggersFactory.BuildLoggerWrapperWithLogTypePrefix())
				.AddWrapperBelow(
					LoggersFactory.BuildLoggerWrapperWithTimestampPrefix(false))
				.AddSink(
					LoggersFactoryUnity.BuildUnityDebugLogSink())
				.Build();

			logger = loggerResolver.GetLogger<PersistentTimerWithSerializationSampleBehaviour>();

			#endregion

			//Initialize timers
			PersistentTimer = TimeFactory.BuildPersistentTimer(
				"AccumulatingPersistentTimer",
				default(TimeSpan),
				loggerResolver);

			PersistentTimer.Accumulate = true;

			PersistentTimer.Start();
		}

		void Update()
		{
			persistentTimerAsTickable.Tick(
				UnityEngine.Time.deltaTime);
		}
	}
}