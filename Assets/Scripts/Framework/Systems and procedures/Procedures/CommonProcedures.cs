using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Systems
{
	public static class CommonProcedures
	{
		#region Delegate

		public delegate void WaitForAllSyncDelegate(IEnumerable<Task> tasksToWaitFor);

		public static Action[] CreateActionSublist(
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

		public static void ExecuteDelegatesSequentiallySync(IEnumerable<Action> actions)
		{
			foreach (var action in actions)
			{
				action();
			}
		}

		public static void BuildAndRunTasksSequentiallySync(IEnumerable<Func<Task>> taskFactories)
		{
			foreach (var taskFactory in taskFactories)
			{
				var task = taskFactory();

				task.Wait();
			}
		}

		public static void FireAndForgetActionsInParallelSync(IEnumerable<Action> actions)
		{
			foreach (var action in actions)
			{
				Task
					.Run(action)
					.ConfigureAwait(false);
			}
		}

		public static void BuildFireAndForgetTasksInParallelSync(IEnumerable<Func<Task>> taskFactories)
		{
			foreach (var taskFactory in taskFactories)
			{
				Task
					.Run(taskFactory)
					.ConfigureAwait(false);
			}
		}

		public static void WaitForSync(Task taskToWaitFor)
		{
			Task.WaitAll(
				taskToWaitFor);
		}

		//WaitForAllSyncDelegate
		public static void WaitForAllSync(IEnumerable<Task> tasksToWaitFor)
		{
			Task.WaitAll(
				(tasksToWaitFor is Task[] taskArray)
					? taskArray
					: tasksToWaitFor.ToArray());
		}

		#endregion

		#region Async

		//Courtesy of https://stackoverflow.com/questions/34145498/await-new-taskt-task-does-not-run/34146538#34146538
		//Courtesy of https://blog.stephencleary.com/2014/05/a-tour-of-task-part-1-constructors.html
		//Courtesy of https://blog.stephencleary.com/2015/02/a-tour-of-task-part-8-starting.html
		//Huge thanks to Stephen Cleary for his blog posts on Tasks

		//public static Task CreateTaskFromAction(Action action)
		//{
		//	return new Task(
		//		action);
		//}

		public static Func<Task> CreateTaskFactoryFromAction(Action action)
		{
			//return () => new Task(
			//	action);

			return async () =>
			{
				action();
			};
		}

		public static IEnumerable<Func<Task>> CreateTaskFactoriesFromActions(
			Action[] actions)
		{
			List<Func<Task>> tasks = new List<Func<Task>>();

			foreach (var action in actions)
			{
				tasks.Add(
					CreateTaskFactoryFromAction(action));
			}

			return tasks;
		}

		public static Func<Task>[] CreateTaskFactoriesSublist(
			Func<Task>[] taskFactories,
			int[] indexes)
		{
			Func<Task>[] result = new Func<Task>[indexes.Length];

			for (int i = 0; i < indexes.Length; i++)
			{
				result[i] = taskFactories[indexes[i]];
			}

			return result;
		}

		public static async Task RunTasksSequentiallyAsync(IEnumerable<Task> tasksToRun)
		{
			foreach (var task in tasksToRun)
			{
				await task;
					//.ConfigureAwait(false);
			}
		}

		public static async Task RunTasksSequentiallyAsync<TInvoker>(
			IEnumerable<Task> tasksToRun,
			ILogger logger)
		{
			foreach (var task in tasksToRun)
			{
				await task;
					//.ConfigureAwait(false);

				await task
					.ThrowExceptionsIfAny<TInvoker>(
						logger);
			}
		}

		public static async Task RunTasksSequentiallyAsync(
			IEnumerable<Task> tasksToRun,
			Type invokerType,
			ILogger logger)
		{
			foreach (var task in tasksToRun)
			{
				await task;
					//.ConfigureAwait(false);
				
				await task
					.ThrowExceptionsIfAny(
						invokerType,
						logger);
			}
		}

		public static async Task BuildAndRunTasksSequentiallyAsync(
			IEnumerable<Func<Task>> tasksFactories)
		{
			foreach (var taskFactory in tasksFactories)
			{
				var task = taskFactory();

				await task;
					//.ConfigureAwait(false);
			}
		}

		public static async Task BuildAndRunTasksSequentiallyAsync<TInvoker>(
			IEnumerable<Func<Task>> tasksFactories,
			ILogger logger)
		{
			foreach (var taskFactory in tasksFactories)
			{
				var task = taskFactory();

				await task;
					//.ConfigureAwait(false);

				await task
					.ThrowExceptionsIfAny<TInvoker>(
						logger);
			}
		}

		public static async Task BuildAndRunTasksSequentiallyAsync(
			IEnumerable<Func<Task>> tasksFactories,
			Type invokerType,
			ILogger logger)
		{
			foreach (var taskFactory in tasksFactories)
			{
				var task = taskFactory();

				await task;
					//.ConfigureAwait(false);

				await task
					.ThrowExceptionsIfAny(
						invokerType,
						logger);
			}
		}

		//public static Task BuildFireAndForgetTasksInParallelAsync(IEnumerable<Func<Task>> taskFactories)
		//{
		//	return Task.Run(() =>
		//	{
		//		foreach (var taskFactory in taskFactories)
		//		{
		//			Task
		//				.Run(taskFactory)
		//				.ConfigureAwait(false);
		//		}
		//	});
		//}

		//public static Task WaitForAllAsync(
		//	IEnumerable<Task> tasksToWaitFor)
		//{
		//	return Task.WhenAll(tasksToWaitFor);
		//}

		#endregion

		#region Completion sources

		public static IEnumerable<Task> CreateCompletionTasksSublist(
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
	}
}