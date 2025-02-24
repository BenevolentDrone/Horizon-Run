using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Persistence
{
	public interface IAsyncBlockFormatSerializer
	{
		#region Serialize

		Task<bool> SerializeBlockAsync<TValue>(
			ISerializationCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<bool> SerializeBlockAsync(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion

		#region Deserialize

		Task<(bool, TValue)> DeserializeBlockAsync<TValue>(
			ISerializationCommandContext context,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<(bool, object)> DeserializeBlockAsync(
			Type valueType,
			ISerializationCommandContext context,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion

		#region Populate

		Task<bool> PopulateBlockAsync<TValue>(
			ISerializationCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<bool> PopulateBlockAsync(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion
	}
}