using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;


namespace HereticalSolutions.Systems.Factories
{
	public static class SystemFactory
	{
		public static SystemNode<TSystem> BuildSystemNode<TSystem>(
			TSystem system,
			sbyte priority = 0)
		{
			return new SystemNode<TSystem>(
				system,
				priority);
		}

		public static StageNode<TSystem> BuildStageNode<TSystem>(
			string stage,
			TSystem system = default(TSystem),
			sbyte priority = 0)
		{
			return new StageNode<TSystem>(
				stage,
				system,
				priority);
		}

		public static SystemBuilder<TSystem> BuildSystemBuilder<TSystem>(
			ILoggerResolver loggerResolver = null)
		{
			var startNode = BuildStageNode<TSystem>(
				SystemConsts.START_NODE_ID,
				default(TSystem));

			var finishNode = BuildStageNode<TSystem>(
				SystemConsts.FINISH_NODE_ID,
				default(TSystem));

			var stages =
				RepositoriesFactory.BuildDictionaryRepository<string, IStageNode<TSystem>>();

			var systems =
				RepositoriesFactory.BuildDictionaryRepository<Type, IList<ISystemNode<TSystem>>>();


			startNode.SequentialNext = finishNode;

			finishNode.SequentialPrevious = startNode;


			stages.Add(
				startNode.Stage,
				startNode);

			stages.Add(
				finishNode.Stage,
				finishNode);


			return new SystemBuilder<TSystem>(
				stages,
				systems,

				startNode,
				finishNode,

				logger: loggerResolver?.GetLogger<SystemBuilder<TSystem>>());
		}
	}
}