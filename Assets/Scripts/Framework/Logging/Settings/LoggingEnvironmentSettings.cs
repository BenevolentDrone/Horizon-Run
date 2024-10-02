using System;

namespace HereticalSolutions.Entities
{
	[Serializable]
	public class LoggingEnvironmentSettings
	{
		public bool GetLogsFolderFromEnvironment = false;

		public string LogsFolderEnvironmentKey;
	}
}