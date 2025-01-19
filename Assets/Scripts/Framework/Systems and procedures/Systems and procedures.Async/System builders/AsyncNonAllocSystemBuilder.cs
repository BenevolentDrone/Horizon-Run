using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

using TSystem = System.Threading.Tasks.ValueTask;
using TProcedure = System.Threading.Tasks.ValueTask;

namespace HereticalSolutions.Systems
{
	public class AsyncNonAllocSystemBuilder
		: ASystemBuilder<TSystem, TProcedure>
	{
		public AsyncNonAllocSystemBuilder(
			HashSet<IProcedureNode<TProcedure>> allProcedureNodes,
			IRepository<string, IStageNode<TProcedure>> stages,
			IRepository<Type, IList<IProcedureNode<TProcedure>>> procedures,

			IStageNode<TProcedure> startNode,
			IStageNode<TProcedure> finishNode,

			byte freeThreadIndex = 2,

			ILogger logger)
			: base(
				allProcedureNodes,
				stages,
				procedures,

				startNode,
				finishNode,

				freeThreadIndex,

				logger)
		{
		}

		#region ISystemBuilder

		public override bool BuildSystem(
			out TSystem system)
		{
			system = default(TSystem);

			if (dirty)
				return false;

			if (!validated)
				return false;


			IRepository<int, IProcedureNode<TProcedure>> threadStartNodeRepository =
				RepositoryFactory.BuildDictionaryRepository<int, IProcedureNode<TProcedure>>();

			IRepository<IProcedureNode<TProcedure>, List<int>> threadsToStartRepository =
				RepositoryFactory.BuildDictionaryRepository<IProcedureNode<TProcedure>, List<int>>();

			List<IProcedureNode<TProcedure>> nodesWithCompletionSource = new List<IProcedureNode<TProcedure>>();


			threadStartNodeRepository.Add(
				1,
				StartNode as IProcedureNode<TProcedure>);

			ScanNodes(
				threadStartNodeRepository,
				threadsToStartRepository,
				nodesWithCompletionSource);

			//CompressExpectedThreads(
			//	threadStartNodeRepository,
			//	threadsToStartRepository);


			//Func<Task>[] threadTaskFactories = new Func<Task>[threadStartNodeRepository.Count];
			//
			//TaskCompletionSource<object>[] completionSources = new TaskCompletionSource<object>[nodesWithCompletionSource.Count];
			//
			//Task[] completionTasks = new Task[nodesWithCompletionSource.Count];
			//
			//
			//var keys = threadStartNodeRepository.Keys.ToArray();

			//for (byte i = 0; i < keys.Length; i++)
			//{
			//	var expectedKey = (byte)(i + 1);
			//
			//	threadTaskFactories[i] = BuildTaskFactoryForThread(
			//		threadStartNodeRepository.Get(
			//			expectedKey),
			//		threadsToStartRepository,
			//		nodesWithCompletionSource,
			//
			//		threadTaskFactories,
			//		completionSources,
			//		completionTasks);
			//}

			//Func<Task> startTaskFactory = threadTaskFactories[0];
			//
			//if (completionSources.Length > 0)
			//{
			//	Func<Task> resetCompletionSourcesTaskFactory =
			//		CommonProcedures.CreateTaskFactoryFromAction(
			//			() => CommonProcedures.ResetCompletionSources<object>(
			//				completionSources,
			//				completionTasks));
			//
			//	system = () => CommonProcedures.BuildAndRunTasksSequentiallyAsync(
			//		new Func<Task>[]
			//		{
			//			resetCompletionSourcesTaskFactory,
			//			startTaskFactory
			//		},
			//		GetType(),
			//		logger);
			//}
			//else
			//{
			//	system = startTaskFactory;
			//}

			return true;
		}

		#endregion

		private void ScanNodes(
			IRepository<int, IProcedureNode<TProcedure>> threadStartNodeRepository,
			IRepository<IProcedureNode<TProcedure>, List<int>> threadsToStartRepository,
			List<IProcedureNode<TProcedure>> nodesWithCompletionSource)
		{
			foreach (var node in allProcedureNodes)
			{
				//bool shouldHaveCompletionSource = false;


				var parallelPrevious = node.ParallelPrevious as IList<IProcedureNode<TProcedure>>;

				if (parallelPrevious != null
					&& parallelPrevious.Count > 0
					&& node.SequentialPrevious == null)
				{
					threadStartNodeRepository.Add(
						node.ExpectedThread,
						node);
				}


				var parallelNext = node.ParallelNext as IList<IProcedureNode<TProcedure>>;

				if (parallelNext != null)
				{
					List<int> threadsToStart = null;

					foreach (var next in parallelNext)
					{
						//If the parallel node has no sequential predecessor then we start its thread right here
						if (next.SequentialPrevious == null)
						{
							if (threadsToStart == null)
							{
								threadsToStart = new List<int>();
							}

							threadsToStart.Add(
								next.ExpectedThread);
						}
						else //Otherwise we create a completion source to wait for
						{
							//shouldHaveCompletionSource = true;
						}
					}

					if (threadsToStart != null)
					{
						threadsToStartRepository.Add(
							node,
							threadsToStart);
					}
				}


				//if (shouldHaveCompletionSource)
				//{
				//	nodesWithCompletionSource.Add(
				//		node);
				//}
			}
		}

		private void CompressExpectedThreads(
			IRepository<int, IProcedureNode<TProcedure>> threadStartNodeRepository,
			IRepository<IProcedureNode<TProcedure>, List<int>> threadsToStartRepository)
		{
			var keys = threadStartNodeRepository.Keys.ToArray();

			var threadsToStartValues = threadsToStartRepository.Values;

			Array.Sort(keys);

			byte inverseKey = (byte)(keys.Length - 1);

			for (byte i = 0; i < keys.Length; i++)
			{
				var expectedKey = (byte)(i + 1);

				if (!threadStartNodeRepository.Has(expectedKey))
				{
					var startNode = threadStartNodeRepository.Get(
						inverseKey);

					var currentNode = startNode;

					while (currentNode != null)
					{
						currentNode.ExpectedThread = expectedKey;

						currentNode = currentNode.SequentialNext as IProcedureNode<TProcedure>;
					}

					foreach (var threadsToStart in threadsToStartValues)
					{
						for (int j = 0; j < threadsToStart.Count; j++)
						{
							if (threadsToStart[j] == inverseKey)
							{
								threadsToStart[j] = expectedKey;
							}
						}
					}

					threadStartNodeRepository.Add(
						expectedKey,
						startNode);

					threadStartNodeRepository.Remove(
						inverseKey);

					inverseKey--;
				}
			}
		}

		/*
		private Func<Task> BuildTaskFactoryForThread(
			IProcedureNode<TProcedure> startNode,
			IRepository<IProcedureNode<TProcedure>, List<int>> threadsToStartRepository,
			List<IProcedureNode<TProcedure>> nodesWithCompletionSource,

			Func<Task>[] threadTaskFactories,
			TaskCompletionSource<object>[] completionSources,
			Task[] completionTasks)
		{
			var currentNode = startNode;

			List<Func<Task>> innerTaskFactories = new List<Func<Task>>();

			while (currentNode != null)
			{
				//If the thread is not started with current node and there are tasks to wait for, add a wait delegate
				var parallelPrevious = currentNode.ParallelPrevious as IList<IProcedureNode<TProcedure>>;

				if (parallelPrevious != null
					&& parallelPrevious.Count > 0
					&& currentNode.SequentialPrevious != null)
				{
					int[] indexes = new int[parallelPrevious.Count];

					for (int i = 0; i < parallelPrevious.Count; i++)
					{
						indexes[i] = nodesWithCompletionSource.IndexOf(
							parallelPrevious[i]);
					}

					//innerTaskFactories.Add(
					//	() => CommonProcedures.WaitForAllAsync(
					//		CommonProcedures.CreateCompletionTasksSublist(
					//			completionTasks,
					//			indexes)));

					innerTaskFactories.Add(
						CommonProcedures.CreateTaskFactoryFromAction(
							() => CommonProcedures.WaitForAllSync(
								CommonProcedures.CreateCompletionTasksSublist(
									completionTasks,
									indexes))));
				}

				//Add the current node's procedure
				if (currentNode.Procedure != null)
				{
					var procedure = currentNode.Procedure;

					innerTaskFactories.Add(
						procedure);
				}

				//If the current node has a completion source, add a completion delegate
				if (nodesWithCompletionSource.Contains(currentNode))
				{
					int index = nodesWithCompletionSource.IndexOf(currentNode);

					innerTaskFactories.Add(
						CommonProcedures.CreateTaskFactoryFromAction(
							() => CommonProcedures.FireCompletion(
								completionSources,
								index)));
				}

				//If the current node starts new threads, add a fire delegate
				if (threadsToStartRepository.Has(currentNode))
				{
					var threadsToStart = threadsToStartRepository.Get(
						currentNode);

					int[] threadIndexes = new int[threadsToStart.Count];

					for (int i = 0; i < threadsToStart.Count; i++)
					{
						threadIndexes[i] = threadsToStart[i] - 1;
					}

					//innerTaskFactories.Add(
					//	() => CommonProcedures.BuildFireAndForgetTasksInParallelAsync(
					//		CommonProcedures.CreateTaskFactoriesSublist(
					//			threadTaskFactories,
					//			threadIndexes)));

					innerTaskFactories.Add(
						CommonProcedures.CreateTaskFactoryFromAction(
							() => CommonProcedures.BuildFireAndForgetTasksInParallelSync(
								CommonProcedures.CreateTaskFactoriesSublist(
									threadTaskFactories,
									threadIndexes))));
				}

				currentNode = currentNode.SequentialNext as IProcedureNode<TProcedure>;
			}

			return () => CommonProcedures.BuildAndRunTasksSequentiallyAsync(
				innerTaskFactories.ToArray(),
				GetType(),
				logger);
		}
		*/
	}
}