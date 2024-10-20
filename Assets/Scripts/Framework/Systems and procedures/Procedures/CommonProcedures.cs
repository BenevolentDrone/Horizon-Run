using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace HereticalSolutions.Systems
{
	public static class CommonProcedures
	{
		#region Delegate

		public delegate void WaitForAllSyncDelegate(IEnumerable<Task> tasksToWaitFor);

		public static void ExecuteDelegatesSequentiallySync(IEnumerable<Action> actions)
		{
			foreach (var action in actions)
			{
				action();
			}
		}

		public static void RunTasksSequentiallySync(IEnumerable<Func<Task>> taskFactories)
		{
			foreach (var taskFactory in taskFactories)
			{
				var task = taskFactory();

				task.Wait();
			}
		}

		public static void FireAndForgetTasksInParallelSync(IEnumerable<Func<Task>> taskFactories)
		{
			foreach (var taskFactory in taskFactories)
			{
				Task.Run(taskFactory).ConfigureAwait(false);
			}
		}

		public static void FireAndForgetActionsInParallelSync(IEnumerable<Action> actions)
		{
			foreach (var action in actions)
			{
				Task.Run(action).ConfigureAwait(false);
			}
		}

		public static Action[] CreateActionInvokationList(
			Action[] actions,
			int[] indexes)
		{
			Action[] result = new Action[indexes.Length];

			for (int i = 0; i < indexes.Length; i++)
			{
				result[i] = actions[indexes[i]];
			}

			return result;
		}

		//WaitForAllSyncDelegate
		public static void WaitForAllSync(IEnumerable<Task> tasksToWaitFor)
		{
			Task.WaitAll(
				(tasksToWaitFor is Task[] taskArray)
					? taskArray
					: tasksToWaitFor.ToArray());
		}

		#region Completion sources

		public static void ResetCompletionSources<TResult>(
			TaskCompletionSource<TResult>[] completionSources,
			Task[] completionTasks)
		{
			for (int i = 0; i < completionSources.Length; i++)
			{
				//Courtesy of https://devblogs.microsoft.com/premier-developer/the-danger-of-taskcompletionsourcet-class/
				completionSources[i] = new TaskCompletionSource<TResult>(
					TaskCreationOptions.RunContinuationsAsynchronously);

				completionTasks[i] = completionSources[i].Task;
			}
		}

		public static void FireCompletion<TResult>(
			TaskCompletionSource<TResult>[] completionSources,
			int index)
		{
			completionSources[index].SetResult(default(TResult));
		}

		//public static void WaitForAllCompletionSourcesSync(
		//	Task[] completionTasks)
		//{
		//	// Wait for all tasks to complete
		//	Task.WaitAll(completionTasks);
		//}

		#endregion

		#endregion

		#region Async

		public static Task CreateTaskFromAction(Action action)
		{
			return new Task(action);
		}

		public static IEnumerable<Func<Task>> CreateTaskFactoriesFromActions(
			Action[] actions)
		{
			List<Func<Task>> tasks = new List<Func<Task>>();

			foreach (var action in actions)
			{
				tasks.Add(
					() => CreateTaskFromAction(action));
			}

			return tasks;
		}

		public static IEnumerable<Task> CreateTasksListFromCompletionTasks(
			Task[] completionTasks,
			int[] indexes)
		{
			List<Task> tasks = new List<Task>();

			foreach (var index in indexes)
			{
				tasks.Add(completionTasks[index]);
			}

			return tasks;
		}

		public static IEnumerable<Func<Task>> CreateTaskFactoriesFromCompletionTasks(
			Task[] completionTasks,
			int[] indexes)
		{
			List<Func<Task>> tasks = new List<Func<Task>>();

			foreach (var index in indexes)
			{
				tasks.Add(
					() => completionTasks[index]);
			}

			return tasks;
		}

		public static async Task RunTasksSequentiallyAsync(IEnumerable<Task> tasksToRun)
		{
			foreach (var task in tasksToRun)
			{
				await task;
			}
		}

		public static Task FireAndForgetTasksInParallelAsync(IEnumerable<Func<Task>> taskFactories)
		{
			return Task.Run(() =>
			{
				foreach (var taskFactory in taskFactories)
				{
					Task.Run(taskFactory);
				}
			});
		}

		public static Task WaitForAllAsync(
			IEnumerable<Task> tasksToWaitFor)
		{
			return Task.WhenAll(tasksToWaitFor);
		}

		#endregion
	}
}