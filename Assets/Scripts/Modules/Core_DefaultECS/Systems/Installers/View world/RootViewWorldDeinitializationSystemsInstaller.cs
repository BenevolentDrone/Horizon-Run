using System.Collections.Generic;

using HereticalSolutions.Systems;
using HereticalSolutions.Systems.Factories;

using HereticalSolutions.Logging;

using TSystem = HereticalSolutions.Modules.Core_DefaultECS.IEntityInitializationSystem;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public class RootViewWorldDeinitializationSystemsInstaller
		: ISystemInstaller<TSystem>
	{
		private readonly EntityWorldController viewWorldController;

		private readonly ILogger logger;

		public RootViewWorldDeinitializationSystemsInstaller(
			EntityWorldController viewWorldController,
			ILogger logger = null)
		{
			this.viewWorldController = viewWorldController;

			this.logger = logger;
		}

		#region ISystemInstaller

		public string Name => SystemConstsDefaultECS.VIEW_WORLD_DEINITIALIZATION_ROOT_INSTALLER_ID;

		public IEnumerable<string> Dependencies => new string[0];

		public bool CanInstall(
			ISystemInstallerContext<TSystem> context)
		{
			if (context.Builders.Has(SystemConstsDefaultECS.VIEW_WORLD_DEINITIALIZATION_SYSTEM_BUILDER_ID))
				return false;

			return true;
		}

		public void Install(
			ISystemInstallerContext<TSystem> context)
		{
			var systemBuilder = SystemFactory.BuildSystemBuilder<TSystem>();

			if (!context.Builders.TryAdd(
				SystemConstsDefaultECS.VIEW_WORLD_DEINITIALIZATION_SYSTEM_BUILDER_ID,
				systemBuilder))
			{
				logger?.LogError(
					GetType(),
					$"FAILED TO ADD SYSTEM BUILDER {SystemConstsDefaultECS.VIEW_WORLD_DEINITIALIZATION_SYSTEM_BUILDER_ID}");

				return;
			}

			systemBuilder.AddStageNodesBetweenStartAndFinish(
				SystemConstsDefaultECS.VIEW_WORLD_DEINITIALIZATION_SYSTEMS_ID);

			viewWorldController.EntityDeinitializationSystems = systemBuilder.BuildSystem();
		}

		#endregion
	}
}