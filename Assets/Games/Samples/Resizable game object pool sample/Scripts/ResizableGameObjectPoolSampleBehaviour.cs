using System.Collections.Generic;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Logging;
using HereticalSolutions.Logging.Factories;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

namespace HereticalSolutions.Samples.ResizableGameObjectPoolSample
{
	public class ResizableGameObjectPoolSampleBehaviour : MonoBehaviour
	{
		[Header("Settings")]

		[SerializeField]
		private SamplePoolSettings poolSettings;

		
		private IManagedPool<GameObject> gameObjectPool;


		private List<IPoolElementFacade<GameObject>> poppedElements;


		private WorldPositionArgument worldPositionArgument;

		private IPoolPopArgument[] argumentsCache;


		private ILoggerResolver loggerResolver;

		private ILogger logger;


		void Start()
		{
			#region Initiate logger resolver and logger itself

			ILoggerBuilder loggerBuilder = LoggersFactory.BuildLoggerBuilder();

			loggerBuilder
				.ToggleAllowedByDefault(false)
				.AddSink(
					LoggersFactoryUnity.BuildUnityDebugLogSink())
				.Wrap(
					LoggersFactory.BuildLoggerWrapperWithLogTypePrefix(
						loggerBuilder.CurrentLogger))
				.Wrap(
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

			poppedElements = new List<IPoolElementFacade<GameObject>>(100);

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
			if (poppedElements.Count == 0)
				return;

			var randomIndex = UnityEngine.Random.Range(0, poppedElements.Count);

			var activeElement = poppedElements[randomIndex];

			//Both options should work the same way
			//nonAllocPool.Push(activeElement.Value);
			activeElement.Push();

			poppedElements.RemoveAt(randomIndex);
		}

		private void PopRandomElement()
		{
			worldPositionArgument.Position = new Vector3(
				UnityEngine.Random.Range(-5f, 5f),
				UnityEngine.Random.Range(-5f, 5f),
				UnityEngine.Random.Range(-5f, 5f));

			var value = gameObjectPool.Pop(argumentsCache);

			poppedElements.Add(value);
		}
	}
}