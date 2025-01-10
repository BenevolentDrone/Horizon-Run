using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates
{
	public interface IAsyncInvokableMultipleArgs
	{
		Task Invoke(
			object[] args,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);
	}
}