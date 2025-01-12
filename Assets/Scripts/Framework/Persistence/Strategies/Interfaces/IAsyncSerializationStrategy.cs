using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Persistence
{
	//Courtesy of https://stackoverflow.com/questions/18716928/how-to-write-an-async-method-with-out-parameter
	public interface IAsyncSerializationStrategy
	{
		#region Read

		Task<(bool, TValue)> ReadAsync<TValue>(

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<(bool, object)> ReadAsync(
			Type valueType,

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion

		#region Write

		Task<bool> WriteAsync<TValue>(
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<bool> WriteAsync(
			Type valueType,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion

		#region Append

		Task<bool> AppendAsync<TValue>(
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<bool> AppendAsync(
			Type valueType,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext);

		#endregion
	}
}