using System.Collections.Generic;

using HereticalSolutions.Systems;
using HereticalSolutions.Systems.Factories;

using HereticalSolutions.Logging;

using TSystem = DefaultEcs.System.ISystem<float>;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public class RootLateUpdateSystemsInstaller
		: ISystemInstaller<TSystem>
	{
		private readonly ILogger logger;

		public RootLateUpdateSystemsInstaller(
			ILogger logger = null)
		{
			this.logger = logger;
		}

		#region ISystemInstaller

		public string Name => SystemConstsUnity.LATE_UPDATE_ROOT_INSTALLER_ID;

		public IEnumerable<string> Dependencies => new string[0];

		public bool CanInstall(
			ISystemInstallerContext<TSystem> context)
		{
			if (context.Builders.Has(SystemConstsUnity.LATE_UPDATE_SYSTEM_BUILDER_ID))
				return false;

			return true;
		}

		public void Install(
			ISystemInstallerContext<TSystem> context)
		{
			var systemBuilder = SystemFactory.BuildSystemBuilder<TSystem>();

			if (!context.Builders.TryAdd(
				SystemConstsUnity.LATE_UPDATE_SYSTEM_BUILDER_ID,
				systemBuilder))
			{
				logger?.LogError(
					GetType(),
					$"FAILED TO ADD SYSTEM BUILDER {SystemConstsUnity.LATE_UPDATE_SYSTEM_BUILDER_ID}");

				return;
			}

			systemBuilder.AddStageNodesBetweenStartAndFinish(
				SystemConstsUnity.LATE_UPDATE_SYSTEMS_ID);

			systemBuilder.AddStageNodesAfterStage<TSystem>(
				SystemConstsUnity.VIEW_INPUT_SYSTEMS_ID,
				SystemConstsUnity.LATE_UPDATE_SYSTEMS_ID,
				false);

			systemBuilder.AddStageNodesAfterStage<TSystem>(
				SystemConstsUnity.VIEW_PRESENTER_SYSTEMS_ID,
				SystemConstsUnity.VIEW_INPUT_SYSTEMS_ID,
				false);

			systemBuilder.AddStageNodesAfterStage<TSystem>(
				SystemConstsUnity.VIEW_VISUAL_SYSTEMS_ID,
				SystemConstsUnity.VIEW_PRESENTER_SYSTEMS_ID,
				false);
		}

		#endregion
	}
}