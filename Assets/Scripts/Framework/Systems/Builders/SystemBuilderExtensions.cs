using System.Collections.Generic;

using HereticalSolutions.Systems.Factories;

namespace HereticalSolutions.Systems
{
	public static class SystemBuilderExtensions
	{
		public static string GetStageStartNodeID(string stageID)
		{
			return $"{stageID} {SystemConsts.START_NODE_ID}";
		}

		public static string GetStageFinishNodeID(string stageID)
		{
			return $"{stageID} {SystemConsts.FINISH_NODE_ID}";
		}

		public static void AddStageNodesBetweenStartAndFinish<TSystem>(
			this ISystemBuilder<TSystem> systemBuilder,
			string stageID)
		{
			systemBuilder.TryAddAfterNode(
				systemBuilder.StartNode,
				SystemFactory.BuildStageNode<TSystem>(
					GetStageStartNodeID(stageID)),
				false);

			systemBuilder.TryAddBeforeNode(
				systemBuilder.FinishNode,
				SystemFactory.BuildStageNode<TSystem>(
					GetStageFinishNodeID(stageID)),
				false);
		}

		public static void AddStageNodesAfterStage<TSystem>(
			this ISystemBuilder<TSystem> systemBuilder,
			string stageID,
			string predecessorStageID,
			bool parallel)
		{
			var stageStartNode = SystemFactory.BuildStageNode<TSystem>(
				GetStageStartNodeID(stageID));

			systemBuilder.TryAddAfterStage(
				GetStageFinishNodeID(predecessorStageID),
				stageStartNode,
				parallel);

			systemBuilder.TryAddAfterNode(
				stageStartNode,
				SystemFactory.BuildStageNode<TSystem>(
					GetStageFinishNodeID(stageID)),
				false);
		}

		public static void AddStageNodesAfterStages<TSystem>(
			this ISystemBuilder<TSystem> systemBuilder,
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

			var stageStartNode = SystemFactory.BuildStageNode<TSystem>(
				GetStageStartNodeID(stageID));

			systemBuilder.TryAddAfterStages(
				predecessorStageFinishNodes,
				stageStartNode,
				parallel);

			systemBuilder.TryAddAfterNode(
				stageStartNode,
				SystemFactory.BuildStageNode<TSystem>(
					GetStageFinishNodeID(stageID)),
				false);
		}

		public static void AddStageNodesAfterStageStart<TSystem>(
			this ISystemBuilder<TSystem> systemBuilder,
			string stageID,
			string predecessorStageID,
			bool parallel)
		{
			var stageStartNode = SystemFactory.BuildStageNode<TSystem>(
				GetStageStartNodeID(stageID));

			systemBuilder.TryAddAfterStage(
				GetStageStartNodeID(predecessorStageID),
				stageStartNode,
				parallel);

			systemBuilder.TryAddAfterNode(
				stageStartNode,
				SystemFactory.BuildStageNode<TSystem>(
					GetStageFinishNodeID(stageID)),
				false);
		}

		public static void AddStageNodesBeforeStageFinish<TSystem>(
			this ISystemBuilder<TSystem> systemBuilder,
			string stageID,
			string predecessorStageID,
			bool parallel)
		{
			var stageFinishNode = SystemFactory.BuildStageNode<TSystem>(
				GetStageFinishNodeID(stageID));

			systemBuilder.TryAddBeforeStage(
				GetStageFinishNodeID(predecessorStageID),
				stageFinishNode,
				parallel);

			systemBuilder.TryAddBeforeNode(
				stageFinishNode,
				SystemFactory.BuildStageNode<TSystem>(
					GetStageStartNodeID(stageID)),
				false);
		}
	}
}