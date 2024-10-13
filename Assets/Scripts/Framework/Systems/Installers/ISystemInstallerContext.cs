using HereticalSolutions.Repositories;

namespace HereticalSolutions.Systems
{
	public interface ISystemInstallerContext<TSystem>
	{
		IRepository<string, ISystemBuilder<TSystem>> Builders { get; }

		#region Has

		bool HasInstaller(
			string installerName);

		#endregion

		#region Add

		bool TryAddInstaller(
			ISystemInstaller<TSystem> installer);

		#endregion

		#region Remove

		bool TryRemoveInstaller(
			string installerName);

		#endregion

		//Sort by dependencies, validate then run the ones that have reported CanInstall == true
		bool RunAllInstallers();
	}
}