using System;
using System.IO;
using System.IO.IsolatedStorage;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public static class IOHelpers
	{
		public static void EnsureDirectoryExists(
			string path,
			ILogger logger)
		{
			if (string.IsNullOrEmpty(path))
				throw new Exception(
					logger.TryFormatException(
						"INVALID PATH"));

			string directoryPath = Path.GetDirectoryName(path);

			if (string.IsNullOrEmpty(directoryPath))
				throw new Exception(
					logger.TryFormatException(
						$"INVALID DIRECTORY PATH: {directoryPath}"));

			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}
		}

		public static bool FileExists(
			string path,
			ILogger logger)
		{
			if (string.IsNullOrEmpty(path))
				throw new Exception(
					logger.TryFormatException(
						"INVALID PATH"));

			string directoryPath = Path.GetDirectoryName(path);

			if (string.IsNullOrEmpty(directoryPath))
				throw new Exception(
					logger.TryFormatException(
						"INVALID DIRECTORY PATH"));

			if (!Directory.Exists(directoryPath))
			{
				return false;
			}

			return File.Exists(path);
		}

		public static void EnsureDirectoryInIsolatedStorageExists(
			string path,
			IsolatedStorageFile isolatedStorageFile,
			ILogger logger)
		{
			if (string.IsNullOrEmpty(path))
				throw new Exception(
					logger.TryFormatException(
						"INVALID PATH"));

			string directoryPath = Path.GetDirectoryName(path);

			if (string.IsNullOrEmpty(directoryPath))
				throw new Exception(
					logger.TryFormatException(
						$"INVALID DIRECTORY PATH: {directoryPath}"));

			bool directoryExists = isolatedStorageFile.DirectoryExists(directoryPath);

			if (!directoryExists)
			{
				isolatedStorageFile.CreateDirectory(directoryPath);
			}
		}

		public static bool FileInIsolatedStorageExists(
			string path,
			IsolatedStorageFile isolatedStorageFile,
			ILogger logger)
		{
			if (string.IsNullOrEmpty(path))
				throw new Exception(
					logger.TryFormatException(
						"INVALID PATH"));

			return isolatedStorageFile.FileExists(path);
		}
	}
}