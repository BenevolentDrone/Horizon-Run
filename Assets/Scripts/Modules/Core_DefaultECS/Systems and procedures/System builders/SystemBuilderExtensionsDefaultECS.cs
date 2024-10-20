using HereticalSolutions.Systems;
using HereticalSolutions.Systems.Factories;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public static class SystemBuilderExtensionsDefaultECS
	{
		public static IProcedureNode<TProcedure> AddLifetimeSystemAfterStage<TSystem, TProcedure>(
			this ISystemBuilder<TSystem, TProcedure> systemBuilder,
			string stageID,
			string predecessorStageID,
			TProcedure system,
			bool parallel)
		{
			systemBuilder.AddStageNodesAfterStage<TSystem, TProcedure>(
				stageID,
				predecessorStageID,
				parallel);

			var lifetimeSystemNode = SystemFactory.BuildProcedureNode<TProcedure>(
				system);

			systemBuilder.TryAddBeforeStage(
				SystemBuilderExtensions.GetStageFinishNodeID(stageID),
				lifetimeSystemNode,
				false);

			return lifetimeSystemNode;
		}

		public static IProcedureNode<TProcedure> AddLifetimeSystemBeforeStageFinish<TSystem, TProcedure>(
			this ISystemBuilder<TSystem, TProcedure> systemBuilder,
			string stageID,
			string predecessorStageID,
			TProcedure system,
			bool parallel)
		{
			systemBuilder.AddStageNodesBeforeStageFinish<TSystem, TProcedure>(
				stageID,
				predecessorStageID,
				parallel);

			var lifetimeSystemNode = SystemFactory.BuildProcedureNode<TProcedure>(
				system);

			systemBuilder.TryAddBeforeStage(
				SystemBuilderExtensions.GetStageFinishNodeID(stageID),
				lifetimeSystemNode,
				false);

			return lifetimeSystemNode;
		}
	}
}