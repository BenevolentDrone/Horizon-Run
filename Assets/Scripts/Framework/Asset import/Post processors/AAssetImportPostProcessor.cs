using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.AssetImport
{
	public abstract class AAssetImportPostProcessor
	{
		public virtual async Task OnImport(
			IResourceVariantData variantData,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
		{
			throw new NotImplementedException();
		}
	}
}