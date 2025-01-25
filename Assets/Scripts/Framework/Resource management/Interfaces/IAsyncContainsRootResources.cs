/*
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.ResourceManagement
{
	public interface IAsyncContainsRootResources
	{
		#region Get

		Task<IReadOnlyResourceData> GetRootResourceWhenAvailable(
			int rootResourceIDHash,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<IReadOnlyResourceData> GetRootResourceWhenAvailable(
			string rootResourceID,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<IReadOnlyResourceData> GetResourceWhenAvailable(
			int[] resourcePathPartHashes,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<IReadOnlyResourceData> GetResourceWhenAvailable(
			string[] resourcePathParts,

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion

		#region Get default

		Task<IResourceVariantData> GetDefaultRootResourceWhenAvailable(
			int rootResourceIDHash,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<IResourceVariantData> GetDefaultRootResourceWhenAvailable(
			string rootResourceID,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<IResourceVariantData> GetDefaultResourceWhenAvailable(
			int[] resourcePathPartHashes,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<IResourceVariantData> GetDefaultResourceWhenAvailable(
			string[] resourcePathParts,

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion
	}
}
*/