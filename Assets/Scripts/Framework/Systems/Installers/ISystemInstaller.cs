using System.Collections.Generic;

namespace HereticalSolutions.Systems
{
	public interface ISystemInstaller<TSystem>
	{
		string Name { get; }

		IEnumerable<string> Dependencies { get; }

		bool CanInstall(
			ISystemBuilder<TSystem> systemBuilder);

		void Install(
			ISystemBuilder<TSystem> systemBuilder);
	}
}