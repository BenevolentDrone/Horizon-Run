using System.Collections.Generic;

using HereticalSolutions.Systems.Factories;

namespace HereticalSolutions.Systems
{
	public static class SystemBuilderExtensions
	{
		public static string GetStageStartNodeID(string stageID)
		{
			return $"{stageID} {SystemConsts.STAGE_START_PREFIX}";
		}

		public static string GetStageFinishNodeID(string stageID)
		{
			return $"{stageID} {SystemConsts.STAGE_FINISH_POSTFIX}";
		}

		public static void AddStageNodesBetweenStartAndFinish<TSystem, TProcedure>(
			this ISystemBuilder<TSystem, TProcedure> systemBuilder,
			string stageID)
		{
			systemBuilder.TryAddAfterNode(
				systemBuilder.StartNode,
				SystemFactory.BuildStageNode<TProcedure>(
					GetStageStartNodeID(stageID)),
				false);

			systemBuilder.TryAddBeforeNode(
				systemBuilder.FinishNode,
				SystemFactory.BuildStageNode<TProcedure>(
					GetStageFinishNodeID(stageID)),
				false);
		}

		public static void AddStageNodesAfterStage<TSystem, TProcedure>(
			this ISystemBuilder<TSystem, TProcedure> systemBuilder,
			string stageID,
			string predecessorStageID,
			bool parallel)
		{
			var stageStartNode = SystemFactory.BuildStageNode<TProcedure>(
				GetStageStartNodeID(stageID));

			systemBuilder.TryAddAfterStage(
				GetStageFinishNodeID(predecessorStageID),
				stageStartNode,
				parallel);

			systemBuilder.TryAddAfterNode(
				stageStartNode,
				SystemFactory.BuildStageNode<TProcedure>(
					GetStageFinishNodeID(stageID)),
				false);
		}

		public static void AddStageNodesAfterStages<TSystem, TProcedure>(
			this ISystemBuilder<TSystem, TProcedure> systemBuilder,
			string stageID,
			IEnumerable<string> predecessorStageIDs,
			bool parallel)
		{
			List<string> predecessorStageFinishNodes = new List<string>();

			foreach (var predecessorStageID in predecessorStageIDs)
			{
				predecessorStageFinishNodes.Add(
					GetStageFinishNodeID(predecessorStageID));
			}

			var stageStartNode = SystemFactory.BuildStageNode<TProcedure>(
				GetStageStartNodeID(stageID));

			systemBuilder.TryAddAfterStages(
				predecessorStageFinishNodes,
				stageStartNode,
				parallel);

			systemBuilder.TryAddAfterNode(
				stageStartNode,
				SystemFactory.BuildStageNode<TProcedure>(
					GetStageFinishNodeID(stageID)),
				false);
		}

		public static void AddStageNodesAfterStageStart<TSystem, TProcedure>(
			this ISystemBuilder<TSystem, TProcedure> systemBuilder,
			string stageID,
			string predecessorStageID,
			bool parallel)
		{
			var stageStartNode = SystemFactory.BuildStageNode<TProcedure>(
				GetStageStartNodeID(stageID));

			systemBuilder.TryAddAfterStage(
				GetStageStartNodeID(predecessorStageID),
				stageStartNode,
				parallel);

			systemBuilder.TryAddAfterNode(
				stageStartNode,
				SystemFactory.BuildStageNode<TProcedure>(
					GetStageFinishNodeID(stageID)),
				false);
		}

		public static void AddStageNodesBeforeStageFinish<TSystem, TProcedure>(
			this ISystemBuilder<TSystem, TProcedure> systemBuilder,
			string stageID,
			string predecessorStageID,
			bool parallel)
		{
			var stageFinishNode = SystemFactory.BuildStageNode<TProcedure>(
				GetStageFinishNodeID(stageID));

			systemBuilder.TryAddBeforeStage(
				GetStageFinishNodeID(predecessorStageID),
				stageFinishNode,
				parallel);

			systemBuilder.TryAddBeforeNode(
				stageFinishNode,
				SystemFactory.BuildStageNode<TProcedure>(
					GetStageStartNodeID(stageID)),
				false);
		}
	}
}