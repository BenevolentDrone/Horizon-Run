using System.Collections.Generic;

using HereticalSolutions.Systems;
using HereticalSolutions.Systems.Factories;

using HereticalSolutions.Modules.Core_DefaultECS.Factories;

using HereticalSolutions.Logging;

using TSystem = HereticalSolutions.Modules.Core_DefaultECS.IEntityInitializationSystem;
using TProcedure = HereticalSolutions.Modules.Core_DefaultECS.IEntityInitializationSystem;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public class RootSimulationWorldResolveSystemsInstaller
		: IProcedurePackageInstaller<TSystem, TProcedure>
	{
		private readonly EntityWorldController simulationWorldController;

		private readonly ILoggerResolver loggerResolver;

		private readonly ILogger logger;

		public RootSimulationWorldResolveSystemsInstaller(
			EntityWorldController simulationWorldController,
			ILoggerResolver loggerResolver,
			ILogger logger = null)
		{
			this.simulationWorldController = simulationWorldController;

			this.loggerResolver = loggerResolver;

			this.logger = logger;
		}

		#region ISystemInstaller

		public string PackageName => SystemConstsDefaultECS.SIMULATION_WORLD_RESOLVE_PACKAGE_INSTALLER_ID;

		public IEnumerable<string> PackageDependencies => new string[0];

		public bool CanInstall(
			IProcedurePackageInstallerContext<TSystem, TProcedure> context)
		{
			if (context.Builders.Has(SystemConstsDefaultECS.SIMULATION_WORLD_RESOLVE_SYSTEM_BUILDER_ID))
				return false;

			return true;
		}

		public void Install(
			IProcedurePackageInstallerContext<TSystem, TProcedure> context)
		{
			var systemBuilder = DefaultECSSystemFactory.BuildDefaultECSSystemBuilder<TSystem, TProcedure>(
				loggerResolver);

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

			//simulationWorldController.EntityResolveSystems = systemBuilder.BuildSystem();
		}

		#endregion
	}
}