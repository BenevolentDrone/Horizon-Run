using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using Zenject;

namespace HereticalSolutions.HorizonRun.DI
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