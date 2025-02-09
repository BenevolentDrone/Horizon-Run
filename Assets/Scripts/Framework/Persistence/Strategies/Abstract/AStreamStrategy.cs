using System;
using System.Threading.Tasks;
using System.IO;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public abstract class AStreamStrategy<TStream>
		: AStrategy,
		  IStrategyWithFilter,
		  IStrategyWithStream
		where TStream : Stream
	{
		protected TStream stream;

		public AStreamStrategy(
			ILogger logger)
			: base(logger)
		{
			CurrentMode = EStreamMode.NONE;

			StreamOpen = false;

			stream = default(TStream);
		}

		#region ISerializationStrategy

		#region Read

		public override bool Read<TValue>(
			out TValue value)
		{
			AssertStrategyIsValid(
				typeof(TValue),
				EStreamMode.READ);

			// Read the source file into a byte array.
			byte[] result = new byte[stream.Length];

			ReadBuffer(
				result);

			value = result.CastFromTo<byte[], TValue>();

			return true;
		}

		public override bool Read(
			Type valueType,
			out object value)
		{
			AssertStrategyIsValid(
				valueType,
				EStreamMode.READ);

			// Read the source file into a byte array.
			byte[] result = new byte[stream.Length];

			ReadBuffer(
				result);

			value = result;

			return true;
		}

		#endregion

		#region Write

		public override bool Write<TValue>(
			TValue value)
		{
			AssertStrategyIsValid(
				typeof(TValue),
				EStreamMode.WRITE);

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			WriteBuffer(
				contents);

			return true;
		}

		public override bool Write(
			Type valueType,
			object value)
		{
			AssertStrategyIsValid(
				valueType,
				EStreamMode.WRITE);

			byte[] contents = value.CastFromTo<object, byte[]>();

			WriteBuffer(
				contents);

			return true;
		}

		#endregion

		#region Append

		public override bool Append<TValue>(
			TValue value)
		{
			AssertStrategyIsValid(
				typeof(TValue),
				EStreamMode.APPEND);

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			WriteBuffer(
				contents);

			return true;
		}

		public override bool Append(
			Type valueType,
			object value)
		{
			AssertStrategyIsValid(
				valueType,
				EStreamMode.APPEND);

			byte[] contents = value.CastFromTo<object, byte[]>();

			WriteBuffer(
				contents);

			return true;
		}

		#endregion

		#endregion

		#region IStrategyWithFilter

		public virtual bool AllowsType<TValue>()
		{
			return typeof(TValue) == typeof(byte[]);
		}

		public virtual bool AllowsType(
			Type valueType)
		{
			return valueType == typeof(byte[]);
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
						return stream;

					default:
						return null;
				}
			}
		}

		public bool StreamOpen { get; private set; }

		#region Flush

		public virtual bool FlushAutomatically { get => false; }

		public void Flush()
		{
			if (!StreamOpen)
				return;

			if (stream != null)
			{
				stream.Flush();
			}
		}

		public async Task FlushAsync(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (!StreamOpen)
				return;

			if (stream != null)
			{
				await stream.FlushAsync();
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

				if ((CurrentMode == EStreamMode.READ
					|| CurrentMode == EStreamMode.WRITE
					|| CurrentMode == EStreamMode.APPEND
					|| CurrentMode == EStreamMode.READ_AND_WRITE)
					&& stream != null)
				{
					return stream.Position;
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

				if ((CurrentMode == EStreamMode.READ
					|| CurrentMode == EStreamMode.WRITE
					|| CurrentMode == EStreamMode.APPEND
					|| CurrentMode == EStreamMode.READ_AND_WRITE)
					&& stream != null)
				{
					return stream.CanSeek;
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

			if ((CurrentMode == EStreamMode.READ
				|| CurrentMode == EStreamMode.WRITE
				|| CurrentMode == EStreamMode.APPEND
				|| CurrentMode == EStreamMode.READ_AND_WRITE)
				&& stream != null)
			{
				position = stream.Seek(
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

			if ((CurrentMode == EStreamMode.READ
				|| CurrentMode == EStreamMode.WRITE
				|| CurrentMode == EStreamMode.APPEND
				|| CurrentMode == EStreamMode.READ_AND_WRITE)
				&& stream != null)
			{
				position = stream.Seek(
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

			if ((CurrentMode == EStreamMode.READ
				|| CurrentMode == EStreamMode.WRITE
				|| CurrentMode == EStreamMode.APPEND
				|| CurrentMode == EStreamMode.READ_AND_WRITE)
				&& stream != null)
			{
				position = stream.Seek(
					offset,
					SeekOrigin.End);

				return true;
			}

			position = -1;

			return false;
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

			// Read the source file into a byte array.
			byte[] result = new byte[stream.Length];

			await ReadBufferAsync(
				result,
				asyncContext);

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

			// Read the source file into a byte array.
			byte[] result = new byte[stream.Length];

			await ReadBufferAsync(
				result,
				asyncContext);

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

			await WriteBufferAsync(
				contents,
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

			await WriteBufferAsync(
				contents,
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

			await WriteBufferAsync(
				contents,
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

			await WriteBufferAsync(
				contents,
				asyncContext);

			return true;
		}

		#endregion

		#endregion

		#region IBlockSerializationStrategy

		#region Read

		public bool ReadBlock<TValue>(
			int blockOffset,
			int blockSize,
			out TValue value)
		{
			AssertStrategyIsValid(
				typeof(TValue),
				EStreamMode.READ);

			// Read the source file into a byte array.
			byte[] result = new byte[blockSize];

			ReadBufferBlock(
				ref result,
				blockOffset,
				blockSize);

			value = result.CastFromTo<byte[], TValue>();

			return true;
		}

		public bool ReadBlock(
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

			ReadBufferBlock(
				ref result,
				blockOffset,
				blockSize);

			value = result;

			return true;
		}

		#endregion

		#region Write

		public bool WriteBlock<TValue>(
			TValue value,
			int blockOffset,
			int blockSize)
		{
			AssertStrategyIsValid(
				typeof(TValue),
				EStreamMode.WRITE);

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			if (blockOffset < 0)
				blockOffset = 0;

			if (blockSize < 0)
				blockSize = contents.Length;

			stream.Write(
				contents,
				blockOffset,
				blockSize);

			if (FlushAutomatically)
				Flush();

			return true;
		}

		public bool WriteBlock(
			Type valueType,
			object value,
			int blockOffset,
			int blockSize)
		{
			AssertStrategyIsValid(
				valueType,
				EStreamMode.WRITE);

			byte[] contents = value.CastFromTo<object, byte[]>();

			if (blockOffset < 0)
				blockOffset = 0;

			if (blockSize < 0)
				blockSize = contents.Length;

			stream.Write(
				contents,
				blockOffset,
				blockSize);

			if (FlushAutomatically)
				Flush();

			return true;
		}

		#endregion

		#endregion

		#region IAsyncBlockSerializationStrategy

		#region Read

		public async Task<(bool, TValue)> ReadBlockAsync<TValue>(
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

			result = await ReadBufferBlockAsync(
				result,
				blockOffset,
				blockSize,

				asyncContext);

			TValue value = result.CastFromTo<byte[], TValue>();

			return (true, value);
		}

		public async Task<(bool, object)> ReadBlockAsync(
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

			result = await ReadBufferBlockAsync(
				result,
				blockOffset,
				blockSize,

				asyncContext);

			object value = result;

			return (true, value);
		}

		#endregion

		#region Write

		public async Task<bool> WriteBlockAsync<TValue>(
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

			if (blockOffset < 0)
				blockOffset = 0;

			if (blockSize < 0)
				blockSize = contents.Length;

			await stream.WriteAsync(
				contents,
				blockOffset,
				blockSize);

			if (FlushAutomatically)
				await FlushAsync(
					asyncContext);

			return true;
		}

		public async Task<bool> WriteBlockAsync(
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

			if (blockOffset < 0)
				blockOffset = 0;

			if (blockSize < 0)
				blockSize = contents.Length;

			await stream.WriteAsync(
				contents,
				blockOffset,
				blockSize);

			if (FlushAutomatically)
				await FlushAsync(
					asyncContext);

			return true;
		}

		#endregion

		#endregion

		#region IHasReadWriteControl

		public virtual bool SupportsSimultaneousReadAndWrite { get => true; }

		public void InitializeRead()
		{
			if (StreamOpen)
				return;

			if (CurrentMode != EStreamMode.NONE)
				return;

			StreamOpen = OpenReadStream(
				out stream);

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

			if (stream != null)
				CloseStream(
					stream);

			stream = default(TStream);

			StreamOpen = false;

			CurrentMode = EStreamMode.NONE;
		}

		public void InitializeWrite()
		{
			if (StreamOpen)
				return;

			if (CurrentMode != EStreamMode.NONE)
				return;

			StreamOpen = OpenWriteStream(
				out stream);

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

			if (stream != null)
				CloseStream(
					stream);

			stream = default(TStream);

			StreamOpen = false;

			CurrentMode = EStreamMode.NONE;
		}

		public void InitializeAppend()
		{
			if (StreamOpen)
				return;

			if (CurrentMode != EStreamMode.NONE)
				return;

			StreamOpen = OpenAppendStream(
				out stream);

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

			if (stream != null)
				CloseStream(
					stream);

			stream = default(TStream);

			StreamOpen = false;

			CurrentMode = EStreamMode.NONE;
		}

		public void InitializeReadAndWrite()
		{
			if (!SupportsSimultaneousReadAndWrite)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"STREAM DOES NOT SUPPORT SIMULTANEOUS READ AND WRITE"));

			if (StreamOpen)
				return;

			if (CurrentMode != EStreamMode.NONE)
				return;

			StreamOpen = OpenReadWriteStream(
				out stream);

			if (!StreamOpen)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"FAILED TO OPEN STREAM"));

			CurrentMode = EStreamMode.READ_AND_WRITE;
		}

		public void FinalizeReadAndWrite()
		{
			if (!SupportsSimultaneousReadAndWrite)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"STREAM DOES NOT SUPPORT SIMULTANEOUS READ AND WRITE"));

			if (!StreamOpen)
				return;

			if (CurrentMode != EStreamMode.READ_AND_WRITE)
				return;

			if (stream != null)
				CloseStream(
					stream);

			stream = default(TStream);

			StreamOpen = false;

			CurrentMode = EStreamMode.NONE;
		}

		#endregion

		protected virtual void AssertStrategyIsValid(
			Type valueType,
			EStreamMode preferredMode)
		{
			AssertValueType(
				valueType,
				typeof(byte[]));

			AssertStream(
				preferredMode);
		}

		protected void AssertStream(
			EStreamMode preferredMode)
		{
			if (!StreamOpen)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"STREAM NOT OPEN"));

			if (CurrentMode != preferredMode
				&& CurrentMode != EStreamMode.READ_AND_WRITE)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"INVALID STREAM MODE: {CurrentMode}"));

			if (stream == null)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"INVALID STREAM"));
		}

		protected void ReadBuffer(
			byte[] buffer)
		{
			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

			int numBytesToRead = (int)stream.Length;

			int numBytesRead = 0;

			while (numBytesToRead > 0)
			{
				// Read may return anything from 0 to numBytesToRead.
				int n = stream.Read(
					buffer,
					numBytesRead,
					numBytesToRead);

				// Break when the end of the file is reached.
				if (n == 0)
					break;

				numBytesRead += n;

				numBytesToRead -= n;
			}
		}

		protected async Task ReadBufferAsync(
			byte[] buffer,
			
			//Async tail
			AsyncExecutionContext asyncContext)
		{
			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

			int numBytesToRead = (int)stream.Length;

			int numBytesRead = 0;

			while (numBytesToRead > 0)
			{
				// Read may return anything from 0 to numBytesToRead.
				int n = await stream.ReadAsync(
					buffer,
					numBytesRead,
					numBytesToRead);

				// Break when the end of the file is reached.
				if (n == 0)
					break;

				numBytesRead += n;

				numBytesToRead -= n;
			}
		}

		protected void ReadBufferBlock(
			ref byte[] result,
			int blockOffset,
			int blockSize)
		{
			int resultLength = stream.Read(
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
		}

		protected async Task<byte[]> ReadBufferBlockAsync(
			byte[] buffer,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		
		{
			int resultLength = await stream.ReadAsync(
				buffer,
				blockOffset,
				blockSize);

			if (resultLength != blockSize)
			{
				byte[] resultTrimmed = new byte[resultLength];

				Array.Copy(
					buffer,
					resultTrimmed,
					resultLength);

				buffer = resultTrimmed;
			}

			return buffer;
		}

		protected void WriteBuffer(
			byte[] buffer)
		{
			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

			int numBytesToWrite = buffer.Length;

			// Write the byte array to the other MemoryStream.
			stream.Write(
				buffer,
				0,
				numBytesToWrite);

			if (FlushAutomatically)
				Flush();
		}

		protected async Task WriteBufferAsync(
			byte[] buffer,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			//Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filestream.read?view=net-8.0

			int numBytesToWrite = buffer.Length;

			// Write the byte array to the other MemoryStream.
			await stream.WriteAsync(
				buffer,
				0,
				numBytesToWrite);

			if (FlushAutomatically)
				await FlushAsync(
					asyncContext);
		}

		protected abstract bool OpenReadStream(
			out TStream dataStream);

		protected abstract bool OpenWriteStream(
			out TStream dataStream);

		protected abstract bool OpenAppendStream(
			out TStream dataStream);

		protected virtual bool OpenReadWriteStream(
			out TStream dataStream)
		{
			throw new NotImplementedException();
		}

		protected virtual void CloseStream(
			TStream stream)
		{
			stream.Close();
		}
	}
}