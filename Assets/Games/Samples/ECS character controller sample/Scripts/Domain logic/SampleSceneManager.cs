using System;

using HereticalSolutions.Entities;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using DefaultEcs;

using Zenject;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleSceneManager : MonoBehaviour
	{
		[Inject]
		private ILoggerResolver loggerResolver;

		[Inject]
		private SampleEntityManager entityManager;

		[SerializeField]
		private string playerEntityPrototypeID;

		private ILogger logger;

		void Awake()
		{
			logger = loggerResolver.GetLogger<SampleSceneManager>();
		}

		public void Start()
		{
			var guid = entityManager.SpawnEntity(playerEntityPrototypeID);

			logger?.Log<SampleSceneManager>(
				$"SPAWNED PLAYER ENTITY WITH GUID: {guid}");
		}
	}
}