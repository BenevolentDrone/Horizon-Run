/*
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.ResourceManagement
{
	public interface IContainsDependencyResources
	{
		Task<IReadOnlyResourceStorageHandle> LoadDependency(
			string path,
			string variantID = null,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<IReadOnlyResourceData> GetDependencyResource(
			string path,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}
*/