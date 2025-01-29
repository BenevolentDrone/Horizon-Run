using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using System.Text;

using HereticalSolutions.Logging;

namespace HereticalSolutions
{
	public static class TaskExtensions
	{
		private const string TASK_EXCEPTION_PREFIX = "TASK WAS TERMINATED WITH EXCEPTION: ";

		private const string INNER_EXCEPTIONS_SUFFIX = "\nINNER EXCEPTIONS:\n";

		private const string INNER_EXCEPTION_DELIMITER = "-------------------------------------";

		//For some reason the previous version I used (that utilized ContinueWith(..., TaskContinuationOptions.OnlyOnFaulted) method chain) would throw a TaskCanceledException in random places in the code
		//As stated over here, it was because TaskContinuationOptions.OnlyOnFaulted is not valid for multi-task continuations
		//https://stackoverflow.com/questions/28633871/taskcanceledexception-with-continuewith
		//So I've changed it to the option provided here:
		//https://stackoverflow.com/a/58469206

		//Courtesy of https://blog.stephencleary.com/2015/01/a-tour-of-task-part-7-continuations.html

		#region Task

		public static async Task LogExceptionsIfAny(
			this Task task)
		{
			await task;

			LookForExceptions(
				task,
				Console.WriteLine);
		}

		public static async Task ThrowExceptionsIfAny(
			this Task task)
		{
			await task;

			LookForExceptions(
				task,
				exception => throw new Exception(
					exception));
		}

		public static async Task LogExceptionsIfAny<TSource>(
			this Task task,
			ILogger logger)
		{
			await task;

			LookForExceptions(
				task,
				exception => logger?.LogError<TSource>(
					exception));
		}

		public static async Task ThrowExceptionsIfAny<TSource>(
			this Task task,
			ILogger logger)
		{
			await task;

			LookForExceptions(
				task,
				exception => throw new Exception(
					logger.TryFormatException<TSource>(
						exception)));
		}

		public static async Task LogExceptionsIfAny(
			this Task task,
			Type logSourceType,
			ILogger logger)
		{
			await task;

			LookForExceptions(
				task,
				exception => logger?.LogError(
					logSourceType,
					exception));
		}

		public static async Task ThrowExceptionsIfAny(
			this Task task,
			Type logSourceType,
			ILogger logger)
		{
			await task;

			LookForExceptions(
				task,
				exception => throw new Exception(
					logger.TryFormatException(
						logSourceType,
						exception)));
		}

		#endregion

		#region Task<T>

		public static async Task<T> LogExceptionsIfAny<T>(
			this Task<T> task)
		{
			var result = await task;

			LookForExceptions(
				task,
				Console.WriteLine);

			return result;
		}

		public static async Task<T> ThrowExceptionsIfAny<T>(
			this Task<T> task)
		{
			var result = await task;

			LookForExceptions(
				task,
				exception => throw new Exception(
					exception));

			return result;
		}

		public static async Task<T> LogExceptionsIfAny<T, TSource>(
			this Task<T> task,
			ILogger logger)
		{
			var result = await task;

			LookForExceptions(
				task,
				exception => logger?.LogError<TSource>(
					exception));

			return result;
		}

		public static async Task<T> ThrowExceptionsIfAny<T, TSource>(
			this Task<T> task,
			ILogger logger)
		{
			var result = await task;

			LookForExceptions(
				task,
				exception => throw new Exception(
					logger.TryFormatException<TSource>(
						exception)));
			
			return result;
		}

		public static async Task<T> LogExceptionsIfAny<T>(
			this Task<T> task,
			Type logSourceType,
			ILogger logger)
		{
			var result = await task;

			LookForExceptions(
				task,
				exception => logger?.LogError(
					logSourceType,
					exception));

			return result;
		}

		public static async Task<T> ThrowExceptionsIfAny<T>(
			this Task<T> task,
			Type logSourceType,
			ILogger logger)
		{
			var result = await task;

			LookForExceptions(
				task,
				exception => throw new Exception(
					logger.TryFormatException(
						logSourceType,
						exception)));

			return result;
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

		//Courtesy of https://stackoverflow.com/questions/5095183/how-would-i-run-an-async-taskt-method-synchronously
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
	
		public static void LookForExceptions(
			Task targetTask,
			Action<string> exceptionAction)
		{
			if (!targetTask.IsFaulted)
			{
				return;
			}

			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.Append(TASK_EXCEPTION_PREFIX);

			stringBuilder.Append(targetTask.Exception.Message);

			stringBuilder.Append(INNER_EXCEPTIONS_SUFFIX);

			stringBuilder.Append(INNER_EXCEPTION_DELIMITER);

			foreach (var innerException in targetTask.Exception.InnerExceptions)
			{
				stringBuilder.Append(innerException.ToString());

				stringBuilder.Append('\n');

				stringBuilder.Append(INNER_EXCEPTION_DELIMITER);

				stringBuilder.Append('\n');
			}

			exceptionAction?.Invoke(
				stringBuilder.ToString());
		}
	}
}