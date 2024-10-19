using System;
using System.Collections.Generic;

namespace HereticalSolutions.Systems
{
	public interface ISystemBuilder<TSystem, TProcedure>
	{
		#region Start and finish

		IStageNode<TProcedure> StartNode { get; }

		IStageNode<TProcedure> FinishNode { get; }

		#endregion

		#region Has

		bool HasStageNode(
			string stageID);

		bool HasAllStageNodes(
			IEnumerable<string> stageIDs);

		bool HasProcedureNodes(
			Type procedureType);

		#endregion

		#region Get

		bool TryGetStageNode(
			string stageID,
			out IStageNode<TProcedure> node);

		bool TryGetProcedureNodes(
			Type procedureType,
			out IEnumerable<IReadOnlyProcedureNode<TProcedure>> nodes);

		#endregion

		#region Add

		bool TryAddBeforeStage(
			string stageID,
			IReadOnlyProcedureNode<TProcedure> node,
			bool parallel = false);

		bool TryAddBeforeStages(
			IEnumerable<string> stageIDs,
			IReadOnlyProcedureNode<TProcedure> node,
			bool parallel = false);

		bool TryAddAfterStage(
			string stageID,
			IReadOnlyProcedureNode<TProcedure> node,
			bool parallel = false);

		bool TryAddAfterStages(
			IEnumerable<string> stageIDs,
			IReadOnlyProcedureNode<TProcedure> node,
			bool parallel = false);

		bool TryAddBeforeNode(
			IReadOnlyProcedureNode<TProcedure> successor,
			IReadOnlyProcedureNode<TProcedure> node,
			bool parallel = false);

		bool TryAddBeforeNodes(
			IEnumerable<IReadOnlyProcedureNode<TProcedure>> successors,
			IReadOnlyProcedureNode<TProcedure> node,
			bool parallel = false);

		bool TryAddAfterNode(
			IReadOnlyProcedureNode<TProcedure> predecessor,
			IReadOnlyProcedureNode<TProcedure> node,
			bool parallel = false);

		bool TryAddAfterNodes(
			IEnumerable<IReadOnlyProcedureNode<TProcedure>> predecessors,
			IReadOnlyProcedureNode<TProcedure> node,
			bool parallel = false);

		#endregion

		#region Remove

		bool TryRemoveStage(
			string stageID);

		bool TryRemoveNode(
			IReadOnlyProcedureNode<TProcedure> node);

		bool TryRemoveNodes(
			IEnumerable<IReadOnlyProcedureNode<TProcedure>> nodes);

		#endregion

		#region Link

		bool TryLinkNodes(
			IReadOnlyProcedureNode<TProcedure> predecessor,
			IReadOnlyProcedureNode<TProcedure> successor);

		#endregion

		#region Validate

		bool ValidateSystem();

		#endregion

		#region Build

		public bool BuildSystem(
			out TSystem system);

		#endregion
	}
}