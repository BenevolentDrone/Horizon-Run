using System.Threading.Tasks;

using HereticalSolutions.Pools;

namespace HereticalSolutions.AssetImport
{
	public interface IAssetImporterPool
	{
		Task<IPoolElementFacade<AAssetImporter>> PopImporter<TImporter>()
			where TImporter : AAssetImporter;

		Task PushImporter(
			IPoolElementFacade<AAssetImporter> pooledImporter);
	}
}