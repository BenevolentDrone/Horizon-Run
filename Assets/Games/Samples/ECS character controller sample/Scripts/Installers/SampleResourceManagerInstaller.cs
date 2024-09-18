using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.Logging;

using Zenject;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample.Installers
{
	public class SampleResourceManagerInstaller : MonoInstaller
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