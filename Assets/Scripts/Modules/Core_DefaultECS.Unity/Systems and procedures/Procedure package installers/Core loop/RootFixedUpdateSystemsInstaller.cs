using System.Collections.Generic;

using HereticalSolutions.Systems;
using HereticalSolutions.Systems.Factories;

using HereticalSolutions.Modules.Core_DefaultECS.Factories;

using HereticalSolutions.Logging;

using TSystem = DefaultEcs.System.ISystem<float>;
using TProcedure = DefaultEcs.System.ISystem<float>;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public class RootFixedUpdateSystemsInstaller
		: IProcedurePackageInstaller<TSystem, TProcedure>
	{
		private readonly ILoggerResolver loggerResolver;

		private readonly ILogger logger;

		public RootFixedUpdateSystemsInstaller(
			ILoggerResolver loggerResolver,
			ILogger logger)
		{
			this.loggerResolver = loggerResolver;

			this.logger = logger;
		}

		#region ISystemInstaller

		public string PackageName => SystemConstsUnity.FIXED_UPDATE_PACKAGE_INSTALLER_ID;

		public IEnumerable<string> PackageDependencies => new string[0];

		public bool CanInstall(
			IProcedurePackageInstallerContext<TSystem, TProcedure> context)
		{
			if (context.Builders.Has(SystemConstsUnity.FIXED_UPDATE_SYSTEM_BUILDER_ID))
				return false;

			return true;
		}

		public void Install(
			IProcedurePackageInstallerContext<TSystem, TProcedure> context)
		{
			var systemBuilder = DefaultECSSystemFactory.BuildDefaultECSSystemBuilder<TSystem, TProcedure>(
				loggerResolver);

			if (!context.Builders.TryAdd(
				SystemConstsUnity.FIXED_UPDATE_SYSTEM_BUILDER_ID,
				systemBuilder))
			{
				logger?.LogError(
					GetType(),
					$"FAILED TO ADD SYSTEM BUILDER {SystemConstsUnity.FIXED_UPDATE_SYSTEM_BUILDER_ID}");

				return;
			}

			systemBuilder.AddStageNodesBetweenStartAndFinish(
				SystemConstsUnity.FIXED_UPDATE_SYSTEMS_ID);
		}

		#endregion
	}
}