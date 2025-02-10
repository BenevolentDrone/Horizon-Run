using System.IO;
using System.IO.IsolatedStorage;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.isolatedstorage.isolatedstoragefile.createdirectory?view=net-9.0&redirectedfrom=MSDN#System_IO_IsolatedStorage_IsolatedStorageFile_CreateDirectory_System_String_
	[SerializationStrategy]
	public class IsolatedStorageStrategy
		: AStreamStrategy<IsolatedStorageFileStream>,
		  IHasIODestination
	{
		private readonly bool flushAutomatically;

		public string FullPath { get; private set; }
		

		private IsolatedStorageFile isolatedStorageFile;

		public IsolatedStorageFile IsolatedStorageFile => isolatedStorageFile;


		public IsolatedStorageFileStream FileStream => stream;


		public IsolatedStorageStrategy(
			string fullPath,
			
			ILogger logger,

			bool flushAutomatically = true)
			: base(
				logger)
		{
			FullPath = fullPath;

			this.flushAutomatically = flushAutomatically;


			//isolatedStorageFile = default(IsolatedStorageFile);

			//This should be done HERE because the IStrategyWithIODestination methods are all expecting for the storage
			//to be initialized before they are called
			//TODO: cleanup on destruction
			//Courtesy of https://discussions.unity.com/t/isolatedstorage-no-applicationidentity-available-for-appdomain/571104/2
			isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
		}

		#region IHasIODestination

		public void EnsureIODestinationExists()
		{
			IOHelpers.EnsureDirectoryInIsolatedStorageExists(
				FullPath,
				isolatedStorageFile,
				logger);

			if (!IOHelpers.FileInIsolatedStorageExists(
				FullPath,
				isolatedStorageFile,
				logger))
			{
				isolatedStorageFile.CreateFile(
					FullPath);
			}
		}

		public bool IODestinationExists()
		{
			return IOHelpers.FileInIsolatedStorageExists(
				FullPath,
				isolatedStorageFile,
				logger);
		}

		public void CreateIODestination()
		{
			EraseIODestination();

			IOHelpers.EnsureDirectoryInIsolatedStorageExists(
				FullPath,
				isolatedStorageFile,
				logger);

			isolatedStorageFile.CreateFile(
				FullPath);
		}

		public void EraseIODestination()
		{
			if (IOHelpers.FileInIsolatedStorageExists(
				FullPath,
				isolatedStorageFile,
				logger))
			{
				isolatedStorageFile.DeleteFile(
					FullPath);
			}
		}

		#endregion

		public override bool FlushAutomatically => flushAutomatically;

		protected override bool OpenReadStream(
			out IsolatedStorageFileStream dataStream)
		{
			dataStream = default(IsolatedStorageFileStream);

			if (!IOHelpers.FileInIsolatedStorageExists(
				FullPath,
				isolatedStorageFile,
				logger))
			{
				return false;
			}

			dataStream = new IsolatedStorageFileStream(
				FullPath,
				FileMode.Open,
				FileAccess.Read);

			return true;
		}

		protected override bool OpenWriteStream(
			out IsolatedStorageFileStream dataStream)
		{
			IOHelpers.EnsureDirectoryInIsolatedStorageExists(
				FullPath,
				isolatedStorageFile,
				logger);

			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filemode?view=net-8.0
			dataStream = new IsolatedStorageFileStream(
				FullPath,
				FileMode.Create,
				FileAccess.Write);

			return true;
		}

		protected override bool OpenAppendStream(
			out IsolatedStorageFileStream dataStream)
		{
			IOHelpers.EnsureDirectoryInIsolatedStorageExists(
				FullPath,
				isolatedStorageFile,
				logger);

			//Courtesy of https://stackoverflow.com/questions/7306214/append-lines-to-a-file-using-a-fileStream
			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filemode?view=net-8.0
			dataStream = new IsolatedStorageFileStream(
				FullPath,
				FileMode.Append,
				FileAccess.Write);

			return true;
		}

		protected override bool OpenReadWriteStream(
			out IsolatedStorageFileStream dataStream)
		{
			IOHelpers.EnsureDirectoryInIsolatedStorageExists(
				FullPath,
				isolatedStorageFile,
				logger);

			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filemode?view=net-8.0
			dataStream = new IsolatedStorageFileStream(
				FullPath,
				FileMode.OpenOrCreate,
				FileAccess.ReadWrite);

			return true;
		}
	}
}