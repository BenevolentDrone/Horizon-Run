using System.Collections.Generic;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

using UnityEngine;

using Zenject;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.DI
{
	public class PhysicsManagerInstaller : MonoInstaller
	{
		[Inject]
		private ILoggerResolver loggerResolver;

		[SerializeField]
		private GameObjectPoolSettings physicsBodiesPoolsSettings;

		[SerializeField]
		private Transform poolParentTransform;

		public override void InstallBindings()
		{
			/*
			var physicsBodiesPool = UniversalTemplateUnityGameObjectPoolsFactory.BuildPool(
				Container,
				physicsBodiesPoolsSettings,
				poolParentTransform,
				loggerResolver);

			UnityPhysicsManager physicsManager = new UnityPhysicsManager(
				physicsBodiesPool,
				new Queue<ushort>(),
				RepositoriesFactory.BuildDictionaryRepository<ushort, UnityPhysicsBodyDescriptor>(),
				loggerResolver?.GetLogger<UnityPhysicsManager>());

			Container
				.Bind<UnityPhysicsManager>()
				.FromInstance(physicsManager)
				.AsCached();
			*/
		}
	}
}