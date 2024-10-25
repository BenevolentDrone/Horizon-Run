using HereticalSolutions.Repositories;

namespace HereticalSolutions.Systems
{
	public interface IProcedurePackageInstallerContext<TSystem, TProcedure>
	{
		IRepository<string, ISystemBuilder<TSystem, TProcedure>> Builders { get; }
	}
}