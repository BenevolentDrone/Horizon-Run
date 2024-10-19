using HereticalSolutions.Repositories;

namespace HereticalSolutions.Systems
{
	public interface IProcedurePackageInstallerContext<TSystem, TProcedure>
	{
		IRepository<string, ISystemBuilder<TSystem, TProcedure>> Builders { get; }

		#region Has

		bool HasInstaller(
			string installerName);

		#endregion

		#region Add

		bool TryAddInstaller(
			IProcedurePackageInstaller<TSystem, TProcedure> installer);

		#endregion

		#region Remove

		bool TryRemoveInstaller(
			string installerName);

		#endregion

		//Sort by dependencies, validate then run the ones that have reported CanInstall == true
		bool RunAllInstallers();
	}
}