using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Persistence
{
	public interface IAsyncBlockSerializationStrategy
	{
		#region Read

		Task<(bool, TValue)> BlockReadAsync<TValue>(
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<(bool, object)> BlockReadAsync(
			Type valueType,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion

		#region Write

		Task<bool> BlockWriteAsync<TValue>(
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<bool> BlockWriteAsync(
			Type valueType,
			object value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion
	}
}