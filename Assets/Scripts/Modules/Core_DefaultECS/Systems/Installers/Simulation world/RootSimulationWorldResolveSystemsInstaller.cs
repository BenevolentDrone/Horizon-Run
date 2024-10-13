using System.Collections.Generic;

using HereticalSolutions.Systems;
using HereticalSolutions.Systems.Factories;

using HereticalSolutions.Logging;

using TSystem = HereticalSolutions.Modules.Core_DefaultECS.IEntityInitializationSystem;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public class RootSimulationWorldResolveSystemsInstaller
		: ISystemInstaller<TSystem>
	{
		private readonly EntityWorldController simulationWorldController;

		private readonly ILogger logger;

		public RootSimulationWorldResolveSystemsInstaller(
			EntityWorldController simulationWorldController,
			ILogger logger = null)
		{
			this.simulationWorldController = simulationWorldController;

			this.logger = logger;
		}

		#region ISystemInstaller

		public string Name => SystemConstsDefaultECS.SIMULATION_WORLD_RESOLVE_ROOT_INSTALLER_ID;

		public IEnumerable<string> Dependencies => new string[0];

		public bool CanInstall(
			ISystemInstallerContext<TSystem> context)
		{
			if (context.Builders.Has(SystemConstsDefaultECS.SIMULATION_WORLD_RESOLVE_SYSTEM_BUILDER_ID))
				return false;

			return true;
		}

		public void Install(
			ISystemInstallerContext<TSystem> context)
		{
			var systemBuilder = SystemFactory.BuildSystemBuilder<TSystem>();

			if (!context.Builders.TryAdd(
				SystemConstsDefaultECS.SIMULATION_WORLD_RESOLVE_SYSTEM_BUILDER_ID,
				systemBuilder))
			{
				logger?.LogError(
					GetType(),
					$"FAILED TO ADD SYSTEM BUILDER {SystemConstsDefaultECS.SIMULATION_WORLD_RESOLVE_SYSTEM_BUILDER_ID}");

				return;
			}

			systemBuilder.AddStageNodesBetweenStartAndFinish(
				SystemConstsDefaultECS.SIMULATION_WORLD_RESOLVE_SYSTEMS_ID);

			simulationWorldController.EntityResolveSystems = systemBuilder.BuildSystem();
		}

		#endregion
	}
}