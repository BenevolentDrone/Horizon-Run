using HereticalSolutions.Persistence;

using HereticalSolutions.Time;
using HereticalSolutions.Time.Factories;

using HereticalSolutions.Logging;
using HereticalSolutions.Logging.Factories;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

namespace HereticalSolutions.Samples.RuntimeTimerWithSerializationSample
{
	public class RuntimeTimerWithSerializationSampleBehaviour : MonoBehaviour
	{
		public FileAtAbsolutePathSettings FileAtAbsolutePathSettings;

		public FileAtRelativePathSettings FileAtRelativePathSettings;

		public FileAtTempPathSettings FileAtTempPathSettings;

		public FileAtApplicationDataPathSettings FileAtApplicationDataPathSettings;

		public FileAtPersistentDataPathSettings FileAtPersistentDataPathSettings;


		public IRuntimeTimer RuntimeTimer { get; private set; }

		private ITickable runtimeTimerAsTickable => RuntimeTimer as ITickable;

		public ISerializer Serializer { get; set; }

		//Loggers
		private ILoggerResolver loggerResolver;

		public ILoggerResolver LoggerResolver => loggerResolver;

		private ILogger logger;

		public ILogger Logger => logger;

		void Start()
		{
			#region Initiate logger resolver and logger itself

			ILoggerBuilder loggerBuilder = LoggerFactory.BuildLoggerBuilder();

			loggerResolver = loggerBuilder
				.NewLogger()
				.ToggleAllowedByDefault(true)
				//.ToggleLogSource(typeof(RuntimeTimerWithSerializationSampleBehaviour), true)
				.AddWrapperBelow(
					LoggerFactory.BuildLoggerWrapperWithSourceTypePrefix())
				.AddWrapperBelow(
					LoggerFactory.BuildLoggerWrapperWithLogTypePrefix())
				.AddWrapperBelow(
					LoggerFactory.BuildLoggerWrapperWithTimestampPrefix(false))
				.AddSink(
					LoggerFactoryUnity.BuildUnityDebugLogSink())
				.Build();

			logger = loggerResolver.GetLogger<RuntimeTimerWithSerializationSampleBehaviour>();

			#endregion

			//Initialize timer
			RuntimeTimer = TimerFactory.BuildRuntimeTimer(
				"AccumulatingRuntimeTimer",
				0f,
				loggerResolver);

			RuntimeTimer.Accumulate = true;

			RuntimeTimer.Start();
		}

		void Update()
		{
			runtimeTimerAsTickable.Tick(
				UnityEngine.Time.deltaTime);
		}
	}
}