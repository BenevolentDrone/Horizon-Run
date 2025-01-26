/*
using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using Zenject;

namespace HereticalSolutions.Modules.Core_DefaultECS.DI
{
	public class ResourceManagerInstaller : MonoInstaller
	{
		//[Inject]
		//private ILoggerResolver loggerResolver;

		public override void InstallBindings()
		{
			var runtimeResourceManager = ResourceManagementFactory.BuildRuntimeResourceManager();

			Container
				.Bind<IRuntimeResourceManager>()
				.FromInstance(runtimeResourceManager)
				.AsCached();
		}
	}
}
*/