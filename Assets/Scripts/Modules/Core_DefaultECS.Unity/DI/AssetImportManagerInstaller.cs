/*
using HereticalSolutions.AssetImport;
using HereticalSolutions.AssetImport.Factories;

using HereticalSolutions.Logging;

using Zenject;

namespace HereticalSolutions.Modules.Core_DefaultECS.DI
{
	public class AssetImportManagerInstaller : MonoInstaller
	{
		[Inject]
		private ILoggerResolver loggerResolver;

		public override void InstallBindings()
		{
			IAssetImportManager assetImportManager = AssetImporterFactory.BuildAssetImportManager(loggerResolver);

			Container
				.Bind<IAssetImportManager>()
				.FromInstance(assetImportManager)
				.AsCached();
		}
	}
}
*/