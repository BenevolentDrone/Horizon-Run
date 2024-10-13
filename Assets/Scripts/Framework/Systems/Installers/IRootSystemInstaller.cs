using System;
using System.Collections.Generic;

namespace HereticalSolutions.Systems
{
	public interface IRootSystemInstaller<TSystem>
		: ISystemInstaller<TSystem>
	{
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
	}
}