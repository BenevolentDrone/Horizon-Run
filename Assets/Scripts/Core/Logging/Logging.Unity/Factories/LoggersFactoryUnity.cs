using System;
using System.Collections.Generic;

using HereticalSolutions.Persistence.Arguments;

using HereticalSolutions.Persistence.IO;

using HereticalSolutions.Persistence.Factories;

using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Logging.Factories
{
	public static class LoggersFactoryUnity
	{
		public static UnityDebugLogger BuildUnityDebugLogger()
		{
			return new UnityDebugLogger();
		}
	}
}