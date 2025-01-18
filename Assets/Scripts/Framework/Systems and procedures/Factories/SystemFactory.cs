using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;
using System.Threading.Tasks;


namespace HereticalSolutions.Systems.Factories
{
	public static class SystemFactory
	{
		public static ProcedureNode<TProcedure> BuildProcedureNode<TProcedure>(
			TProcedure system,
			sbyte priority = 0)
		{
			return new ProcedureNode<TProcedure>(
				system,
				priority);
		}

		public static StageNode<TProcedure> BuildStageNode<TProcedure>(
			string stage,
			TProcedure system = default(TProcedure),
			sbyte priority = 0)
		{
			return new StageNode<TProcedure>(
				stage,
				system,
				priority);
		}

		public static void PrepareSystemBuilderDependencies<TSystem, TProcedure>(
			out HashSet<IProcedureNode<TProcedure>> allProcedureNodes,
			out IRepository<string, IStageNode<TProcedure>> stages,
			out IRepository<Type, IList<IProcedureNode<TProcedure>>> procedures,

			out IStageNode<TProcedure> startNode,
			out IStageNode<TProcedure> finishNode,

			ILoggerResolver loggerResolver = null)
		{
			startNode = BuildStageNode<TProcedure>(
				SystemConsts.START_NODE_ID,
				default(TProcedure));

			((StageNode<TProcedure>)startNode).IsDetached = false;

			((StageNode<TProcedure>)startNode).ExpectedThread = 1;

			finishNode = BuildStageNode<TProcedure>(
				SystemConsts.FINISH_NODE_ID,
				default(TProcedure));

			((StageNode<TProcedure>)finishNode).IsDetached = false;

			((StageNode<TProcedure>)finishNode).ExpectedThread = 1;
			

			allProcedureNodes = new HashSet<IProcedureNode<TProcedure>>();

			stages =
				RepositoryFactory.BuildDictionaryRepository<string, IStageNode<TProcedure>>();

			procedures =
				RepositoryFactory.BuildDictionaryRepository<Type, IList<IProcedureNode<TProcedure>>>();


			((StageNode<TProcedure>)startNode).SequentialNext = finishNode;

			((StageNode<TProcedure>)finishNode).SequentialPrevious = startNode;


			allProcedureNodes.Add(startNode as IProcedureNode<TProcedure>);

			allProcedureNodes.Add(finishNode as IProcedureNode<TProcedure>);


			stages.Add(
				startNode.Stage,
				startNode);

			stages.Add(
				finishNode.Stage,
				finishNode);
		}

		public static DelegateSystemBuilder BuildDelegateSystemBuilder(
			ILoggerResolver loggerResolver = null)
		{
			PrepareSystemBuilderDependencies<Action, Action>(
				out HashSet<IProcedureNode<Action>> allProcedureNodes,
				out IRepository<string, IStageNode<Action>> stages,
				out IRepository<Type, IList<IProcedureNode<Action>>> procedures,

				out IStageNode<Action> startNode,
				out IStageNode<Action> finishNode,

				loggerResolver);

			return new DelegateSystemBuilder(
				allProcedureNodes,
				stages,
				procedures,

				startNode,
				finishNode,

				logger: loggerResolver?.GetLogger<DelegateSystemBuilder>());
		}

		public static AsyncSystemBuilder BuildAsyncSystemBuilder(
			ILoggerResolver loggerResolver = null)
		{
			PrepareSystemBuilderDependencies<Func<Task>, Func<Task>>(
				out HashSet<IProcedureNode<Func<Task>>> allProcedureNodes,
				out IRepository<string, IStageNode<Func<Task>>> stages,
				out IRepository<Type, IList<IProcedureNode<Func<Task>>>> procedures,

				out IStageNode<Func<Task>> startNode,
				out IStageNode<Func<Task>> finishNode,

				loggerResolver);

			return new AsyncSystemBuilder(
				allProcedureNodes,
				stages,
				procedures,

				startNode,
				finishNode,

				logger: loggerResolver?.GetLogger<AsyncSystemBuilder>());
		}
	}
}