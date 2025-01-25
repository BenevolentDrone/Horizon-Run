using System;
using System.Collections.Generic;

using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Notifiers
{
	public class AsyncNotifierSingleArgGeneric<TArgument, TValue>
		: IAsyncNotifierSingleArgGeneric<TArgument, TValue>
		  where TArgument : IEquatable<TArgument>
	{
		private readonly List<NotifyRequestSingleArgGeneric<TArgument, TValue>> requests;

		private readonly SemaphoreSlim semaphore;

		private readonly ILogger logger;

		public AsyncNotifierSingleArgGeneric(
			List<NotifyRequestSingleArgGeneric<TArgument, TValue>> requests,
			SemaphoreSlim semaphore,
			ILogger logger)
		{
			this.requests = requests;

			this.semaphore = semaphore;

			this.logger = logger;
		}

		#region IAsyncNotifierSingleArgGeneric

		public async Task<TValue> GetValueWhenNotified(
			//Async tail
			AsyncExecutionContext asyncContext,

			TArgument argument = default,
			bool ignoreKey = false)
		{
			TaskCompletionSource<TValue> completionSource = new TaskCompletionSource<TValue>();

			var request = new NotifyRequestSingleArgGeneric<TArgument, TValue>(
				argument,
				ignoreKey,
				completionSource);


			await semaphore.WaitAsync();

			logger?.Log(
				GetType(),
				$"GetValueWhenNotified SEMAPHORE ACQUIRED");

			requests.Add(request);

			semaphore.Release();

			logger?.Log(
				GetType(),
				$"GetValueWhenNotified SEMAPHORE RELEASED");


			var task = completionSource
				.Task;

			await task;
				//.ConfigureAwait(false);

			await task
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			return task.Result;
		}

		public async Task<Task<TValue>> GetWaitForNotificationTask(
			//Async tail
			AsyncExecutionContext asyncContext,

			TArgument argument = default,
			bool ignoreKey = false)
		{
			TaskCompletionSource<TValue> completionSource = new TaskCompletionSource<TValue>();

			var request = new NotifyRequestSingleArgGeneric<TArgument, TValue>(
				argument,
				ignoreKey,
				completionSource);


			await semaphore.WaitAsync();

			logger?.Log(
				GetType(),
				$"GetWaitForNotificationTask SEMAPHORE ACQUIRED");

			requests.Add(request);

			semaphore.Release();

			logger?.Log(
				GetType(),
				$"GetWaitForNotificationTask SEMAPHORE RELEASED");


			return GetValueFromCompletionSource(
				completionSource,
				
				asyncContext);
		}

		private async Task<TValue> GetValueFromCompletionSource(
			TaskCompletionSource<TValue> completionSource,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var task = completionSource
				.Task;

			await task;
				//.ConfigureAwait(false);

			await task
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			return task.Result;
		}

		public async Task Notify(
			TArgument argument,
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await semaphore.WaitAsync();

			logger?.Log(
				GetType(),
				$"Notify SEMAPHORE ACQUIRED");

			for (int i = requests.Count - 1; i >= 0; i--)
			{
				var request = requests[i];

				if (request.IgnoreKey
					|| EqualityComparer<TArgument>.Default.Equals(request.Key, argument)) //if (request.Key.Equals(argument)) - bad for strings
				{
					requests.RemoveAt(i);

					request.TaskCompletionSource.TrySetResult(value);					
				}
			}

			semaphore.Release();

			logger?.Log(
				GetType(),
				$"Notify SEMAPHORE RELEASED");
		}

		#endregion
	}
}