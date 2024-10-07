using HereticalSolutions.Logging;

using Zenject;

using HereticalSolutions.Modules.Core_DefaultECS.Factories;

namespace HereticalSolutions.Modules.Core_DefaultECS.DI
{
	public class EntityListManagerInstaller : MonoInstaller
	{
		[Inject]
		private ILoggerResolver loggerResolver;
		
		public override void InstallBindings()
		{
			var entityListManager = EntityFactory
				.BuildEntityListManager(
					loggerResolver);

			Container
				.Bind<EntityListManager>()
				.FromInstance(entityListManager)
				.AsCached();
		}
	}
}