using UnityEngine;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;
using HereticalSolutions.Pools.Arguments;

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

		
		private INonAllocDecoratedPool<GameObject> gameObjectPool;


		private ILoggerResolver	loggerResolver;

		private ILogger logger;


		private AddressArgument addressArgument;
		
		private WorldPositionArgument worldPositionArgument;

		private IPoolDecoratorArgument[] argumentsCache;


		private int[][] addressHashesCache;

		void Start()
		{
			#region Initiate logger resolver and logger itself

			ILoggerBuilder loggerBuilder = LoggersFactory.BuildLoggerBuilder();

			loggerBuilder
				.ToggleAllowedByDefault(false)
				.AddOrWrap(
					LoggersFactoryUnity.BuildUnityDebugLogger())
				.AddOrWrap(
					LoggersFactory.BuildLoggerWrapperWithLogTypePrefix(
						loggerBuilder.CurrentLogger))
				.AddOrWrap(
					LoggersFactory.BuildLoggerWrapperWithSourceTypePrefix(
						loggerBuilder.CurrentLogger))
				.ToggleLogSource(typeof(PoolWithAddressVariantAndTimerSampleBehaviour), true);

			loggerResolver = (ILoggerResolver)loggerBuilder;

			logger = loggerResolver.GetLogger<PoolWithAddressVariantAndTimerSampleBehaviour>();

			#endregion

			#region Initiate time manager and Update() loop

			timeManager = TimeFactory.BuildTimeManager(loggerResolver);

			timeManagerAsTickable = timeManager as ITickable;

			var synchronizablesRepository = timeManager as ISynchronizablesGenericArgRepository<float>;

			synchronizablesRepository.AddSynchronizable(
				SynchronizationFactory.BuildSynchronizationContextGeneric<float>(
					"Update",
					canBeToggled: true,
					active: true,
					canScale: true,
					scale: 1f,
					scaleDeltaDelegate: (value, scale) => value * scale,
					loggerResolver: loggerResolver));

			var synchronizationProvidersRepository = timeManager as  ISynchronizationProvidersRepository;

			synchronizationProvidersRepository.TryGetProvider(
				"Update",
				out var updateProvider);

			#endregion

			#region Initiate timer manager

			timerManager = TimeFactory.BuildTimerManager(
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