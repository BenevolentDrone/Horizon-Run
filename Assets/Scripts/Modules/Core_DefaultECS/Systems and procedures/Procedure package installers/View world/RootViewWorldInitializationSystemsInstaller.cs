using System.Collections.Generic;

using HereticalSolutions.Systems;
using HereticalSolutions.Systems.Factories;

using HereticalSolutions.Modules.Core_DefaultECS.Factories;

using HereticalSolutions.Logging;

using TSystem = HereticalSolutions.Modules.Core_DefaultECS.IEntityInitializationSystem;
using TProcedure = HereticalSolutions.Modules.Core_DefaultECS.IEntityInitializationSystem;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public class RootViewWorldInitializationSystemsInstaller
		: IProcedurePackageInstaller<TSystem, TProcedure>
	{
		private readonly EntityWorldController viewWorldController;

		private readonly ILoggerResolver loggerResolver;

		private readonly ILogger logger;

		public RootViewWorldInitializationSystemsInstaller(
			EntityWorldController viewWorldController,
			ILoggerResolver loggerResolver,
			ILogger logger)
		{
			this.viewWorldController = viewWorldController;

			this.loggerResolver = loggerResolver;

			this.logger = logger;
		}

		#region ISystemInstaller

		public string PackageName => SystemConstsDefaultECS.VIEW_WORLD_INITIALIZATION_PACKAGE_INSTALLER_ID;

		public IEnumerable<string> PackageDependencies => new string[0];

		public bool CanInstall(
			IProcedurePackageInstallerContext<TSystem, TProcedure> context)
		{
			if (context.Builders.Has(SystemConstsDefaultECS.VIEW_WORLD_INITIALIZATION_SYSTEM_BUILDER_ID))
				return false;

			return true;
		}

		public void Install(
			IProcedurePackageInstallerContext<TSystem, TProcedure> context)
		{
			var systemBuilder = DefaultECSSystemFactory.BuildDefaultECSSystemBuilder<TSystem, TProcedure>(
				loggerResolver);

			if (!context.Builders.TryAdd(
				SystemConstsDefaultECS.VIEW_WORLD_INITIALIZATION_SYSTEM_BUILDER_ID,
				systemBuilder))
			{
				logger?.LogError(
					GetType(),
					$"FAILED TO ADD SYSTEM BUILDER {SystemConstsDefaultECS.VIEW_WORLD_INITIALIZATION_SYSTEM_BUILDER_ID}");

				return;
			}

			systemBuilder.AddStageNodesBetweenStartAndFinish(
				SystemConstsDefaultECS.VIEW_WORLD_INITIALIZATION_SYSTEMS_ID);

			systemBuilder.AddStageNodesAfterStageStart(
				SystemConstsDefaultECS.PRESENTER_INITIALIZATION_SYSTEMS_ID,
				SystemConstsDefaultECS.VIEW_WORLD_INITIALIZATION_SYSTEMS_ID,
				false);

			//viewWorldController.EntityInitializationSystems = systemBuilder.BuildSystem();
		}

		#endregion
	}
}