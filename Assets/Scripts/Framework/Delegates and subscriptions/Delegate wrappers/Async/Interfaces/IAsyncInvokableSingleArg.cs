using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates
{
	public interface IAsyncInvokableSingleArg
	{
		Task Invoke<TArgument>(
			TArgument value,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		Task Invoke(
			Type valueType,
			object value,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);
	}
}