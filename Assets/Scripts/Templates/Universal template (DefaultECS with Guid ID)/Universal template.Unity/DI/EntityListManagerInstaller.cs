using System.Collections.Generic;

using HereticalSolutions.Entities;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

using Zenject;

using DefaultEcs;

namespace HereticalSolutions.Templates.Universal.Unity.DI
{
	public class EntityListManagerInstaller : MonoInstaller
	{
		[Inject]
		private ILoggerResolver loggerResolver;
		
		public override void InstallBindings()
		{
			DefaultECSEntityListManager entityListManager = new DefaultECSEntityListManager(
				new Queue<ushort>(),
				RepositoriesFactory.BuildDictionaryRepository<ushort, List<Entity>>(),
				loggerResolver?.GetLogger<DefaultECSEntityListManager>());

			Container
				.Bind<DefaultECSEntityListManager>()
				.FromInstance(entityListManager)
				.AsCached();
		}
	}
}