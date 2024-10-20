/*
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Repositories;

using HereticalSolutions.Systems;

using HereticalSolutions.Logging;

using TSystem = System.Action;
//using TProcedure = System.Action;
using TProcedure = System.Func<System.Threading.Tasks.Task>;

namespace HereticalSolutions.Systems
{
	public class AsyncSystemBuilder
		: ASystemBuilder<TSystem, TProcedure>
	{
		public AsyncSystemBuilder(
			HashSet<IProcedureNode<TProcedure>> allProcedureNodes,
			IRepository<string, IStageNode<TProcedure>> stages,
			IRepository<Type, IList<IProcedureNode<TProcedure>>> procedures,

			IStageNode<TProcedure> startNode,
			IStageNode<TProcedure> finishNode,

			byte freeThreadIndex = 2,

			ILogger logger = null)
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
			IRepository<IProcedureNode<TProcedure>, Task> waitTasks =
				RepositoriesFactory.BuildDictionaryRepository<IProcedureNode<TProcedure>, Task>();

			IRepository<IProcedureNode<TProcedure>, Task> runInParallelTasks =
				RepositoriesFactory.BuildDictionaryRepository<IProcedureNode<TProcedure>, Task>();

			IProcedureNode<TProcedure> currentNode = StarthNode as IProcedureNode<TProcedure>;
		}

		#endregion

		private Action VisitNode(
			IProcedureNode<TProcedure> node,
			IRepository<int, Task> threadTasks)
		{
			Action result = node.Procedure;

			//Wait for all threads to finish
			var parallelPrevious = node.ParallelPrevious as IList<IProcedureNode<TProcedure>>;

			if (parallelPrevious != null)
			{
				List<Task> tasksToWait = new List<Task>();

				foreach (var previous in parallelPrevious)
				{
					tasksToWait.Add(
						threadTasks.Get(
							previous.ExpectedThread));
				}

				result = () =>
				{
					Task.WaitAll(tasksToWait.ToArray());
				};
			}

			if (node.SequentialNext != null)
			{
				var next = node.SequentialNext as IProcedureNode<TProcedure>;

				if (next != null)
				{
				}
			}

			if (node.ParallelNext != null)
			{
				var parallelNext = node.ParallelNext as IList<IProcedureNode<TProcedure>>;

				if (parallelNext != null)
				{
					foreach (var next in parallelNext)
					{
					}
				}
			}
		}
	}
}
*/