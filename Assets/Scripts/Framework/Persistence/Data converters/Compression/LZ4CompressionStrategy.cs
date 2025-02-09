using System;
using System.IO;
using System.Text;

using K4os.Compression.LZ4;

namespace HereticalSolutions.Persistence
{
	/// <summary>
	/// A wrapper strategy that compresses data using LZ4 compression algorithm
	/// </summary>
	public class LZ4CompressionStrategy
		: AWrapperStrategy,
		  IStrategyWithFilter
	{
		#region Fields
		
		private readonly LZ4Level compressionLevel;
		
		#endregion
		
		#region Constructor
		
		public LZ4CompressionStrategy(
			ISerializationStrategy innerStrategy,
			LZ4Level compressionLevel = LZ4Level.L00_FAST)
			: base(innerStrategy)
		{
			this.compressionLevel = compressionLevel;
		}
		
		#endregion

		#region ISerializationStrategy

		#region Read

		public override bool Read<TValue>(
			out TValue value)
		{
			AssertStrategyIsValid(typeof(TValue));

			if (!innerStrategy.Read<byte[]>(out var compressedBytes))
			{
				value = default;
				return false;
			}

			try
			{
				var decompressedBytes = new byte[LZ4Codec.MaximumOutputSize(compressedBytes.Length)];
				
				var decompressedSize = LZ4Codec.Decode(
					compressedBytes,
					0,
					compressedBytes.Length,
					decompressedBytes,
					0,
					decompressedBytes.Length);

				if (decompressedSize < 0)
				{
					value = default;
					return false;
				}

				Array.Resize(ref decompressedBytes, decompressedSize);

				if (typeof(TValue) == typeof(byte[]))
				{
					value = (TValue)(object)decompressedBytes;
					return true;
				}

				var json = Encoding.UTF8.GetString(decompressedBytes);
				value = System.Text.Json.JsonSerializer.Deserialize<TValue>(json);
				return true;
			}
			catch (Exception)
			{
				value = default;
				return false;
			}
		}

		public override bool Read(
			Type valueType,
			out object value)
		{
			AssertStrategyIsValid(valueType);

			if (!innerStrategy.Read<byte[]>(out var compressedBytes))
			{
				value = default;
				return false;
			}

			try
			{
				var decompressedBytes = new byte[LZ4Codec.MaximumOutputSize(compressedBytes.Length)];
				
				var decompressedSize = LZ4Codec.Decode(
					compressedBytes,
					0,
					compressedBytes.Length,
					decompressedBytes,
					0,
					decompressedBytes.Length);

				if (decompressedSize < 0)
				{
					value = default;
					return false;
				}

				Array.Resize(ref decompressedBytes, decompressedSize);

				if (valueType == typeof(byte[]))
				{
					value = decompressedBytes;
					return true;
				}

				var json = Encoding.UTF8.GetString(decompressedBytes);
				value = System.Text.Json.JsonSerializer.Deserialize(json, valueType);
				return true;
			}
			catch (Exception)
			{
				value = default;
				return false;
			}
		}

		#endregion

		#region Write

		public override bool Write<TValue>(
			TValue value)
		{
			AssertStrategyIsValid(typeof(TValue));

			try
			{
				byte[] sourceBytes;
				
				if (typeof(TValue) == typeof(byte[]))
				{
					sourceBytes = value as byte[];
				}
				else
				{
					var json = System.Text.Json.JsonSerializer.Serialize(value);
					sourceBytes = Encoding.UTF8.GetBytes(json);
				}

				var compressedBytes = new byte[LZ4Codec.MaximumOutputSize(sourceBytes.Length)];
				
				var compressedSize = LZ4Codec.Encode(
					sourceBytes,
					0,
					sourceBytes.Length,
					compressedBytes,
					0,
					compressedBytes.Length,
					compressionLevel);

				if (compressedSize < 0)
					return false;

				Array.Resize(ref compressedBytes, compressedSize);

				return innerStrategy.Write(compressedBytes);
			}
			catch (Exception)
			{
				return false;
			}
		}

		public override bool Write(
			Type valueType,
			object value)
		{
			AssertStrategyIsValid(valueType);

			try
			{
				byte[] sourceBytes;
				
				if (valueType == typeof(byte[]))
				{
					sourceBytes = value as byte[];
				}
				else
				{
					var json = System.Text.Json.JsonSerializer.Serialize(value, valueType);
					sourceBytes = Encoding.UTF8.GetBytes(json);
				}

				var compressedBytes = new byte[LZ4Codec.MaximumOutputSize(sourceBytes.Length)];
				
				var compressedSize = LZ4Codec.Encode(
					sourceBytes,
					0,
					sourceBytes.Length,
					compressedBytes,
					0,
					compressedBytes.Length,
					compressionLevel);

				if (compressedSize < 0)
					return false;

				Array.Resize(ref compressedBytes, compressedSize);

				return innerStrategy.Write(compressedBytes);
			}
			catch (Exception)
			{
				return false;
			}
		}

		#endregion

		#region Append

		public bool Append<TValue>(
			TValue value)
		{
			AssertStrategyIsValid(typeof(TValue));

			try
			{
				byte[] sourceBytes;
				
				if (typeof(TValue) == typeof(byte[]))
				{
					sourceBytes = value as byte[];
				}
				else
				{
					var json = System.Text.Json.JsonSerializer.Serialize(value);
					sourceBytes = Encoding.UTF8.GetBytes(json);
				}

				var compressedBytes = new byte[LZ4Codec.MaximumOutputSize(sourceBytes.Length)];
				
				var compressedSize = LZ4Codec.Encode(
					sourceBytes,
					0,
					sourceBytes.Length,
					compressedBytes,
					0,
					compressedBytes.Length,
					compressionLevel);

				if (compressedSize < 0)
					return false;

				Array.Resize(ref compressedBytes, compressedSize);

				if (innerStrategy is IStrategyWithFilter appendStrategy)
					return appendStrategy.Append(compressedBytes);

				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool Append(
			Type valueType,
			object value)
		{
			AssertStrategyIsValid(valueType);

			try
			{
				byte[] sourceBytes;
				
				if (valueType == typeof(byte[]))
				{
					sourceBytes = value as byte[];
				}
				else
				{
					var json = System.Text.Json.JsonSerializer.Serialize(value, valueType);
					sourceBytes = Encoding.UTF8.GetBytes(json);
				}

				var compressedBytes = new byte[LZ4Codec.MaximumOutputSize(sourceBytes.Length)];
				
				var compressedSize = LZ4Codec.Encode(
					sourceBytes,
					0,
					sourceBytes.Length,
					compressedBytes,
					0,
					compressedBytes.Length,
					compressionLevel);

				if (compressedSize < 0)
					return false;

				Array.Resize(ref compressedBytes, compressedSize);

				if (innerStrategy is IStrategyWithFilter appendStrategy)
					return appendStrategy.Append(valueType, compressedBytes);

				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}

		#endregion

		#endregion

		#region IStrategyWithFilter

		public bool AllowsType<TValue>()
		{
			if (innerStrategy is IStrategyWithFilter filter)
				return filter.AllowsType<TValue>();

			return true;
		}

		public bool AllowsType(
			Type valueType)
		{
			if (innerStrategy is IStrategyWithFilter filter)
				return filter.AllowsType(valueType);

			return true;
		}

		#endregion

		#region Private methods

		private void AssertStrategyIsValid(Type type)
		{
			if (!AllowsType(type))
				throw new InvalidOperationException($"Strategy does not support type {type}");
		}

		#endregion
	}
}
