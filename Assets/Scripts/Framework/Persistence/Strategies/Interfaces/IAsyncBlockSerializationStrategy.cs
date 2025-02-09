using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Persistence
{
	public interface IAsyncBlockSerializationStrategy
	{
		#region Read

		Task<(bool, TValue)> ReadBlockAsync<TValue>(
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<(bool, object)> ReadBlockAsync(
			Type valueType,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion

		#region Write

		Task<bool> WriteBlockAsync<TValue>(
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<bool> WriteBlockAsync(
			Type valueType,
			object value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion
	}
}