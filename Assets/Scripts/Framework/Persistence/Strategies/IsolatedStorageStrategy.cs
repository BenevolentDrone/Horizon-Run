using System;
using System.Threading.Tasks;

using System.IO;
using System.IO.IsolatedStorage;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.isolatedstorage.isolatedstoragefile.createdirectory?view=net-9.0&redirectedfrom=MSDN#System_IO_IsolatedStorage_IsolatedStorageFile_CreateDirectory_System_String_
	[SerializationStrategy]
	public class IsolatedStorageStrategy
		: ISerializationStrategy,
		  IAsyncSerializationStrategy,
		  IStrategyWithIODestination,
		  IStrategyWithStream,
		  IStrategyWithState,
		  IBlockSerializationStrategy,
		  IAsyncBlockSerializationStrategy,
		  IStrategyWithFilter
	{
		private readonly bool flushAutomatically;

		private readonly ILogger logger;


		public string FullPath { get; private set; }
		

		private IsolatedStorageFile isolatedStorageFile;

		public IsolatedStorageFile IsolatedStorageFile { get { return isolatedStorageFile; } }


		private IsolatedStorageFileStream fileStream;

		public IsolatedStorageFileStream FileStream { get { return fileStream; } }


		public IsolatedStorageStrategy(
			string fullPath,
			
			ILogger logger,

			bool flushAutomatically = true)
		{
			FullPath = fullPath;

			this.flushAutomatically = flushAutomatically;

			this.logger = logger;


			CurrentMode = EStreamMode.NONE;

			StreamOpen = false;

			//isolatedStorageFile = default(IsolatedStorageFile);

			fileStream = default(IsolatedStorageFileStream);


			//This should be done HERE because the IStrategyWithIODestination methods are all expecting for the storage
			//to be initialized before they are called
			//TODO: cleanup on destruction
			//Courtesy of https://discussions.unity.com/t/isolatedstorage-no-applicationidentity-available-for-appdomain/571104/2
			isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication();
		}


		#region ISerializationStrategy

		#region Read

		public bool Read<TValue>(
			out TValue value)
		{
			AssertStrategyIsValid(
				typeof(TValue),
				EStreamMode.READ);

			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

			// Read the source file into a byte array.
			byte[] result = new byte[fileStream.Length];

			int numBytesToRead = (int)fileStream.Length;

			int numBytesRead = 0;

			while (numBytesToRead > 0)
			{
				// Read may return anything from 0 to numBytesToRead.
				int n = fileStream.Read(
					result,
					numBytesRead,
					numBytesToRead);

				// Break when the end of the file is reached.
				if (n == 0)
					break;

				numBytesRead += n;

				numBytesToRead -= n;
			}

			numBytesToRead = result.Length;

			value = result.CastFromTo<byte[], TValue>();

			return true;
		}

		public bool Read(
			Type valueType,
			out object value)
		{
			AssertStrategyIsValid(
				valueType,
				EStreamMode.READ);

			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

			// Read the source file into a byte array.
			byte[] result = new byte[fileStream.Length];

			int numBytesToRead = (int)fileStream.Length;

			int numBytesRead = 0;

			while (numBytesToRead > 0)
			{
				// Read may return anything from 0 to numBytesToRead.
				int n = fileStream.Read(
					result,
					numBytesRead,
					numBytesToRead);

				// Break when the end of the file is reached.
				if (n == 0)
					break;

				numBytesRead += n;

				numBytesToRead -= n;
			}

			numBytesToRead = result.Length;

			value = result;

			return true;
		}

		#endregion

		#region Write

		public bool Write<TValue>(
			TValue value)
		{
			AssertStrategyIsValid(
				typeof(TValue),
				EStreamMode.WRITE);

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

			int numBytesToWrite = contents.Length;

			// Write the byte array to the other FileStream.
			fileStream.Write(
				contents,
				0,
				numBytesToWrite);

			if (flushAutomatically)
				Flush();

			return true;
		}

		public bool Write(
			Type valueType,
			object value)
		{
			AssertStrategyIsValid(
				valueType,
				EStreamMode.WRITE);

			byte[] contents = value.CastFromTo<object, byte[]>();

			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

			int numBytesToWrite = contents.Length;

			// Write the byte array to the other FileStream.
			fileStream.Write(
				contents,
				0,
				numBytesToWrite);

			if (flushAutomatically)
				Flush();

			return true;
		}

		#endregion

		#region Append

		public bool Append<TValue>(
			TValue value)
		{
			AssertStrategyIsValid(
				typeof(TValue),
				EStreamMode.APPEND);

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

			int numBytesToWrite = contents.Length;

			// Write the byte array to the other FileStream.
			fileStream.Write(
				contents,
				0,
				numBytesToWrite);

			if (flushAutomatically)
				Flush();

			return true;
		}

		public bool Append(
			Type valueType,
			object value)
		{
			AssertStrategyIsValid(
				valueType,
				EStreamMode.READ);

			byte[] contents = value.CastFromTo<object, byte[]>();

			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

			int numBytesToWrite = contents.Length;

			// Write the byte array to the other FileStream.
			fileStream.Write(
				contents,
				0,
				numBytesToWrite);

			if (flushAutomatically)
				Flush();

			return true;
		}

		#endregion

		#endregion

		#region IAsyncSerializationStrategy

		#region Read

		public async Task<(bool, TValue)> ReadAsync<TValue>(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			AssertStrategyIsValid(
				typeof(TValue),
				EStreamMode.READ);

			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

			// Read the source file into a byte array.
			byte[] result = new byte[fileStream.Length];

			int numBytesToRead = (int)fileStream.Length;

			int numBytesRead = 0;

			while (numBytesToRead > 0)
			{
				// Read may return anything from 0 to numBytesToRead.
				int n = await fileStream.ReadAsync(
					result,
					numBytesRead,
					numBytesToRead);

				// Break when the end of the file is reached.
				if (n == 0)
					break;

				numBytesRead += n;

				numBytesToRead -= n;
			}

			numBytesToRead = result.Length;

			TValue value = result.CastFromTo<byte[], TValue>();

			return (true, value);
		}

		public async Task<(bool, object)> ReadAsync(
			Type valueType,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			AssertStrategyIsValid(
				valueType,
				EStreamMode.READ);

			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

			// Read the source file into a byte array.
			byte[] result = new byte[fileStream.Length];

			int numBytesToRead = (int)fileStream.Length;

			int numBytesRead = 0;

			while (numBytesToRead > 0)
			{
				// Read may return anything from 0 to numBytesToRead.
				int n = await fileStream.ReadAsync(
					result,
					numBytesRead,
					numBytesToRead);

				// Break when the end of the file is reached.
				if (n == 0)
					break;

				numBytesRead += n;

				numBytesToRead -= n;
			}

			numBytesToRead = result.Length;

			object value = result;

			return (true, value);
		}

		#endregion

		#region Write

		public async Task<bool> WriteAsync<TValue>(
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			AssertStrategyIsValid(
				typeof(TValue),
				EStreamMode.WRITE);

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

			int numBytesToWrite = contents.Length;

			// Write the byte array to the other FileStream.
			await fileStream.WriteAsync(
				contents,
				0,
				numBytesToWrite);

			if (flushAutomatically)
				await FlushAsync(
					asyncContext);

			return true;
		}

		public async Task<bool> WriteAsync(
			Type valueType,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			AssertStrategyIsValid(
				valueType,
				EStreamMode.WRITE);

			byte[] contents = value.CastFromTo<object, byte[]>();

			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

			int numBytesToWrite = contents.Length;

			// Write the byte array to the other FileStream.
			await fileStream.WriteAsync(
				contents,
				0,
				numBytesToWrite);

			if (flushAutomatically)
				await FlushAsync(
					asyncContext);

			return true;
		}

		#endregion

		#region Append

		public async Task<bool> AppendAsync<TValue>(
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			AssertStrategyIsValid(
				typeof(TValue),
				EStreamMode.APPEND);

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

			int numBytesToWrite = contents.Length;

			// Write the byte array to the other FileStream.
			await fileStream.WriteAsync(
				contents,
				0,
				numBytesToWrite);

			if (flushAutomatically)
				await FlushAsync(
					asyncContext);

			return true;
		}

		public async Task<bool> AppendAsync(
			Type valueType,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			AssertStrategyIsValid(
				valueType,
				EStreamMode.APPEND);

			byte[] contents = value.CastFromTo<object, byte[]>();

			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

			int numBytesToWrite = contents.Length;

			// Write the byte array to the other FileStream.
			await fileStream.WriteAsync(
				contents,
				0,
				numBytesToWrite);

			if (flushAutomatically)
				await FlushAsync(
					asyncContext);

			return true;
		}

		#endregion

		#endregion

		#region IStrategyWithIODestination

		public void EnsureIOTargetDestinationExists()
		{
			IOHelpers.EnsureDirectoryInIsolatedStorageExists(
				FullPath,
				isolatedStorageFile,
				logger,
				GetType());
		}

		public bool IOTargetExists()
		{
			return IOHelpers.FileInIsolatedStorageExists(
				FullPath,
				isolatedStorageFile,
				logger,
				GetType());
		}

		public void CreateIOTarget()
		{
			string savePath = FullPath;

			IOHelpers.EnsureDirectoryInIsolatedStorageExists(
				FullPath,
				isolatedStorageFile,
				logger,
				GetType());

			if (!IOHelpers.FileInIsolatedStorageExists(
				savePath,
				isolatedStorageFile,
				logger,
				GetType()))
			{
				isolatedStorageFile.CreateFile(
					FullPath);
			}
		}

		public void EraseIOTarget()
		{
			string savePath = FullPath;

			if (IOHelpers.FileInIsolatedStorageExists(
				savePath,
				isolatedStorageFile,
				logger,
				GetType()))
			{
				isolatedStorageFile.DeleteFile(savePath);
			}
		}

		#endregion

		#region IStrategyWithStream

		public EStreamMode CurrentMode { get; private set; }

		public Stream Stream
		{
			get
			{
				switch (CurrentMode)
				{
					case EStreamMode.READ:
					case EStreamMode.WRITE:
					case EStreamMode.APPEND:
					case EStreamMode.READ_AND_WRITE:
						return fileStream;

					default:
						return null;
				}
			}
		}

		public bool StreamOpen { get; private set; }

		#region Flush

		public bool FlushAutomatically { get => flushAutomatically; }

		public void Flush()
		{
			if (!StreamOpen)
				return;

			if (fileStream != null)
			{
				fileStream.Flush(true);
			}
		}

		public async Task FlushAsync(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (!StreamOpen)
				return;

			if (fileStream != null)
			{
				await fileStream.FlushAsync();
			}
		}

		#endregion


		#region Seek

		public long Position
		{
			get
			{
				if (!StreamOpen)
					return -1;

				if ((CurrentMode == EStreamMode.READ || CurrentMode == EStreamMode.WRITE || CurrentMode == EStreamMode.APPEND || CurrentMode == EStreamMode.READ_AND_WRITE)
					&& fileStream != null)
				{
					return fileStream.Position;
				}

				return -1;
			}
		}

		public bool CanSeek
		{
			get
			{
				if (!StreamOpen)
					return false;

				if ((CurrentMode == EStreamMode.READ || CurrentMode == EStreamMode.WRITE || CurrentMode == EStreamMode.APPEND || CurrentMode == EStreamMode.READ_AND_WRITE)
					&& fileStream != null)
				{
					return fileStream.CanSeek;
				}

				return false;
			}
		}

		public bool Seek(
			long offset,
			out long position)
		{
			if (!StreamOpen)
			{
				position = -1;

				return false;
			}

			if ((CurrentMode == EStreamMode.READ || CurrentMode == EStreamMode.WRITE || CurrentMode == EStreamMode.APPEND || CurrentMode == EStreamMode.READ_AND_WRITE)
				&& fileStream != null)
			{
				position = fileStream.Seek(
					offset,
					SeekOrigin.Current);

				return true;
			}

			position = -1;

			return false;
		}

		public bool SeekFromStart(
			long offset,
			out long position)
		{
			if (!StreamOpen)
			{
				position = -1;

				return false;
			}

			if ((CurrentMode == EStreamMode.READ || CurrentMode == EStreamMode.WRITE || CurrentMode == EStreamMode.APPEND || CurrentMode == EStreamMode.READ_AND_WRITE)
				&& fileStream != null)
			{
				position = fileStream.Seek(
					offset,
					SeekOrigin.Begin);

				return true;
			}

			position = -1;

			return false;
		}

		public bool SeekFromFinish(
			long offset,
			out long position)
		{
			if (!StreamOpen)
			{
				position = -1;

				return false;
			}

			if ((CurrentMode == EStreamMode.READ || CurrentMode == EStreamMode.WRITE || CurrentMode == EStreamMode.APPEND || CurrentMode == EStreamMode.READ_AND_WRITE)
				&& fileStream != null)
			{
				position = fileStream.Seek(
					offset,
					SeekOrigin.End);

				return true;
			}

			position = -1;

			return false;
		}

		#endregion

		#endregion

		#region IStrategyWithState

		public bool SupportsSimultaneousReadAndWrite { get => true; }

		public void InitializeRead()
		{
			if (StreamOpen)
				return;

			StreamOpen = OpenReadStream(
				out fileStream);

			if (!StreamOpen)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"FAILED TO OPEN STREAM: {FullPath}"));

			CurrentMode = EStreamMode.READ;
		}

		public void FinalizeRead()
		{
			if (!StreamOpen)
				return;

			if (CurrentMode != EStreamMode.READ)
				return;

			if (fileStream != null)
				CloseStream(
					fileStream);

			fileStream = default(IsolatedStorageFileStream);

			StreamOpen = false;

			CurrentMode = EStreamMode.NONE;
		}

		public void InitializeWrite()
		{
			if (StreamOpen)
				return;

			StreamOpen = OpenWriteStream(
				out fileStream);

			if (!StreamOpen)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"FAILED TO OPEN STREAM: {FullPath}"));

			CurrentMode = EStreamMode.WRITE;
		}

		public void FinalizeWrite()
		{
			if (!StreamOpen)
				return;

			if (CurrentMode != EStreamMode.WRITE)
				return;

			if (fileStream != null)
				CloseStream(
					fileStream);

			fileStream = default(IsolatedStorageFileStream);

			StreamOpen = false;

			CurrentMode = EStreamMode.NONE;
		}

		public void InitializeAppend()
		{
			if (StreamOpen)
				return;

			StreamOpen = OpenAppendStream(
				out fileStream);

			if (!StreamOpen)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"FAILED TO OPEN STREAM: {FullPath}"));

			CurrentMode = EStreamMode.APPEND;
		}

		public void FinalizeAppend()
		{
			if (!StreamOpen)
				return;

			if (CurrentMode != EStreamMode.APPEND)
				return;

			if (fileStream != null)
				CloseStream(
					fileStream);

			fileStream = default(IsolatedStorageFileStream);

			StreamOpen = false;

			CurrentMode = EStreamMode.NONE;
		}

		public void InitializeReadAndWrite()
		{
			if (StreamOpen)
				return;

			StreamOpen = OpenReadWriteStream(
				out fileStream);

			if (!StreamOpen)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"FAILED TO OPEN STREAM: {FullPath}"));

			CurrentMode = EStreamMode.READ_AND_WRITE;
		}

		public void FinalizeReadAndWrite()
		{
			if (!StreamOpen)
				return;

			if (CurrentMode != EStreamMode.READ_AND_WRITE)
				return;

			if (fileStream != null)
				CloseStream(
					fileStream);

			fileStream = default(IsolatedStorageFileStream);

			StreamOpen = false;

			CurrentMode = EStreamMode.NONE;
		}

		#endregion

		#region IBlockSerializationStrategy

		#region Read

		public bool BlockRead<TValue>(
			int blockOffset,
			int blockSize,
			out TValue value)
		{
			AssertStrategyIsValid(
				typeof(TValue),
				EStreamMode.READ);

			// Read the source file into a byte array.
			byte[] result = new byte[blockSize];

			int resultLength = fileStream.Read(
				result,
				blockOffset,
				blockSize);

			if (resultLength != blockSize)
			{
				byte[] resultTrimmed = new byte[resultLength];

				Array.Copy(
					result,
					resultTrimmed,
					resultLength);

				result = resultTrimmed;
			}

			value = result.CastFromTo<byte[], TValue>();

			return true;
		}

		public bool BlockRead(
			Type valueType,
			int blockOffset,
			int blockSize,
			out object value)
		{
			AssertStrategyIsValid(
				valueType,
				EStreamMode.READ);

			// Read the source file into a byte array.
			byte[] result = new byte[blockSize];

			int resultLength = fileStream.Read(
				result,
				blockOffset,
				blockSize);

			if (resultLength != blockSize)
			{
				byte[] resultTrimmed = new byte[resultLength];

				Array.Copy(
					result,
					resultTrimmed,
					resultLength);

				result = resultTrimmed;
			}

			value = result;

			return true;
		}

		#endregion

		#region Write

		public bool BlockWrite<TValue>(
			TValue value,
			int blockOffset,
			int blockSize)
		{
			AssertStrategyIsValid(
				typeof(TValue),
				EStreamMode.WRITE);

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			fileStream.Write(
				contents,
				blockOffset,
				blockSize);

			if (flushAutomatically)
				Flush();

			return true;
		}

		public bool BlockWrite(
			Type valueType,
			object value,
			int blockOffset,
			int blockSize)
		{
			AssertStrategyIsValid(
				valueType,
				EStreamMode.WRITE);

			byte[] contents = value.CastFromTo<object, byte[]>();

			fileStream.Write(
				contents,
				blockOffset,
				blockSize);

			if (flushAutomatically)
				Flush();

			return true;
		}

		#endregion

		#endregion

		#region IAsyncBlockSerializationStrategy

		#region Read

		public async Task<(bool, TValue)> BlockReadAsync<TValue>(
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			AssertStrategyIsValid(
				typeof(TValue),
				EStreamMode.READ);

			// Read the source file into a byte array.
			byte[] result = new byte[blockSize];

			int resultLength = await fileStream.ReadAsync(
				result,
				blockOffset,
				blockSize);

			if (resultLength != blockSize)
			{
				byte[] resultTrimmed = new byte[resultLength];

				Array.Copy(
					result,
					resultTrimmed,
					resultLength);

				result = resultTrimmed;
			}

			TValue value = result.CastFromTo<byte[], TValue>();

			return (true, value);
		}

		public async Task<(bool, object)> BlockReadAsync(
			Type valueType,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			AssertStrategyIsValid(
				valueType,
				EStreamMode.READ);

			// Read the source file into a byte array.
			byte[] result = new byte[blockSize];

			int resultLength = await fileStream.ReadAsync(
				result,
				blockOffset,
				blockSize);

			if (resultLength != blockSize)
			{
				byte[] resultTrimmed = new byte[resultLength];

				Array.Copy(
					result,
					resultTrimmed,
					resultLength);

				result = resultTrimmed;
			}

			object value = result;

			return (true, value);
		}

		#endregion

		#region Write

		public async Task<bool> BlockWriteAsync<TValue>(
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			AssertStrategyIsValid(
				typeof(TValue),
				EStreamMode.WRITE);

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			await fileStream.WriteAsync(
				contents,
				blockOffset,
				blockSize);

			if (flushAutomatically)
				await FlushAsync(
					asyncContext);

			return true;
		}

		public async Task<bool> BlockWriteAsync(
			Type valueType,
			object value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			AssertStrategyIsValid(
				valueType,
				EStreamMode.WRITE);

			byte[] contents = value.CastFromTo<object, byte[]>();

			await fileStream.WriteAsync(
				contents,
				blockOffset,
				blockSize);

			if (flushAutomatically)
				await FlushAsync(
					asyncContext);

			return true;
		}

		#endregion

		#endregion

		#region IStrategyWithFilter

		public bool AllowsType<TValue>()
		{
			return typeof(TValue) == typeof(byte[]);
		}

		public bool AllowsType(
			Type valueType)
		{
			return valueType == typeof(byte[]);
		}

		#endregion

		private void AssertStrategyIsValid(
			Type valueType,
			EStreamMode preferredMode)
		{
			if (valueType != typeof(byte[]))
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"INVALID VALUE TYPE: {valueType.Name}"));

			if (!StreamOpen)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"STREAM NOT OPEN: {FullPath}"));

			if (CurrentMode != preferredMode && CurrentMode != EStreamMode.READ_AND_WRITE)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"INVALID STREAM MODE: {CurrentMode}"));

			if (isolatedStorageFile == null)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"ISOLATED STORAGE FILE IS NULL: {FullPath}"));

			if (fileStream == null)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"FILE STREAM IS NULL: {FullPath}"));
		}

		private bool OpenReadStream(
			out IsolatedStorageFileStream dataStream)
		{
			dataStream = default(IsolatedStorageFileStream);

			if (!IOHelpers.FileInIsolatedStorageExists(
				FullPath,
				isolatedStorageFile,
				logger,
				GetType()))
			{
				return false;
			}

			dataStream = new IsolatedStorageFileStream(
				FullPath,
				FileMode.Open,
				FileAccess.Read);

			return true;
		}

		private bool OpenWriteStream(
			out IsolatedStorageFileStream dataStream)
		{
			IOHelpers.EnsureDirectoryInIsolatedStorageExists(
				FullPath,
				isolatedStorageFile,
				logger,
				GetType());

			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filemode?view=net-8.0
			dataStream = new IsolatedStorageFileStream(
				FullPath,
				FileMode.Create,
				FileAccess.Write);

			return true;
		}

		private bool OpenAppendStream(
			out IsolatedStorageFileStream dataStream)
		{
			IOHelpers.EnsureDirectoryInIsolatedStorageExists(
				FullPath,
				isolatedStorageFile,
				logger,
				GetType());

			//Courtesy of https://stackoverflow.com/questions/7306214/append-lines-to-a-file-using-a-fileStream
			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filemode?view=net-8.0
			dataStream = new IsolatedStorageFileStream(
				FullPath,
				FileMode.Append,
				FileAccess.Write);

			return true;
		}

		private bool OpenReadWriteStream(
			out IsolatedStorageFileStream dataStream)
		{
			IOHelpers.EnsureDirectoryInIsolatedStorageExists(
				FullPath,
				isolatedStorageFile,
				logger,
				GetType());

			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filemode?view=net-8.0
			dataStream = new IsolatedStorageFileStream(
				FullPath,
				FileMode.OpenOrCreate,
				FileAccess.ReadWrite);

			return true;
		}

		private void CloseStream(IsolatedStorageFileStream dataStream)
		{
			dataStream.Close();
		}
	}
}