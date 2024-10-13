using System.Collections.Generic;

namespace HereticalSolutions.Systems
{
	public interface ISystemInstaller<TSystem>
	{
		string Name { get; }

		IEnumerable<string> Dependencies { get; }

		bool CanInstall(
			ISystemInstallerContext<TSystem> context);

		void Install(
			ISystemInstallerContext<TSystem> context);
	}
}