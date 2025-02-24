using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Persistence
{
	public interface IAsyncSerializer
		: IHasIODestination,
		  IHasReadWriteControl
	{
		IReadOnlySerializerContext Context { get; }

		#region Serialize

		Task<bool> SerializeAsync<TValue>(
			TValue value,
			
			//Async tail
			AsyncExecutionContext asyncContext);

		Task<bool> SerializeAsync(
			Type valueType,
			object valueObject,

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion

		#region Deserialize

		Task <(bool, TValue)> DeserializeAsync<TValue>(

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<(bool, object)> DeserializeAsync(
			Type valueType,

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion

		#region Populate

		Task<bool> PopulateAsync<TValue>(
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<bool> PopulateAsync(
			Type valueType,
			object valueObject,

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion
	}
}