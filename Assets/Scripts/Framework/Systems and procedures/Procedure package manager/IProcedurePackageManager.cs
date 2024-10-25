using System.Collections.Generic;

namespace HereticalSolutions.Systems
{
	public interface IProcedurePackageManager<TSystem, TProcedure>
	{
		#region Has

		bool HasPackageInstaller(
			string installerName);

		#endregion

		#region Add

		bool TryAddPackageInstaller(
			IProcedurePackageInstaller<TSystem, TProcedure> installer);

		#endregion

		#region Remove

		bool TryRemovePackageInstaller(
			string installerName);

		#endregion

		//Sort by dependencies, validate then run the ones that have reported CanInstall == true
		bool RunAllInstallers();
	}
}