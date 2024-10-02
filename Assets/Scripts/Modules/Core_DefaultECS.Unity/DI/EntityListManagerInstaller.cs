using System.Collections.Generic;

using HereticalSolutions.Entities;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

using Zenject;

using DefaultEcs;
using HereticalSolutions.Entities.Factories;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.DI
{
	public class EntityListManagerInstaller : MonoInstaller
	{
		[Inject]
		private ILoggerResolver loggerResolver;
		
		public override void InstallBindings()
		{
			var entityListManager = DefaultECSEntityFactory
				.BuildDefaultECSEntityListManager(
					loggerResolver);

			Container
				.Bind<DefaultECSEntityListManager>()
				.FromInstance(entityListManager)
				.AsCached();
		}
	}
}