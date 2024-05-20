using UnityEngine;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Collections;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;
using HereticalSolutions.Pools.Arguments;

using HereticalSolutions.Logging;
using HereticalSolutions.Logging.Factories;
using ILogger = HereticalSolutions.Logging.ILogger;

namespace HereticalSolutions.Samples.ResizableGameObjectPoolSample
{
	public class ResizableGameObjectPoolSampleBehaviour : MonoBehaviour
	{
		[Header("Settings")]

		[SerializeField]
		private SamplePoolSettings poolSettings;

		
		private INonAllocDecoratedPool<GameObject> gameObjectPool;


		private INonAllocPool<IPoolElement<GameObject>> poppedElements;

		private IIndexable<IPoolElement<IPoolElement<GameObject>>> poppedElementsAsIndexable;


		private WorldPositionArgument worldPositionArgument;

		private IPoolDecoratorArgument[] argumentsCache;


		private ILoggerResolver loggerResolver;

		private ILogger logger;


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
				.ToggleLogSource(typeof(ResizableGameObjectPoolSampleBehaviour), true);

			loggerResolver = (ILoggerResolver)loggerBuilder;

			logger = loggerResolver.GetLogger<ResizableGameObjectPoolSampleBehaviour>();

			#endregion

			#region Initiate pool and arguments

			gameObjectPool = SamplePoolFactory.BuildPool(
				null,
				poolSettings,
				loggerResolver);

			argumentsCache = new ArgumentBuilder()
				.Add<WorldPositionArgument>(out worldPositionArgument)
				.Build();

			#endregion

			#region Initiate popped elements pool

			poppedElements = PoolsFactory.BuildPackedArrayPool<IPoolElement<GameObject>>(
				PoolsFactory.BuildPoolElementAllocationCommand<IPoolElement<GameObject>>(
					new AllocationCommandDescriptor
					{
						Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,
						Amount = 100
					},
					AllocationsFactory.NullAllocationDelegate<IPoolElement<GameObject>>,
					new[]
					{
						PoolsFactory.BuildIndexedMetadataDescriptor()
					}),
					loggerResolver);

			poppedElementsAsIndexable = (IIndexable<IPoolElement<IPoolElement<GameObject>>>)poppedElements;

			#endregion
		}

		// Update is called once per frame
		void Update()
		{
			bool doSomething = UnityEngine.Random.Range(0f, 1f) < 0.1f;

			if (doSomething)
			{
				bool push = UnityEngine.Random.Range(0f, 1f) < 0.5f;

				if (push)
				{
					PushRandomElement();
				}
				else
				{
					PopRandomElement();
				}
			}
		}

		private void PushRandomElement()
		{
			if (poppedElementsAsIndexable.Count == 0)
				return;

			var randomIndex = UnityEngine.Random.Range(0, poppedElementsAsIndexable.Count);

			var activeElement = poppedElementsAsIndexable[randomIndex];

			//Both options should work the same way
			//nonAllocPool.Push(activeElement.Value);
			activeElement.Value.Push();

			poppedElements.Push(activeElement);
		}

		private void PopRandomElement()
		{
			worldPositionArgument.Position = new Vector3(
				UnityEngine.Random.Range(-5f, 5f),
				UnityEngine.Random.Range(-5f, 5f),
				UnityEngine.Random.Range(-5f, 5f));

			var value = gameObjectPool.Pop(argumentsCache);

			var activeElement = poppedElements.Pop();

			activeElement.Value = value;
		}
	}
}