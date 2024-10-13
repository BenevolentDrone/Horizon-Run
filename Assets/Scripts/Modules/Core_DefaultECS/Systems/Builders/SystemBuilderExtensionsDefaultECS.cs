using HereticalSolutions.Systems;
using HereticalSolutions.Systems.Factories;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public static class SystemBuilderExtensionsDefaultECS
	{
		public static ISystemNode<TSystem> AddLifetimeSystemAfterStage<TSystem>(
			this ISystemBuilder<TSystem> systemBuilder,
			string stageID,
			string predecessorStageID,
			TSystem system,
			bool parallel)
		{
			systemBuilder.AddStageNodesAfterStage<TSystem>(
				stageID,
				predecessorStageID,
				parallel);

			var lifetimeSystemNode = SystemFactory.BuildSystemNode<TSystem>(
				system);

			systemBuilder.TryAddBeforeStage(
				SystemBuilderExtensions.GetStageFinishNodeID(stageID),
				lifetimeSystemNode,
				false);

			return lifetimeSystemNode;
		}

		public static ISystemNode<TSystem> AddLifetimeSystemBeforeStageFinish<TSystem>(
			this ISystemBuilder<TSystem> systemBuilder,
			string stageID,
			string predecessorStageID,
			TSystem system,
			bool parallel)
		{
			systemBuilder.AddStageNodesBeforeStageFinish<TSystem>(
				stageID,
				predecessorStageID,
				parallel);

			var lifetimeSystemNode = SystemFactory.BuildSystemNode<TSystem>(
				system);

			systemBuilder.TryAddBeforeStage(
				SystemBuilderExtensions.GetStageFinishNodeID(stageID),
				lifetimeSystemNode,
				false);

			return lifetimeSystemNode;
		}
	}
}