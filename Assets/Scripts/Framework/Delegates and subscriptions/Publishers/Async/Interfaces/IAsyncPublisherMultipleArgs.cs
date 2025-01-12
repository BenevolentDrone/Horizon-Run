using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Delegates
{
	public interface IAsyncPublisherMultipleArgs
	{
		Task PublishAsync(
			object[] values,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}