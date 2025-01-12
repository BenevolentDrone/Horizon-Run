using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Delegates
{
	public interface IAsyncInvokableSingleArg
	{
		Type ValueType { get; }

		Task InvokeAsync<TArgument>(
			TArgument value,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task InvokeAsync(
			Type valueType,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}