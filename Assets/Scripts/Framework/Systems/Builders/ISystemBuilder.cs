using System;
using System.Collections.Generic;

namespace HereticalSolutions.Systems
{
	public interface ISystemBuilder<TSystem>
	{
		IStageNode<TSystem> StartNode { get; }

		IStageNode<TSystem> FinishNode { get; }

		#region Has

		bool HasStageNode(
			string stageID);

		bool HasAllStageNodes(
			IEnumerable<string> stageIDs);

		bool HasSystemNodes(
			Type systemType);

		#endregion

		#region Get

		bool TryGetStageNode(
			string stageID,
			out IStageNode<TSystem> node);

		bool TryGetSystemNodes(
			Type systemType,
			out IEnumerable<IReadOnlySystemNode<TSystem>> nodes);

		#endregion

		#region Add

		bool TryAddBeforeStage(
			string stageID,
			IReadOnlySystemNode<TSystem> node,
			//sbyte priority = 0,	//priority is stored in the node
			bool parallel = false);

		bool TryAddBeforeStages(
			IEnumerable<string> stageIDs,
			IReadOnlySystemNode<TSystem> node,
			//sbyte priority = 0,	//priority is stored in the node
			bool parallel = false);

		bool TryAddAfterStage(
			string stageID,
			IReadOnlySystemNode<TSystem> node,
			//sbyte priority = 0,	//priority is stored in the node
			bool parallel = false);

		bool TryAddAfterStages(
			IEnumerable<string> stageIDs,
			IReadOnlySystemNode<TSystem> node,
			//sbyte priority = 0,	//priority is stored in the node
			bool parallel = false);

		bool TryAddBeforeNode(
			IReadOnlySystemNode<TSystem> successor,
			IReadOnlySystemNode<TSystem> node,
			//sbyte priority = 0,	//priority is stored in the node
			bool parallel = false);

		bool TryAddBeforeNodes(
			IEnumerable<IReadOnlySystemNode<TSystem>> successors,
			IReadOnlySystemNode<TSystem> node,
			//sbyte priority = 0,	//priority is stored in the node
			bool parallel = false);

		bool TryAddAfterNode(
			IReadOnlySystemNode<TSystem> predecessor,
			IReadOnlySystemNode<TSystem> node,
			//sbyte priority = 0,	//priority is stored in the node
			bool parallel = false);

		bool TryAddAfterNodes(
			IEnumerable<IReadOnlySystemNode<TSystem>> predecessors,
			IReadOnlySystemNode<TSystem> node,
			//sbyte priority = 0,	//priority is stored in the node
			bool parallel = false);

		#endregion

		#region Remove

		bool TryRemoveStage(
			string stageID);

		bool TryRemoveNode(
			IReadOnlySystemNode<TSystem> node);

		bool TryRemoveNodes(
			IEnumerable<IReadOnlySystemNode<TSystem>> nodes);

		#endregion

		#region Link

		bool TryLinkNodes(
			IReadOnlySystemNode<TSystem> predecessor,
			IReadOnlySystemNode<TSystem> successor);

		#endregion

		#region Build

		TSystem BuildSystem();

		#endregion
	}
}