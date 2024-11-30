using System;
using System.IO;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	[SerializationStrategy]
	public class MemoryStreamStrategy
		: ISerializationStrategy,
		  IAsyncSerializationStrategy,
		  IStrategyWithStream,
		  IStrategyWithState,
		  IBlockSerializationStrategy,
		  IAsyncBlockSerializationStrategy,
		  IStrategyWithFilter
	{
		private readonly ILogger logger;


		private MemoryStream memoryStream;

		public MemoryStream MemoryStream { get { return memoryStream; } }


		public MemoryStreamStrategy(
			ILogger logger = null)
		{
			this.logger = logger;


			CurrentMode = EStreamMode.NONE;

			StreamOpen = false;

			memoryStream = default(MemoryStream);
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
			byte[] result = new byte[memoryStream.Length];

			int numBytesToRead = (int)memoryStream.Length;

			int numBytesRead = 0;

			while (numBytesToRead > 0)
			{
				// Read may return anything from 0 to numBytesToRead.
				int n = memoryStream.Read(
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
			byte[] result = new byte[memoryStream.Length];

			int numBytesToRead = (int)memoryStream.Length;

			int numBytesRead = 0;

			while (numBytesToRead > 0)
			{
				// Read may return anything from 0 to numBytesToRead.
				int n = memoryStream.Read(
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

			// Write the byte array to the other MemoryStream.
			memoryStream.Write(
				contents,
				0,
				numBytesToWrite);

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

			// Write the byte array to the other MemoryStream.
			memoryStream.Write(
				contents,
				0,
				numBytesToWrite);

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

			// Write the byte array to the other MemoryStream.
			memoryStream.Write(
				contents,
				0,
				numBytesToWrite);

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

			// Write the byte array to the other MemoryStream.
			memoryStream.Write(
				contents,
				0,
				numBytesToWrite);

			return true;
		}

		#endregion

		#endregion

		#region IAsyncSerializationStrategy

		#region Read

		public async Task<(bool, TValue)> ReadAsync<TValue>()
		{
			AssertStrategyIsValid(
				typeof(TValue),
				EStreamMode.READ);

			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

			// Read the source file into a byte array.
			byte[] result = new byte[memoryStream.Length];

			int numBytesToRead = (int)memoryStream.Length;

			int numBytesRead = 0;

			while (numBytesToRead > 0)
			{
				// Read may return anything from 0 to numBytesToRead.
				int n = await memoryStream.ReadAsync(
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
			Type valueType)
		{
			AssertStrategyIsValid(
				valueType,
				EStreamMode.READ);

			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

			// Read the source file into a byte array.
			byte[] result = new byte[memoryStream.Length];

			int numBytesToRead = (int)memoryStream.Length;

			int numBytesRead = 0;

			while (numBytesToRead > 0)
			{
				// Read may return anything from 0 to numBytesToRead.
				int n = await memoryStream.ReadAsync(
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
			TValue value)
		{
			AssertStrategyIsValid(
				typeof(TValue),
				EStreamMode.WRITE);

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

			int numBytesToWrite = contents.Length;

			// Write the byte array to the other MemoryStream.
			await memoryStream.WriteAsync(
				contents,
				0,
				numBytesToWrite);

			return true;
		}

		public async Task<bool> WriteAsync(
			Type valueType,
			object value)
		{
			AssertStrategyIsValid(
				valueType,
				EStreamMode.WRITE);

			byte[] contents = value.CastFromTo<object, byte[]>();

			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

			int numBytesToWrite = contents.Length;

			// Write the byte array to the other MemoryStream.
			await memoryStream.WriteAsync(
				contents,
				0,
				numBytesToWrite);

			return true;
		}

		#endregion

		#region Append

		public async Task<bool> AppendAsync<TValue>(
			TValue value)
		{
			AssertStrategyIsValid(
				typeof(TValue),
				EStreamMode.APPEND);

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

			int numBytesToWrite = contents.Length;

			// Write the byte array to the other MemoryStream.
			await memoryStream.WriteAsync(
				contents,
				0,
				numBytesToWrite);

			return true;
		}

		public async Task<bool> AppendAsync(
			Type valueType,
			object value)
		{
			AssertStrategyIsValid(
				valueType,
				EStreamMode.APPEND);

			byte[] contents = value.CastFromTo<object, byte[]>();

			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

			int numBytesToWrite = contents.Length;

			// Write the byte array to the other MemoryStream.
			await memoryStream.WriteAsync(
				contents,
				0,
				numBytesToWrite);

			return true;
		}

		#endregion

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
						return memoryStream;

					default:
						return null;
				}
			}
		}

		public bool StreamOpen { get; private set; }

		#region Flush

		public bool FlushAutomatically { get => false; }

		public void Flush()
		{
			if (!StreamOpen)
				return;

			if (memoryStream != null)
			{
				memoryStream.Flush();
			}
		}

		public async Task FlushAsync()
		{
			if (!StreamOpen)
				return;

			if (memoryStream != null)
			{
				await memoryStream.FlushAsync();
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
					&& memoryStream != null)
				{
					return memoryStream.Position;
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
					&& memoryStream != null)
				{
					return memoryStream.CanSeek;
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
				&& memoryStream != null)
			{
				position = memoryStream.Seek(
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
				&& memoryStream != null)
			{
				position = memoryStream.Seek(
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
				&& memoryStream != null)
			{
				position = memoryStream.Seek(
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

			StreamOpen = OpenStream(
				out memoryStream);

			if (!StreamOpen)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"FAILED TO OPEN STREAM"));

			CurrentMode = EStreamMode.READ;
		}

		public void FinalizeRead()
		{
			if (!StreamOpen)
				return;

			if (CurrentMode != EStreamMode.READ)
				return;

			if (memoryStream != null)
				CloseStream(
					memoryStream);

			memoryStream = default(MemoryStream);

			StreamOpen = false;

			CurrentMode = EStreamMode.NONE;
		}

		public void InitializeWrite()
		{
			if (StreamOpen)
				return;

			StreamOpen = OpenStream(
				out memoryStream);

			if (!StreamOpen)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"FAILED TO OPEN STREAM"));

			CurrentMode = EStreamMode.WRITE;
		}

		public void FinalizeWrite()
		{
			if (!StreamOpen)
				return;

			if (CurrentMode != EStreamMode.WRITE)
				return;

			if (memoryStream != null)
				CloseStream(
					memoryStream);

			memoryStream = default(MemoryStream);

			StreamOpen = false;

			CurrentMode = EStreamMode.NONE;
		}

		public void InitializeAppend()
		{
			if (StreamOpen)
				return;

			StreamOpen = OpenStream(
				out memoryStream);

			if (!StreamOpen)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"FAILED TO OPEN STREAM"));

			CurrentMode = EStreamMode.APPEND;
		}

		public void FinalizeAppend()
		{
			if (!StreamOpen)
				return;

			if (CurrentMode != EStreamMode.APPEND)
				return;

			if (memoryStream != null)
				CloseStream(
					memoryStream);

			memoryStream = default(MemoryStream);

			StreamOpen = false;

			CurrentMode = EStreamMode.NONE;
		}

		public void InitializeReadAndWrite()
		{
			if (StreamOpen)
				return;

			StreamOpen = OpenStream(
				out memoryStream);

			if (!StreamOpen)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"FAILED TO OPEN STREAM"));

			CurrentMode = EStreamMode.READ_AND_WRITE;
		}

		public void FinalizeReadAndWrite()
		{
			if (!StreamOpen)
				return;

			if (CurrentMode != EStreamMode.READ_AND_WRITE)
				return;

			if (memoryStream != null)
				CloseStream(
					memoryStream);

			memoryStream = default(MemoryStream);

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

			int resultLength = memoryStream.Read(
				result,
				0,
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

			int resultLength = memoryStream.Read(
				result,
				0,
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

			memoryStream.Write(
				contents,
				blockOffset,
				blockSize);

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

			memoryStream.Write(
				contents,
				blockOffset,
				blockSize);

			return true;
		}

		#endregion

		#endregion

		#region IAsyncBlockSerializationStrategy

		#region Read

		public async Task<(bool, TValue)> BlockReadAsync<TValue>(
			int blockOffset,
			int blockSize)
		{
			AssertStrategyIsValid(
				typeof(TValue),
				EStreamMode.READ);

			// Read the source file into a byte array.
			byte[] result = new byte[blockSize];

			int resultLength = await memoryStream.ReadAsync(
				result,
				0,
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
			int blockSize)
		{
			AssertStrategyIsValid(
				valueType,
				EStreamMode.READ);

			// Read the source file into a byte array.
			byte[] result = new byte[blockSize];

			int resultLength = await memoryStream.ReadAsync(
				result,
				0,
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
			int blockSize)
		{
			AssertStrategyIsValid(
				typeof(TValue),
				EStreamMode.WRITE);

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			await memoryStream.WriteAsync(
				contents,
				blockOffset,
				blockSize);

			return true;
		}

		public async Task<bool> BlockWriteAsync(
			Type valueType,
			object value,
			int blockOffset,
			int blockSize)
		{
			AssertStrategyIsValid(
				valueType,
				EStreamMode.WRITE);

			byte[] contents = value.CastFromTo<object, byte[]>();

			await memoryStream.WriteAsync(
				contents,
				blockOffset,
				blockSize);

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
						$"STREAM NOT OPEN"));

			if (CurrentMode != preferredMode && CurrentMode != EStreamMode.READ_AND_WRITE)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"INVALID STREAM MODE: {CurrentMode}"));

			if (memoryStream == null)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"FILE STREAM IS NULL"));
		}

		private bool OpenStream(
			out MemoryStream dataStream)
		{
			dataStream = new MemoryStream();

			return true;
		}

		private void CloseStream(MemoryStream dataStream)
		{
			dataStream.Close();
		}
	}
}