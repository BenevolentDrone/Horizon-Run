using System;

namespace HereticalSolutions.Entities
{
	[Serializable]
	public class LoggingSettings
	{
		public bool EnableLoggingByDefault = true;

		public bool LogTimeInUtc = true;
	}
}