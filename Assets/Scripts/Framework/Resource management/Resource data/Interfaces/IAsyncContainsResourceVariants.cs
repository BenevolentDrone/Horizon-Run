using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
	public interface IAsyncContainsResourceVariants
	{
		Task<IResourceVariantData> GetDefaultVariantWhenAvailable(

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		Task<IResourceVariantData> GetVariantWhenAvailable(
			int variantIDHash,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		Task<IResourceVariantData> GetVariantWhenAvailable(
			string variantID,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);
	}
}