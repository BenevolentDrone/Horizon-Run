using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Persistence
{
	public interface IAsyncBlockSerializer
		: IHasIODestination,
		  IHasReadWriteControl
	{
		IReadOnlySerializerContext Context { get; }

		#region Serialize

		Task<bool> SerializeBlockAsync<TValue>(
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<bool> SerializeBlockAsync(
			Type valueType,
			object valueObject,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion

		#region Deserialize

		Task<(bool, TValue)> DeserializeBlockAsync<TValue>(
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<(bool, object)> DeserializeBlockAsync(
			Type valueType,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion

		#region Populate

		Task<bool> PopulateBlockAsync<TValue>(
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<bool> PopulateBlockAsync(
			Type valueType,
			object valueObject,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion
	}
}