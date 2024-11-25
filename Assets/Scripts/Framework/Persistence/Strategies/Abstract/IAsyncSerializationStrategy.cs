using System;
using System.Threading.Tasks;

namespace HereticalSolutions.Persistence
{
	//Courtesy of https://stackoverflow.com/questions/18716928/how-to-write-an-async-method-with-out-parameter
	public interface IAsyncSerializationStrategy
	{
		Type[] AllowedValueTypes { get; }

		#region Read

		Task<(bool, TValue)> ReadAsync<TValue>();

		Task<(bool, object)> ReadAsync(
			Type valueType);

		#endregion

		#region Write

		Task<bool> WriteAsync<TValue>(
			TValue value);

		Task<bool> WriteAsync(
			Type valueType,
			object value);

		#endregion

		#region Append

		Task<bool> AppendAsync<TValue>(
			TValue value);

		Task<bool> AppendAsync(
			Type valueType,
			object value);

		#endregion
	}
}