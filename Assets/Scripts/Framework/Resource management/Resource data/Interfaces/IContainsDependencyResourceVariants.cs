using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
	public interface IContainsDependencyResourceVariants
	{
		Task<IResourceVariantData> GetDependencyResourceVariant(
			string variantID = null,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);
	}
}