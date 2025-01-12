using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Delegates.Notifiers
{
	public interface IAsyncNotifierSingleArgGeneric<TArgument, TValue>
	{
		Task<TValue> GetValueWhenNotified(
			TArgument argument = default,
			bool ignoreKey = false,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<Task<TValue>> GetWaitForNotificationTask(
			TArgument argument = default,
			bool ignoreKey = false,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task Notify(
			TArgument argument,
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}