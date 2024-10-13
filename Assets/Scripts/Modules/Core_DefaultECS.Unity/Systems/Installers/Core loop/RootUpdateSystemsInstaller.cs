using System.Collections.Generic;

using HereticalSolutions.Systems;
using HereticalSolutions.Systems.Factories;

using HereticalSolutions.Logging;

using TWorld = DefaultEcs.World;
using TSystem = DefaultEcs.System.ISystem<float>;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public class RootUpdateSystemsInstaller
		: ISystemInstaller<TSystem>
	{
		private readonly TWorld simulationWorld;

		private readonly TWorld viewWorld;

		private readonly bool includeViewWorld;

		private readonly ILogger logger;

		public RootUpdateSystemsInstaller(
			TWorld simulationWorld,
			TWorld viewWorld,
			bool includeViewWorld,
			ILogger logger = null)
		{
			this.simulationWorld = simulationWorld;

			this.viewWorld = viewWorld;

			this.includeViewWorld = includeViewWorld;

			this.logger = logger;
		}

		#region ISystemInstaller

		public string Name => SystemConstsUnity.UPDATE_ROOT_INSTALLER_ID;

		public IEnumerable<string> Dependencies => new string[0];

		public bool CanInstall(
			ISystemInstallerContext<TSystem> context)
		{
			if (context.Builders.Has(SystemConstsUnity.UPDATE_SYSTEM_BUILDER_ID))
				return false;

			return true;
		}

		public void Install(
			ISystemInstallerContext<TSystem> context)
		{
			var systemBuilder = SystemFactory.BuildSystemBuilder<TSystem>();

			if (!context.Builders.TryAdd(
				SystemConstsUnity.UPDATE_SYSTEM_BUILDER_ID,
				systemBuilder))
			{
				logger?.LogError(
					GetType(),
					$"FAILED TO ADD SYSTEM BUILDER {SystemConstsUnity.UPDATE_SYSTEM_BUILDER_ID}");

				return;
			}

			systemBuilder.AddStageNodesBetweenStartAndFinish(
				SystemConstsUnity.UPDATE_SYSTEMS_ID);

			if (includeViewWorld)
			{
				systemBuilder.AddStageNodesAfterStage<TSystem>(
					SystemConstsUnity.MODEL_VALIDATION_SYSTEMS_ID,
					SystemConstsUnity.UPDATE_SYSTEMS_ID,
					false);
			}

			systemBuilder.AddStageNodesAfterStages<TSystem>(
				SystemConstsUnity.GAME_LOGIC_SYSTEMS_ID,
				new []
				{
					SystemConstsUnity.UPDATE_SYSTEMS_ID,
					SystemConstsUnity.MODEL_VALIDATION_SYSTEMS_ID
				},
				false);

			systemBuilder.AddLifetimeSystemAfterStage<TSystem>(
				SystemConstsDefaultECS.SIMULATION_WORLD_LIFETIME_SYSTEMS_ID,
				SystemConstsUnity.GAME_LOGIC_SYSTEMS_ID,
				new DespawnSystem(
					simulationWorld),
				false);

			//Otherwise the view world would start with entities pending deletion
			//Also I noticed NO such system in view world
			//TODO: decide whether the view world should have one
			if (includeViewWorld)
			{
				systemBuilder.AddLifetimeSystemAfterStage<TSystem>(
					SystemConstsDefaultECS.VIEW_WORLD_LIFETIME_SYSTEMS_ID,
					SystemConstsDefaultECS.SIMULATION_WORLD_LIFETIME_SYSTEMS_ID,
					new DespawnSystem(
						viewWorld),
					false);
			}
		}

		#endregion
	}
}