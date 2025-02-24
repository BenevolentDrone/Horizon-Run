using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Persistence
{
	public interface IAsyncFormatSerializer
	{
		#region Serialize

		Task<bool> SerializeAsync<TValue>(
			ISerializationCommandContext context,
			TValue value,
			
			//Async tail
			AsyncExecutionContext asyncContext);

		Task<bool> SerializeAsync(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject,
			
			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion

		#region Deserialize

		Task<(bool, TValue)> DeserializeAsync<TValue>(
			ISerializationCommandContext context,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<(bool, object)> DeserializeAsync(
			Type valueType,
			ISerializationCommandContext context,

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion

		#region Populate

		Task<bool> PopulateAsync<TValue>(
			ISerializationCommandContext context,
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<bool> PopulateAsync(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject,

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion
	}
}