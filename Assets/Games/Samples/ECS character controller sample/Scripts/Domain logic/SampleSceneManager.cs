using HereticalSolutions.Modules.Core_DefaultECS;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleSceneManager : MonoBehaviour
	{
		[Inject]
		private ILoggerResolver loggerResolver;

		[Inject]
		private EntityManager entityManager;

		[SerializeField]
		private string playerEntityPrototypeID;

		private ILogger logger;

		void Awake()
		{
			logger = loggerResolver.GetLogger<SampleSceneManager>();
		}

		public void Start()
		{
			entityManager.SpawnEntity(
				out var guid,
				playerEntityPrototypeID);

			logger?.Log<SampleSceneManager>(
				$"SPAWNED PLAYER ENTITY WITH GUID: {guid}");
		}
	}
}