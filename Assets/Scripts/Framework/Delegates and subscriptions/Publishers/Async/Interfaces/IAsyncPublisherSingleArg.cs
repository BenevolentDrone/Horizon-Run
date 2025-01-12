using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Delegates
{
	public interface IAsyncPublisherSingleArg
	{
		Task PublishAsync<TValue>(
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task PublishAsync(
			Type valueType,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}