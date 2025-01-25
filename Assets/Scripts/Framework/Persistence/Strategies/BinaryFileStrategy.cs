using System;
using System.IO;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	[SerializationStrategy]
	public class BinaryFileStrategy
		: ISerializationStrategy,
		  IStrategyWithIODestination,
		  IStrategyWithFilter
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
			//Does not exist in Unity somehow
			throw new NotSupportedException();
		}

		public bool Append(
			Type valueType,
			object value)
		{
			//https://learn.microsoft.com/en-us/dotnet/api/system.io.file.appendallbytes?view=net-9.0
			//Does not exist in Unity somehow
			throw new NotSupportedException();
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