using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;


namespace HereticalSolutions.Systems.Factories
{
	public static class SystemFactory
	{
		public static ProcedureNode<TProcedure> BuildSystemNode<TProcedure>(
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

			finishNode = BuildStageNode<TProcedure>(
				SystemConsts.FINISH_NODE_ID,
				default(TProcedure));

			allProcedureNodes = new HashSet<IProcedureNode<TProcedure>>();

			stages =
				RepositoriesFactory.BuildDictionaryRepository<string, IStageNode<TProcedure>>();

			procedures =
				RepositoriesFactory.BuildDictionaryRepository<Type, IList<IProcedureNode<TProcedure>>>();


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


			//return new SystemBuilder<TProcedure>(
			//	allProcedureNodes,
			//	stages,
			//	systems,
			//
			//	startNode,
			//	finishNode,
			//
			//	logger: loggerResolver?.GetLogger<SystemBuilder<TProcedure>>());
		}
	}
}