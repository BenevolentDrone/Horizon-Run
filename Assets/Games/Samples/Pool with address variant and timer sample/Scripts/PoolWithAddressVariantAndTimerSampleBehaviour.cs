using UnityEngine;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Synchronization;
using HereticalSolutions.Synchronization.Factories;

using HereticalSolutions.Time;
using HereticalSolutions.Time.Factories;

using HereticalSolutions.Logging;
using HereticalSolutions.Logging.Factories;
using ILogger = HereticalSolutions.Logging.ILogger;

namespace HereticalSolutions.Samples.PoolWithAddressVariantAndTimerSample
{
	public class PoolWithAddressVariantAndTimerSampleBehaviour : MonoBehaviour
	{
		[Header("Settings")]

		[SerializeField]
		private SamplePoolSettings poolSettings;
	
		[SerializeField]
		private Transform poolParent;


		private ITimeManager timeManager;

		private ITickable timeManagerAsTickable;


		private ITimerManager timerManager;

		
		private IManagedPool<GameObject> gameObjectPool;


		private ILoggerResolver	loggerResolver;

		private ILogger logger;


		private AddressArgument addressArgument;
		
		private WorldPositionArgument worldPositionArgument;

		private IPoolPopArgument[] argumentsCache;


		private int[][] addressHashesCache;

		void Start()
		{
			#region Initiate logger resolver and logger itself

			ILoggerBuilder loggerBuilder = LoggerFactory.BuildLoggerBuilder();

			loggerResolver = loggerBuilder
				.NewLogger()
				.ToggleAllowedByDefault(false)
				.ToggleLogSource(typeof(PoolWithAddressVariantAndTimerSampleBehaviour), true)
				.AddWrapperBelow(
					LoggerFactory.BuildLoggerWrapperWithSourceTypePrefix())
				.AddWrapperBelow(
					LoggerFactory.BuildLoggerWrapperWithLogTypePrefix())
				.AddWrapperBelow(
					LoggerFactory.BuildLoggerWrapperWithTimestampPrefix(false))
				.AddSink(
					LoggerFactoryUnity.BuildUnityDebugLogSink())
				.Build();

			logger = loggerResolver.GetLogger<PoolWithAddressVariantAndTimerSampleBehaviour>();

			#endregion

			#region Initiate time manager and Update() loop

			timeManager = TimerFactory.BuildTimeManager(loggerResolver);

			timeManagerAsTickable = timeManager as ITickable;

			var synchronizableRepository = timeManager as ISynchronizableGenericArgRepository<float>;

			synchronizableRepository.AddSynchronizable(
				SynchronizationFactory.BuildSynchronizationContextGeneric<float>(
					"Update",
					canBeToggled: true,
					active: true,
					canScale: true,
					scale: 1f,
					scaleDeltaDelegate: (value, scale) => value * scale,
					loggerResolver: loggerResolver));

			var synchronizationProviderRepository = timeManager as  ISynchronizationProviderRepository;

			synchronizationProviderRepository.TryGetProvider(
				"Update",
				out var updateProvider);

			#endregion

			#region Initiate timer manager

			timerManager = TimerFactory.BuildTimerManager(
				"PoolWithAddressVariantAndTimerSampleBehaviour",
				updateProvider,
				false,
				loggerResolver);

			#endregion

			#region Initiate pool and arguments

			gameObjectPool = SamplePoolFactory.BuildPool(
				null,
				poolSettings,
				timerManager,
				poolParent,
				loggerResolver);

			argumentsCache = new ArgumentBuilder()
				.Add<WorldPositionArgument>(out worldPositionArgument)
				.Add<AddressArgument>(out addressArgument)
				.Build();

			addressHashesCache = new int[poolSettings.Elements.Length][];

			for (int i = 0; i < addressHashesCache.Length; i++)
				addressHashesCache[i] = poolSettings.Elements[i].Name.AddressToHashes();

			#endregion
		}

		// Update is called once per frame
		void Update()
		{
			timeManagerAsTickable.Tick(UnityEngine.Time.deltaTime);

			bool doSomething = UnityEngine.Random.Range(0f, 1f) < 0.1f;

			if (doSomething)
			{
				PopRandomElement();
			}
		}

		private void PopRandomElement()
		{
			worldPositionArgument.Position = new Vector3(
				UnityEngine.Random.Range(-5f, 5f),
				UnityEngine.Random.Range(-5f, 5f),
				UnityEngine.Random.Range(-5f, 5f));

			var address = addressHashesCache[UnityEngine.Random.Range(0, addressHashesCache.Length)];

			addressArgument.AddressHashes = address;
			
			gameObjectPool.Pop(argumentsCache);
		}
	}
}