using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Delegates.Notifiers
{
	public interface IAsyncNotifierSingleArgGeneric<TArgument, TValue>
	{
		Task<TValue> GetValueWhenNotified(
			//Async tail
			AsyncExecutionContext asyncContext,

			TArgument argument = default,
			bool ignoreKey = false);

		Task<Task<TValue>> GetWaitForNotificationTask(
			//Async tail
			AsyncExecutionContext asyncContext,
			
			TArgument argument = default,
			bool ignoreKey = false);

		Task Notify(
			TArgument argument,
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}