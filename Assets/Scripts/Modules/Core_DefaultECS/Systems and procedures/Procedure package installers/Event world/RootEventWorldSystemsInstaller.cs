using System.Collections.Generic;

using HereticalSolutions.Systems;
using HereticalSolutions.Systems.Factories;

using HereticalSolutions.Modules.Core_DefaultECS.Factories;

using HereticalSolutions.Logging;

using TSystem = DefaultEcs.System.ISystem<float>;
using TProcedure = DefaultEcs.System.ISystem<float>;
using TWorld = DefaultEcs.World;


namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public class RootEventWorldInitializationSystemsInstaller
		: IProcedurePackageInstaller<TSystem, TProcedure>
	{
		private readonly TWorld eventWorld;

		private readonly ILoggerResolver loggerResolver;

		private readonly ILogger logger;

		public RootEventWorldInitializationSystemsInstaller(
			TWorld eventWorld,
			ILoggerResolver loggerResolver,
			ILogger logger)
		{
			this.eventWorld = eventWorld;

			this.loggerResolver = loggerResolver;

			this.logger = logger;
		}

		#region ISystemInstaller

		public string PackageName => SystemConstsDefaultECS.EVENT_WORLD_PACKAGE_INSTALLER_ID;

		public IEnumerable<string> PackageDependencies => new string[0];

		public bool CanInstall(
			IProcedurePackageInstallerContext<TSystem, TProcedure> context)
		{
			if (context.Builders.Has(SystemConstsDefaultECS.EVENT_WORLD_SYSTEM_BUILDER_ID))
				return false;

			return true;
		}

		public void Install(
			IProcedurePackageInstallerContext<TSystem, TProcedure> context)
		{
			var systemBuilder = DefaultECSSystemFactory.BuildDefaultECSSystemBuilder<TSystem, TProcedure>(
				loggerResolver);

			if (!context.Builders.TryAdd(
				SystemConstsDefaultECS.EVENT_WORLD_SYSTEM_BUILDER_ID,
				systemBuilder))
			{
				logger?.LogError(
					GetType(),
					$"FAILED TO ADD SYSTEM BUILDER {SystemConstsDefaultECS.EVENT_WORLD_SYSTEM_BUILDER_ID}");

				return;
			}

			systemBuilder.AddStageNodesBetweenStartAndFinish(
				SystemConstsDefaultECS.EVENT_WORLD_SYSTEMS_ID);

			//var despawnSystemNode = systemBuilder.AddLifetimeSystemBeforeStageFinish<TSystem, TProcedure>(
			//	SystemConstsDefaultECS.EVENT_WORLD_LIFETIME_SYSTEMS_ID,
			//	SystemConstsDefaultECS.EVENT_WORLD_SYSTEMS_ID,
			//	new DespawnSystem(
			//		eventWorld),
			//	false);
			//
			//systemBuilder.TryAddBeforeNode(
			//	despawnSystemNode,
			//	SystemFactory.BuildProcedureNode<TProcedure>(
			//		new DisposeProcessedEventsSystem<float>(
			//			eventWorld)),
			//	false);
		}

		#endregion
	}
}