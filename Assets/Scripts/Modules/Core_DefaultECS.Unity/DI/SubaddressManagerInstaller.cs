using System;

using HereticalSolutions.Entities;

using HereticalSolutions.Entities.Factories;

using HereticalSolutions.Logging;

using Zenject;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.DI
{
	public class SubaddressManagerInstaller : MonoInstaller
	{
		[Inject]
		private ILoggerResolver loggerResolver;

		public override void InstallBindings()
		{
			var subaddressManager = DefaultECSEntityFactory.BuildSubaddressManager(
				loggerResolver?.GetLogger<ISubaddressManager>());

			Container
				.Bind<ISubaddressManager>()
				.FromInstance(subaddressManager)
				.AsCached();
		}
	}
}