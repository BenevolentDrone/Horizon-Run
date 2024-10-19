using System.Collections.Generic;

namespace HereticalSolutions.Systems
{
	public interface IProcedurePackageInstaller<TSystem, TProcedure>
	{
		string PackageName { get; }

		IEnumerable<string> PackageDependencies { get; }

		bool CanInstall(
			IProcedurePackageInstallerContext<TSystem, TProcedure> context);

		void Install(
			IProcedurePackageInstallerContext<TSystem, TProcedure> context);
	}
}