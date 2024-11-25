using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	[SerializationStrategy]
	public class TextStreamStrategy
		: ISerializationStrategy,
		  IAsyncSerializationStrategy,
		  IStrategyWithIODestination,
		  IStrategyWithStream,
		  IStrategyWithState,
		  IBlockSerializationStrategy,
		  IAsyncBlockSerializationStrategy
	{
		private static readonly Type[] allowedValueTypes = new Type[]
		{
			typeof(char[]),
			typeof(string)
		};

		private readonly bool flushAutomatically;

		private readonly ILogger logger;


		public string FullPath { get; private set; }


		private StreamReader streamReader;

		public StreamReader StreamReader
		{
			get
			{
				return streamReader;
			}
		}

		private StreamWriter streamWriter;
		
		public StreamWriter StreamWriter
		{
			get
			{
				return streamWriter;
			}
		}

		public TextStreamStrategy(
			string fullPath,
			bool flushAutomatically = false,
			ILogger logger = null)
		{
			FullPath = fullPath;

			this.flushAutomatically = flushAutomatically;

			this.logger = logger;


			CurrentMode = ESreamMode.NONE;

			StreamOpen = false;

			streamReader = default(StreamReader);

			streamWriter = default(StreamWriter);
		}

		#region ISerializationStrategy

		public Type[] AllowedValueTypes { get => allowedValueTypes; }

		#region Read

		public bool Read<TValue>(
			out TValue value)
		{
			bool resultIsCharArray = typeof(TValue) == typeof(char[]);

			bool resultIsString = typeof(TValue) == typeof(string);

			AssertStrategyIsValid(
				typeof(TValue),
				resultIsCharArray,
				resultIsString,
				ESreamMode.READ,
				streamReader);

			if (streamReader.EndOfStream)
			{
				value = (resultIsCharArray)
					? Array.Empty<char>().CastFromTo<char[], TValue>()
					: string.Empty.CastFromTo<string, TValue>();

				return false;
			}

			string result = streamReader.ReadToEnd();

			value = (resultIsCharArray)
				? result.ToCharArray().CastFromTo<char[], TValue>()
				: result.CastFromTo<string, TValue>();

			return true;
		}

		public bool Read(
			Type valueType,
			out object value)
		{
			bool resultIsCharArray = valueType == typeof(char[]);

			bool resultIsString = valueType == typeof(string);

			AssertStrategyIsValid(
				valueType,
				resultIsCharArray,
				resultIsString,
				ESreamMode.READ,
				streamReader);

			if (streamReader.EndOfStream)
			{
				value = (resultIsCharArray)
					? Array.Empty<char>()
					: string.Empty;

				return false;
			}

			string result = streamReader.ReadToEnd();

			value = (resultIsCharArray)
				? result.ToCharArray()
				: result;

			return true;
		}

		#endregion

		#region Write

		public bool Write<TValue>(
			TValue value)
		{
			bool resultIsCharArray = typeof(TValue) == typeof(char[]);

			bool resultIsString = typeof(TValue) == typeof(string);

			AssertStrategyIsValid(
				typeof(TValue),
				resultIsCharArray,
				resultIsString,
				ESreamMode.WRITE,
				streamWriter);

			string contents = (resultIsCharArray)
				? value.CastFromTo<TValue, char[]>().ToString()
				: value.CastFromTo<TValue, string>();

			streamWriter.Write(contents);

			return true;
		}

		public bool Write(
			Type valueType,
			object value)
		{
			bool resultIsCharArray = valueType == typeof(char[]);

			bool resultIsString = valueType == typeof(string);

			AssertStrategyIsValid(
				valueType,
				resultIsCharArray,
				resultIsString,
				ESreamMode.WRITE,
				streamWriter);

			string contents = (resultIsCharArray)
				? value.CastFromTo<object, char[]>().ToString()
				: value.CastFromTo<object, string>();

			streamWriter.Write(contents);

			return true;
		}

		#endregion

		#region Append

		public bool Append<TValue>(
			TValue value)
		{
			bool resultIsCharArray = typeof(TValue) == typeof(char[]);

			bool resultIsString = typeof(TValue) == typeof(string);

			AssertStrategyIsValid(
				typeof(TValue),
				resultIsCharArray,
				resultIsString,
				ESreamMode.APPEND,
				streamWriter);

			string contents = (resultIsCharArray)
				? value.CastFromTo<TValue, char[]>().ToString()
				: value.CastFromTo<TValue, string>();

			streamWriter.Write(contents);

			return true;
		}

		public bool Append(
			Type valueType,
			object value)
		{
			bool resultIsCharArray = valueType == typeof(char[]);

			bool resultIsString = valueType == typeof(string);

			AssertStrategyIsValid(
				valueType,
				resultIsCharArray,
				resultIsString,
				ESreamMode.APPEND,
				streamWriter);

			string contents = (resultIsCharArray)
				? value.CastFromTo<object, char[]>().ToString()
				: value.CastFromTo<object, string>();

			streamWriter.Write(contents);

			return true;
		}

		#endregion

		#endregion

		#region IAsyncSerializationStrategy

		#region Read

		public async Task<(bool, TValue)> ReadAsync<TValue>()
		{
			bool resultIsCharArray = typeof(TValue) == typeof(char[]);

			bool resultIsString = typeof(TValue) == typeof(string);

			AssertStrategyIsValid(
				typeof(TValue),
				resultIsCharArray,
				resultIsString,
				ESreamMode.READ,
				streamReader);

			if (streamReader.EndOfStream)
			{
				var nullValue = (resultIsCharArray)
					? Array.Empty<char>().CastFromTo<char[], TValue>()
					: string.Empty.CastFromTo<string, TValue>();

				return (false, nullValue);
			}

			string result = await streamReader.ReadToEndAsync();

			TValue value = (resultIsCharArray)
				? result.ToCharArray().CastFromTo<char[], TValue>()
				: result.CastFromTo<string, TValue>();

			return (true, value);
		}

		public async Task<(bool, object)> ReadAsync(
			Type valueType)
		{
			bool resultIsCharArray = valueType == typeof(char[]);

			bool resultIsString = valueType == typeof(string);

			AssertStrategyIsValid(
				valueType,
				resultIsCharArray,
				resultIsString,
				ESreamMode.READ,
				streamReader);

			if (streamReader.EndOfStream)
			{
				object nullValue = (resultIsCharArray)
					? Array.Empty<char>()
					: string.Empty;

				return (false, nullValue);
			}

			string result = await streamReader.ReadToEndAsync();

			object value = (resultIsCharArray)
				? result.ToCharArray()
				: result;

			return (true, value);
		}

		#endregion

		#region Write

		public async Task<bool> WriteAsync<TValue>(
			TValue value)
		{
			bool resultIsCharArray = typeof(TValue) == typeof(char[]);

			bool resultIsString = typeof(TValue) == typeof(string);

			AssertStrategyIsValid(
				typeof(TValue),
				resultIsCharArray,
				resultIsString,
				ESreamMode.WRITE,
				streamWriter);

			string contents = (resultIsCharArray)
				? value.CastFromTo<TValue, char[]>().ToString()
				: value.CastFromTo<TValue, string>();

			await streamWriter.WriteAsync(contents);

			return true;
		}

		public async Task<bool> WriteAsync(
			Type valueType,
			object value)
		{
			bool resultIsCharArray = valueType == typeof(char[]);

			bool resultIsString = valueType == typeof(string);

			AssertStrategyIsValid(
				valueType,
				resultIsCharArray,
				resultIsString,
				ESreamMode.WRITE,
				streamWriter);

			string contents = (resultIsCharArray)
				? value.CastFromTo<object, char[]>().ToString()
				: value.CastFromTo<object, string>();

			await streamWriter.WriteAsync(contents);

			return true;
		}

		#endregion

		#region Append

		public async Task<bool> AppendAsync<TValue>(
			TValue value)
		{
			bool resultIsCharArray = typeof(TValue) == typeof(char[]);

			bool resultIsString = typeof(TValue) == typeof(string);

			AssertStrategyIsValid(
				typeof(TValue),
				resultIsCharArray,
				resultIsString,
				ESreamMode.APPEND,
				streamWriter);

			string contents = (resultIsCharArray)
				? value.CastFromTo<TValue, char[]>().ToString()
				: value.CastFromTo<TValue, string>();

			await streamWriter.WriteAsync(contents);

			return true;
		}

		public async Task<bool> AppendAsync(
			Type valueType,
			object value)
		{
			bool resultIsCharArray = valueType == typeof(char[]);

			bool resultIsString = valueType == typeof(string);

			AssertStrategyIsValid(
				valueType,
				resultIsCharArray,
				resultIsString,
				ESreamMode.APPEND,
				streamWriter);

			string contents = (resultIsCharArray)
				? value.CastFromTo<object, char[]>().ToString()
				: value.CastFromTo<object, string>();

			await streamWriter.WriteAsync(contents);

			return true;
		}

		#endregion

		#endregion

		#region IStrategyWithIODestination

		public void EnsureIOTargetDestinationExists()
		{
			IOHelpers.EnsureDirectoryExists(
				FullPath,
				logger,
				GetType());
		}

		public bool IOTargetExists()
		{
			return IOHelpers.FileExists(
				FullPath,
				logger,
				GetType());
		}

		public void CreateIOTarget()
		{
			string savePath = FullPath;

			IOHelpers.EnsureDirectoryExists(
				FullPath,
				logger,
				GetType());

			if (!IOHelpers.FileExists(
				savePath,
				logger,
				GetType()))
			{
				File.Create(
					FullPath);
			}
		}

		public void EraseIOTarget()
		{
			string savePath = FullPath;

			if (IOHelpers.FileExists(
				savePath,
				logger,
				GetType()))
			{
				File.Delete(savePath);
			}
		}

		#endregion

		#region IStrategyWithStream

		public ESreamMode CurrentMode { get; private set; }

		public Stream Stream
		{
			get
			{
				switch (CurrentMode)
				{
					case ESreamMode.READ:
						if (streamReader == null)
							return null;

						return streamReader.BaseStream;

					case ESreamMode.WRITE:
					case ESreamMode.APPEND:
						if (streamWriter == null)
							return null;

						return streamWriter.BaseStream;

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

			if (streamWriter != null)
			{
				streamWriter.Flush();
			}
		}

		public async Task FlushAsync()
		{
			if (!StreamOpen)
				return;

			if (streamWriter != null)
			{
				await streamWriter.FlushAsync();
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

				if (CurrentMode == ESreamMode.READ
					&& streamReader != null)
				{
					return streamReader.BaseStream.Position;
				}

				if ((CurrentMode == ESreamMode.WRITE || CurrentMode == ESreamMode.APPEND)
					&& streamWriter != null)
				{
					return streamWriter.BaseStream.Position;
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

				if (CurrentMode == ESreamMode.READ
					&& streamReader != null)
				{
					return streamReader.BaseStream.CanSeek;
				}
				
				if ((CurrentMode == ESreamMode.WRITE || CurrentMode == ESreamMode.APPEND)
					&& streamWriter != null)
				{
					return streamWriter.BaseStream.CanSeek;
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

			if (CurrentMode == ESreamMode.READ
				&& streamReader != null)
			{
				position = streamReader.BaseStream.Seek(
					offset,
					SeekOrigin.Current);

				return true;
			}

			if ((CurrentMode == ESreamMode.WRITE || CurrentMode == ESreamMode.APPEND)
				&& streamWriter != null)
			{
				position = streamWriter.BaseStream.Seek(
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

			if (CurrentMode == ESreamMode.READ
				&& streamReader != null)
			{
				position = streamReader.BaseStream.Seek(
					offset,
					SeekOrigin.Begin);

				return true;
			}

			if ((CurrentMode == ESreamMode.WRITE || CurrentMode == ESreamMode.APPEND)
				&& streamWriter != null)
			{
				position = streamWriter.BaseStream.Seek(
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

			if (CurrentMode == ESreamMode.READ
				&& streamReader != null)
			{
				position = streamReader.BaseStream.Seek(
					offset,
					SeekOrigin.End);

				return true;
			}

			if ((CurrentMode == ESreamMode.WRITE || CurrentMode == ESreamMode.APPEND)
				&& streamWriter != null)
			{
				position = streamWriter.BaseStream.Seek(
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

		public void InitializeRead()
		{
			if (StreamOpen)
				return;

			StreamOpen = OpenReadStream(
				out streamReader);

			if (!StreamOpen)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"FAILED TO OPEN STREAM: {FullPath}"));

			CurrentMode = ESreamMode.READ;
		}

		public void FinalizeRead()
		{
			if (!StreamOpen)
				return;

			if (CurrentMode != ESreamMode.READ)
				return;

			if (streamReader != null)
				CloseStream(
					streamReader);

			streamReader = default(StreamReader);

			StreamOpen = false;

			CurrentMode = ESreamMode.NONE;
		}

		public void InitializeWrite()
		{
			if (StreamOpen)
				return;

			StreamOpen = OpenWriteStream(
				out streamWriter);

			if (!StreamOpen)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"FAILED TO OPEN STREAM: {FullPath}"));

			CurrentMode = ESreamMode.WRITE;
		}

		public void FinalizeWrite()
		{
			if (!StreamOpen)
				return;

			if (CurrentMode != ESreamMode.WRITE)
				return;

			if (streamWriter != null)
				CloseStream(
					streamWriter);

			streamWriter = default(StreamWriter);

			StreamOpen = false;

			CurrentMode = ESreamMode.NONE;
		}

		public void InitializeAppend()
		{
			if (StreamOpen)
				return;

			StreamOpen = OpenAppendStream(
				out streamWriter);

			if (!StreamOpen)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"FAILED TO OPEN STREAM: {FullPath}"));

			CurrentMode = ESreamMode.APPEND;
		}

		public void FinalizeAppend()
		{
			if (!StreamOpen)
				return;

			if (CurrentMode != ESreamMode.APPEND)
				return;

			if (streamWriter != null)
				CloseStream(
					streamWriter);

			streamWriter = default(StreamWriter);

			StreamOpen = false;

			CurrentMode = ESreamMode.NONE;
		}

		#endregion

		#region IBlockSerializationStrategy

		#region Read

		public bool BlockRead<TValue>(
			int blockOffset,
			int blockSize,
			out TValue value)
		{
			bool resultIsCharArray = typeof(TValue) == typeof(char[]);

			bool resultIsString = typeof(TValue) == typeof(string);

			AssertStrategyIsValid(
				typeof(TValue),
				resultIsCharArray,
				resultIsString,
				ESreamMode.READ,
				streamReader);

			if (streamReader.EndOfStream)
			{
				value = (resultIsCharArray)
					? Array.Empty<char>().CastFromTo<char[], TValue>()
					: string.Empty.CastFromTo<string, TValue>();

				return false;
			}

			char[] resultArray = new char[blockSize];

			int resultLength = streamReader.Read(
				resultArray,
				blockOffset,
				blockSize);

			if (resultLength != blockSize)
			{
				char[] resultArrayTrimmed = new char[resultLength];

				Array.Copy(
					resultArray,
					resultArrayTrimmed,
					resultLength);

				resultArray = resultArrayTrimmed;
			}

			value = (resultIsCharArray)
				? resultArray.CastFromTo<char[], TValue>()
				: new string(resultArray).CastFromTo<string, TValue>();

			return true;
		}

		public bool BlockRead(
			Type valueType,
			int blockOffset,
			int blockSize,
			out object value)
		{
			bool resultIsCharArray = valueType == typeof(char[]);

			bool resultIsString = valueType == typeof(string);

			AssertStrategyIsValid(
				valueType,
				resultIsCharArray,
				resultIsString,
				ESreamMode.READ,
				streamReader);

			if (streamReader.EndOfStream)
			{
				value = (resultIsCharArray)
					? Array.Empty<char>()
					: string.Empty;

				return false;
			}

			char[] resultArray = new char[blockSize];

			int resultLength = streamReader.Read(
				resultArray,
				blockOffset,
				blockSize);

			if (resultLength != blockSize)
			{
				char[] resultArrayTrimmed = new char[resultLength];

				Array.Copy(
					resultArray,
					resultArrayTrimmed,
					resultLength);

				resultArray = resultArrayTrimmed;
			}

			value = (resultIsCharArray)
				? resultArray
				: new string(resultArray);

			return true;
		}

		#endregion

		#region Write

		public bool BlockWrite<TValue>(
			TValue value,
			int blockOffset,
			int blockSize)
		{
			bool resultIsCharArray = typeof(TValue) == typeof(char[]);

			bool resultIsString = typeof(TValue) == typeof(string);

			AssertStrategyIsValid(
				typeof(TValue),
				resultIsCharArray,
				resultIsString,
				ESreamMode.WRITE,
				streamWriter);

			char[] contentsArray = (resultIsCharArray)
				? value.CastFromTo<TValue, char[]>()
				: value.CastFromTo<TValue, string>().ToCharArray();

			streamWriter.Write(
				contentsArray,
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
			bool resultIsCharArray = valueType == typeof(char[]);

			bool resultIsString = valueType == typeof(string);

			AssertStrategyIsValid(
				valueType,
				resultIsCharArray,
				resultIsString,
				ESreamMode.WRITE,
				streamWriter);

			char[] contentsArray = (resultIsCharArray)
				? value.CastFromTo<object, char[]>()
				: value.CastFromTo<object, string>().ToCharArray();

			streamWriter.Write(
				contentsArray,
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
			bool resultIsCharArray = typeof(TValue) == typeof(char[]);

			bool resultIsString = typeof(TValue) == typeof(string);

			AssertStrategyIsValid(
				typeof(TValue),
				resultIsCharArray,
				resultIsString,
				ESreamMode.READ,
				streamReader);

			if (streamReader.EndOfStream)
			{
				TValue nullValue = (resultIsCharArray)
					? Array.Empty<char>().CastFromTo<char[], TValue>()
					: string.Empty.CastFromTo<string, TValue>();

				return (false, nullValue);
			}

			char[] resultArray = new char[blockSize];

			int resultLength = await streamReader.ReadAsync(
				resultArray,
				blockOffset,
				blockSize);

			if (resultLength != blockSize)
			{
				char[] resultArrayTrimmed = new char[resultLength];

				Array.Copy(
					resultArray,
					resultArrayTrimmed,
					resultLength);

				resultArray = resultArrayTrimmed;
			}

			TValue value = (resultIsCharArray)
				? resultArray.CastFromTo<char[], TValue>()
				: new string(resultArray).CastFromTo<string, TValue>();

			return (true, value);
		}

		public async Task<(bool, object)> BlockReadAsync(
			Type valueType,
			int blockOffset,
			int blockSize)
		{
			bool resultIsCharArray = valueType == typeof(char[]);

			bool resultIsString = valueType == typeof(string);

			AssertStrategyIsValid(
				valueType,
				resultIsCharArray,
				resultIsString,
				ESreamMode.READ,
				streamReader);

			if (streamReader.EndOfStream)
			{
				object nullValue = (resultIsCharArray)
					? Array.Empty<char>()
					: string.Empty;

				return (false, nullValue);
			}

			char[] resultArray = new char[blockSize];

			int resultLength = await streamReader.ReadAsync(
				resultArray,
				blockOffset,
				blockSize);

			if (resultLength != blockSize)
			{
				char[] resultArrayTrimmed = new char[resultLength];

				Array.Copy(
					resultArray,
					resultArrayTrimmed,
					resultLength);

				resultArray = resultArrayTrimmed;
			}

			object value = (resultIsCharArray)
				? resultArray
				: new string(resultArray);

			return (true, value);
		}

		#endregion

		#region Write

		public async Task<bool> BlockWriteAsync<TValue>(
			TValue value,
			int blockOffset,
			int blockSize)
		{
			bool resultIsCharArray = typeof(TValue) == typeof(char[]);

			bool resultIsString = typeof(TValue) == typeof(string);

			AssertStrategyIsValid(
				typeof(TValue),
				resultIsCharArray,
				resultIsString,
				ESreamMode.WRITE,
				streamWriter);

			char[] contentsArray = (resultIsCharArray)
				? value.CastFromTo<TValue, char[]>()
				: value.CastFromTo<TValue, string>().ToCharArray();

			await streamWriter.WriteAsync(
				contentsArray,
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
			bool resultIsCharArray = valueType == typeof(char[]);

			bool resultIsString = valueType == typeof(string);

			AssertStrategyIsValid(
				valueType,
				resultIsCharArray,
				resultIsString,
				ESreamMode.WRITE,
				streamWriter);

			char[] contentsArray = (resultIsCharArray)
				? value.CastFromTo<object, char[]>()
				: value.CastFromTo<object, string>().ToCharArray();

			await streamWriter.WriteAsync(
				contentsArray,
				blockOffset,
				blockSize);

			return true;
		}

		#endregion

		#endregion

		private void AssertStrategyIsValid(
			Type valueType,
			bool resultIsCharArray,
			bool resultIsString,
			ESreamMode preferredMode,
			object readerWriter)
		{
			if (!resultIsCharArray && !resultIsString)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"INVALID VALUE TYPE: {valueType.Name}"));

			if (!StreamOpen)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"STREAM NOT OPEN: {FullPath}"));

			if (CurrentMode != preferredMode)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"INVALID STREAM MODE. EXPECTED: {preferredMode} CURRENT: {CurrentMode}"));

			if (readerWriter == null)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"STREAM {((preferredMode == ESreamMode.READ) ? "READER" : "WRITER")} IS NULL: {FullPath}"));
		}

		private bool OpenReadStream(
			out StreamReader streamReader)
		{
			streamReader = default(StreamReader);

			if (!IOHelpers.FileExists(
				FullPath,
				logger,
				GetType()))
			{
				return false;
			}

			streamReader = new StreamReader(
				FullPath,
				Encoding.UTF8);

			return true;
		}

		private bool OpenWriteStream(
			out StreamWriter streamWriter)
		{
			IOHelpers.EnsureDirectoryExists(
				FullPath,
				logger,
				GetType());

			//Courtesy of https://stackoverflow.com/questions/7306214/append-lines-to-a-file-using-a-streamwriter
			streamWriter = new StreamWriter(
				FullPath,
				append: false,
				Encoding.UTF8);

			streamWriter.AutoFlush = flushAutomatically;

			return true;
		}

		private bool OpenAppendStream(
			out StreamWriter streamWriter)
		{
			IOHelpers.EnsureDirectoryExists(
				FullPath,
				logger,
				GetType());

			//Courtesy of https://stackoverflow.com/questions/7306214/append-lines-to-a-file-using-a-streamwriter
			streamWriter = new StreamWriter(
				FullPath,
				append: true,
				Encoding.UTF8);

			streamWriter.AutoFlush = flushAutomatically;

			return true;
		}

		private void CloseStream(StreamReader streamReader)
		{
			streamReader.Close();
		}

		private void CloseStream(StreamWriter streamWriter)
		{
			streamWriter.Close();
		}
	}
}