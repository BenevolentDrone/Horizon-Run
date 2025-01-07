using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	//Courtesy of https://stackoverflow.com/questions/18716928/how-to-write-an-async-method-with-out-parameter
	public interface IAsyncSerializationStrategy
	{
		#region Read

		Task<(bool, TValue)> ReadAsync<TValue>(

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		Task<(bool, object)> ReadAsync(
			Type valueType,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		#endregion

		#region Write

		Task<bool> WriteAsync<TValue>(
			TValue value,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		Task<bool> WriteAsync(
			Type valueType,
			object value,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		#endregion

		#region Append

		Task<bool> AppendAsync<TValue>(
			TValue value,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		Task<bool> AppendAsync(
			Type valueType,
			object value,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		#endregion
	}
}