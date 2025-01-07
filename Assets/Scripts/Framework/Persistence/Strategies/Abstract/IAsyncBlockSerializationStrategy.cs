using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public interface IAsyncBlockSerializationStrategy
	{
		#region Read

		Task<(bool, TValue)> BlockReadAsync<TValue>(
			int blockOffset,
			int blockSize,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		Task<(bool, object)> BlockReadAsync(
			Type valueType,
			int blockOffset,
			int blockSize,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		#endregion

		#region Write

		Task<bool> BlockWriteAsync<TValue>(
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		Task<bool> BlockWriteAsync(
			Type valueType,
			object value,
			int blockOffset,
			int blockSize,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		#endregion
	}
}