using System;
using System.IO;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public static class IOHelpers
	{
		public static void EnsureDirectoryExists(
			string path,
			ILogger logger = null,
			Type callerType = null)
		{
			if (string.IsNullOrEmpty(path))
				throw new Exception(
					logger.TryFormatException(
						callerType,
						"INVALID PATH"));

			string directoryPath = Path.GetDirectoryName(path);

			if (string.IsNullOrEmpty(directoryPath))
				throw new Exception(
					logger.TryFormatException(
						callerType,
						$"INVALID DIRECTORY PATH: {directoryPath}"));

			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}
		}

		public static bool FileExists(
			string path,
			ILogger logger = null,
			Type callerType = null)
		{
			if (string.IsNullOrEmpty(path))
				throw new Exception(
					logger.TryFormatException(
						callerType,
						"INVALID PATH"));

			string directoryPath = Path.GetDirectoryName(path);

			if (string.IsNullOrEmpty(directoryPath))
				throw new Exception(
					logger.TryFormatException(
						callerType,
						"INVALID DIRECTORY PATH"));

			if (!Directory.Exists(directoryPath))
			{
				return false;
			}

			return File.Exists(path);
		}
	}
}