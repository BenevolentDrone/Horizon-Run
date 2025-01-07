using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
	public interface IAsyncContainsNestedResources
	{
		Task<IReadOnlyResourceData> GetNestedResourceWhenAvailable(
			int nestedResourceIDHash,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		Task<IReadOnlyResourceData> GetNestedResourceWhenAvailable(
			string nestedResourceID,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);
	}
}