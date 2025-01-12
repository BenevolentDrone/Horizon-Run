using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Delegates
{
	public interface IAsyncPublisherNoArgs
	{
		Task PublishAsync(

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}