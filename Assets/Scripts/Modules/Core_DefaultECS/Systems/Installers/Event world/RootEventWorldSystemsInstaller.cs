using System.Collections.Generic;

using HereticalSolutions.Systems;
using HereticalSolutions.Systems.Factories;

using HereticalSolutions.Logging;

using TSystem = DefaultEcs.System.ISystem<float>;
using TWorld = DefaultEcs.World;


namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public class RootEventWorldInitializationSystemsInstaller
		: ISystemInstaller<TSystem>
	{
		private readonly TWorld eventWorld;

		private readonly ILogger logger;

		public RootEventWorldInitializationSystemsInstaller(
			TWorld eventWorld,
			ILogger logger = null)
		{
			this.eventWorld = eventWorld;

			this.logger = logger;
		}

		#region ISystemInstaller

		public string Name => SystemConstsDefaultECS.EVENT_WORLD_ROOT_INSTALLER_ID;

		public IEnumerable<string> Dependencies => new string[0];

		public bool CanInstall(
			ISystemInstallerContext<TSystem> context)
		{
			if (context.Builders.Has(SystemConstsDefaultECS.EVENT_WORLD_SYSTEM_BUILDER_ID))
				return false;

			return true;
		}

		public void Install(
			ISystemInstallerContext<TSystem> context)
		{
			var systemBuilder = SystemFactory.BuildSystemBuilder<TSystem>();

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

			var despawnSystemNode = systemBuilder.AddLifetimeSystemBeforeStageFinish<TSystem>(
				SystemConstsDefaultECS.EVENT_WORLD_LIFETIME_SYSTEMS_ID,
				SystemConstsDefaultECS.EVENT_WORLD_SYSTEMS_ID,
				new DespawnSystem(
					eventWorld),
				false);

			systemBuilder.TryAddBeforeNode(
				despawnSystemNode,
				SystemFactory.BuildSystemNode<TSystem>(
					new DisposeProcessedEventsSystem<float>(
						eventWorld)),
				false);
		}

		#endregion
	}
}