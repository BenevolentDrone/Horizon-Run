using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;

using System.Text;

using HereticalSolutions.Logging;

namespace HereticalSolutions
{
	public static class TaskExtensions
	{
		private const bool AWAIT_IN_THE_SAME_THREAD = true;

		//For some reason the previous version I used (that utilized ContinueWith(..., TaskContinuationOptions.OnlyOnFaulted) method chain) would throw a TaskCanceledException in random places in the code
		//As stated over here, it was because TaskContinuationOptions.OnlyOnFaulted is not valid for multi-task continuations
		//https://stackoverflow.com/questions/28633871/taskcanceledexception-with-continuewith
		//So I've changed it to the option provided here:
		//https://stackoverflow.com/a/58469206

		#region Task

		public static ConfiguredTaskAwaitable LogExceptions(this Task task)
		{
			return task
				.ContinueWith(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						Console.WriteLine($"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");
					})
				.ConfigureAwait(AWAIT_IN_THE_SAME_THREAD);
		}

		public static ConfiguredTaskAwaitable ThrowExceptions(this Task task)
		{
			return task
				.ContinueWith(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						throw new Exception($"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");
					})
				.ConfigureAwait(AWAIT_IN_THE_SAME_THREAD);
		}

		public static ConfiguredTaskAwaitable LogExceptions<TSource>(
			this Task task,
			ILogger logger)
		{
			return task
				.ContinueWith(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						logger?.LogError<TSource>(
							$"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");
					})
				.ConfigureAwait(AWAIT_IN_THE_SAME_THREAD);
		}

		public static ConfiguredTaskAwaitable ThrowExceptions<TSource>(
			this Task task,
			ILogger logger)
		{
			return task
				.ContinueWith(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						throw new Exception(
							logger.TryFormat<TSource>(
								$"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}"));
					})
				.ConfigureAwait(AWAIT_IN_THE_SAME_THREAD);
		}

		public static ConfiguredTaskAwaitable LogExceptions(
			this Task task,
			Type logSource,
			ILogger logger)
		{
			return task
				.ContinueWith(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						logger?.LogError(
							logSource,
							$"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");
					})
				.ConfigureAwait(AWAIT_IN_THE_SAME_THREAD);
		}

		public static ConfiguredTaskAwaitable ThrowExceptions(
			this Task task,
			Type logSource,
			ILogger logger)
		{
			return task
				.ContinueWith(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						throw new Exception(
							logger.TryFormat(
								logSource,
								$"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}"));
					})
				.ConfigureAwait(AWAIT_IN_THE_SAME_THREAD);
		}

		#endregion

		#region Task<T>

		public static ConfiguredTaskAwaitable<T> LogExceptions<T>(this Task<T> task)
		{
			return task
				.ContinueWith<T>(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return targetTask.Result;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						Console.WriteLine($"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");

						return default;
					})
				.ConfigureAwait(AWAIT_IN_THE_SAME_THREAD);
		}

		public static ConfiguredTaskAwaitable<T> ThrowExceptions<T>(this Task<T> task)
		{
			return task
				.ContinueWith<T>(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return targetTask.Result;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						throw new Exception($"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");
					})
				.ConfigureAwait(AWAIT_IN_THE_SAME_THREAD);
		}

		public static ConfiguredTaskAwaitable<T> LogExceptions<T, TSource>(
			this Task<T> task,
			ILogger logger)
		{
			return task
				.ContinueWith<T>(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return targetTask.Result;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						logger?.LogError<TSource>(
							$"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");

						return default;
					})
				.ConfigureAwait(AWAIT_IN_THE_SAME_THREAD);
		}

		public static ConfiguredTaskAwaitable<T> ThrowExceptions<T, TSource>(
			this Task<T> task,
			ILogger logger)
		{
			return task
				.ContinueWith<T>(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return targetTask.Result;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						throw new Exception(
							logger.TryFormat<TSource>(
								$"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}"));
					})
				.ConfigureAwait(AWAIT_IN_THE_SAME_THREAD);
		}

		public static ConfiguredTaskAwaitable<T> LogExceptions<T>(
			this Task<T> task,
			Type logSource,
			ILogger logger)
		{
			return task
				.ContinueWith<T>(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return targetTask.Result;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						logger?.LogError(
							logSource,
							$"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}");

						return default;
					})
				.ConfigureAwait(AWAIT_IN_THE_SAME_THREAD);
		}

		public static ConfiguredTaskAwaitable<T> ThrowExceptions<T>(
			this Task<T> task,
			Type logSource,
			ILogger logger)
		{
			return task
				.ContinueWith<T>(
					targetTask =>
					{
						if (!targetTask.IsFaulted)
						{
							return targetTask.Result;
						}

						StringBuilder stringBuilder = new StringBuilder();

						foreach (var innerException in targetTask.Exception.InnerExceptions)
						{
							stringBuilder.Append(innerException.ToString());
							stringBuilder.Append('\n');
						}

						throw new Exception(
							logger.TryFormat(
								logSource,
								$"{targetTask.Exception.Message} INNER EXCEPTIONS:\n{stringBuilder.ToString()}"));
					})
				.ConfigureAwait(AWAIT_IN_THE_SAME_THREAD);
		}

		#endregion

		#region Helpers from 3rd parties

		//Courtesy of https://stackoverflow.com/questions/5095183/how-would-i-run-an-async-taskt-method-synchronously/5097066#5097066

		/// <summary>
		/// Synchronously execute's an async Task method which has a void return value.
		/// </summary>
		/// <param name="task">The Task method to execute.</param>
		public static void RunSync(Func<Task> task)
		{
			var oldContext = SynchronizationContext.Current;
			var syncContext = new ExclusiveSynchronizationContext();
			SynchronizationContext.SetSynchronizationContext(syncContext);

			syncContext.Post(async _ =>
			{
				try
				{
					await task();
				}
				catch (Exception e)
				{
					syncContext.InnerException = e;
					throw;
				}
				finally
				{
					syncContext.EndMessageLoop();
				}
			}, null);

			syncContext.BeginMessageLoop();

			SynchronizationContext.SetSynchronizationContext(oldContext);
		}

		/// <summary>
		/// Synchronously execute's an async Task<T> method which has a T return type.
		/// </summary>
		/// <typeparam name="T">Return Type</typeparam>
		/// <param name="task">The Task<T> method to execute.</param>
		/// <returns>The result of awaiting the given Task<T>.</returns>
		public static T RunSync<T>(Func<Task<T>> task)
		{
			var oldContext = SynchronizationContext.Current;
			var syncContext = new ExclusiveSynchronizationContext();
			SynchronizationContext.SetSynchronizationContext(syncContext);
			T result = default;

			syncContext.Post(async _ =>
			{
				try
				{
					result = await task();
				}
				catch (Exception e)
				{
					syncContext.InnerException = e;
					throw;
				}
				finally
				{
					syncContext.EndMessageLoop();
				}
			}, null);

			syncContext.BeginMessageLoop();

			SynchronizationContext.SetSynchronizationContext(oldContext);

			return result;
		}

		private class ExclusiveSynchronizationContext : SynchronizationContext
		{
			private readonly AutoResetEvent workItemsWaiting = new AutoResetEvent(false);
			private readonly Queue<Tuple<SendOrPostCallback, object>> items =
				new Queue<Tuple<SendOrPostCallback, object>>();
			private bool done;

			public Exception InnerException { get; set; }

			public override void Send(SendOrPostCallback d, object state)
			{
				throw new NotSupportedException("We cannot send to our same thread");
			}

			public override void Post(SendOrPostCallback d, object state)
			{
				lock (items)
				{
					items.Enqueue(Tuple.Create(d, state));
				}

				workItemsWaiting.Set();
			}

			public void EndMessageLoop()
			{
				Post(_ => done = true, null);
			}

			public void BeginMessageLoop()
			{
				while (!done)
				{
					Tuple<SendOrPostCallback, object> task = null;
					lock (items)
					{
						if (items.Count > 0)
						{
							task = items.Dequeue();
						}
					}

					if (task != null)
					{
						task.Item1(task.Item2);
						if (InnerException != null) // the method threw an exeption
						{
							throw new AggregateException("AsyncHelpers.Run method threw an exception.", InnerException);
						}
					}
					else
					{
						workItemsWaiting.WaitOne();
					}
				}
			}

			public override SynchronizationContext CreateCopy()
			{
				return this;
			}
		}

		#endregion
	}
}