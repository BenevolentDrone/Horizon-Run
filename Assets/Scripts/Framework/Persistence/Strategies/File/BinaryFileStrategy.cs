using System;
using System.Threading.Tasks;
using System.IO;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	[SerializationStrategy]
	public class BinaryFileStrategy
		: ISerializationStrategy,
		  IStrategyWithFilter,
		  IHasIODestination,
		  IAsyncSerializationStrategy
	{
		private readonly ILogger logger;

		public string FullPath { get; private set; }

		public BinaryFileStrategy(
			string fullPath,
			ILogger logger)
		{
			FullPath = fullPath;

			this.logger = logger;
		}

		#region ISerializationStrategy

		#region Read

		public bool Read<TValue>(
			out TValue value)
		{
			AssertStrategyIsValid(
				typeof(TValue));

			string savePath = FullPath;

			byte[] result = null;

			if (!IOHelpers.FileExists(
				savePath,
				logger))
			{
				value = result.CastFromTo<byte[], TValue>();

				return false;
			}

			result = File.ReadAllBytes(savePath);

			value = result.CastFromTo<byte[], TValue>();

			return true;
		}

		public bool Read(
			Type valueType,
			out object value)
		{
			AssertStrategyIsValid(
				valueType);

			string savePath = FullPath;

			byte[] result = null;

			if (!IOHelpers.FileExists(
				savePath,
				logger))
			{
				value = result.CastFromTo<byte[], object>();

				return false;
			}

			result = File.ReadAllBytes(savePath);

			value = result.CastFromTo<byte[], object>();

			return true;
		}

		#endregion

		#region Write

		public bool Write<TValue>(
			TValue value)
		{
			AssertStrategyIsValid(
				typeof(TValue));

			string savePath = FullPath;

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			File.WriteAllBytes(savePath, contents);

			return true;
		}

		public bool Write(
			Type valueType,
			object value)
		{
			AssertStrategyIsValid(
				valueType);

			string savePath = FullPath;

			byte[] contents = value.CastFromTo<object, byte[]>();

			File.WriteAllBytes(savePath, contents);

			return true;
		}

		#endregion

		#region Append

		public bool Append<TValue>(
			TValue value)
		{
			//https://learn.microsoft.com/en-us/dotnet/api/system.io.file.appendallbytes?view=net-9.0
#if ENABLE_MONO || ENABLE_IL2CPP || ENABLE_DOTNET || UNITY_EDITOR
			//Does not exist in Unity somehow
			throw new NotSupportedException();
#else
			AssertStrategyIsValid(
				typeof(TValue));

			string savePath = FullPath;

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			File.AppendAllBytes(
				savePath,
				contents);

			return true;
#endif
		}

		public bool Append(
			Type valueType,
			object value)
		{
			//https://learn.microsoft.com/en-us/dotnet/api/system.io.file.appendallbytes?view=net-9.0
#if ENABLE_MONO || ENABLE_IL2CPP || ENABLE_DOTNET || UNITY_EDITOR
			//Does not exist in Unity somehow
			throw new NotSupportedException();
#else
			AssertStrategyIsValid(
				valueType);

			string savePath = FullPath;

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			File.AppendAllBytes(
				savePath,
				contents);

			return true;
#endif
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

		#region IStrategyWithIODestination

		public void EnsureIODestinationExists()
		{
			IOHelpers.EnsureDirectoryExists(
				FullPath,
				logger);
		}

		public bool IODestinationExists()
		{
			return IOHelpers.FileExists(
				FullPath,
				logger);
		}

		public void CreateIODestination()
		{
			string savePath = FullPath;

			IOHelpers.EnsureDirectoryExists(
				FullPath,
				logger);

			if (!IOHelpers.FileExists(
				savePath,
				logger))
			{
				File.Create(
					FullPath);
			}
		}

		public void EraseIODestination()
		{
			string savePath = FullPath;

			if (IOHelpers.FileExists(
				savePath,
				logger))
			{
				File.Delete(savePath);
			}
		}

		#endregion

		#region IAsyncSerializationStrategy

		#region Read

		public async Task<(bool, TValue)> ReadAsync<TValue>(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			AssertStrategyIsValid(
				typeof(TValue));

			string savePath = FullPath;

			TValue value = default;

			if (!IOHelpers.FileExists(
				savePath,
				logger))
			{
				return (false, value);
			}

			byte[] result = await File.ReadAllBytesAsync(
				savePath);

			value = result.CastFromTo<byte[], TValue>();

			return (true, value);
		}

		public async Task<(bool, object)> ReadAsync(
			Type valueType,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			AssertStrategyIsValid(
				valueType);

			string savePath = FullPath;

			object value = default;

			if (!IOHelpers.FileExists(
				savePath,
				logger))
			{
				return (false, value);
			}

			byte[] result = await File.ReadAllBytesAsync(
				savePath);

			value = result.CastFromTo<byte[], object>();

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
				typeof(TValue));

			string savePath = FullPath;

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			await File.WriteAllBytesAsync(
				savePath,
				contents);

			return true;
		}

		public async Task<bool> WriteAsync(
			Type valueType,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			AssertStrategyIsValid(
				valueType);

			string savePath = FullPath;

			byte[] contents = value.CastFromTo<object, byte[]>();

			await File.WriteAllBytesAsync(
				savePath,
				contents);

			return true;
		}

		#endregion

		#region Append

		public async Task<bool> AppendAsync<TValue>(
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			//https://learn.microsoft.com/en-us/dotnet/api/system.io.file.appendallbytes?view=net-9.0
#if ENABLE_MONO || ENABLE_IL2CPP || ENABLE_DOTNET || UNITY_EDITOR
			//Does not exist in Unity somehow
			throw new NotSupportedException();
#else
			AssertStrategyIsValid(
				typeof(TValue));

			string savePath = FullPath;

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			await File.AppendAllBytesAsync(
				savePath,
				contents);

			return true;
#endif
		}

		public async Task<bool> AppendAsync(
			Type valueType,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			//https://learn.microsoft.com/en-us/dotnet/api/system.io.file.appendallbytes?view=net-9.0
#if ENABLE_MONO || ENABLE_IL2CPP || ENABLE_DOTNET || UNITY_EDITOR
			//Does not exist in Unity somehow
			throw new NotSupportedException();
#else
			AssertStrategyIsValid(
				valueType);

			string savePath = FullPath;

			byte[] contents = value.CastFromTo<TValue, byte[]>();

			await File.AppendAllBytesAsync(
				savePath,
				contents);

			return true;
#endif
		}

		#endregion

		#endregion

		private void AssertStrategyIsValid(
			Type valueType)
		{
			if (valueType != typeof(byte[]))
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"INVALID VALUE TYPE: {valueType.Name}"));
		}
	}
}