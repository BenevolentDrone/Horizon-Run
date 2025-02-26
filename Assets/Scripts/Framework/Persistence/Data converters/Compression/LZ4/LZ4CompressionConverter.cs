#if LZ4_SUPPORT

using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

using K4os.Compression.LZ4;

namespace HereticalSolutions.Persistence
{
	public class LZ4CompressionConverter
		: AWrapperConverter
	{
		private readonly IByteArrayConverter byteArrayConverter;

		private readonly LZ4Level compressionLevel;

		public LZ4CompressionConverter(
			IDataConverter innerDataConverter,
			IByteArrayConverter byteArrayConverter,
			LZ4Level compressionLevel,
			ILogger logger)
			: base(
				innerDataConverter,
				logger)
		{
			this.byteArrayConverter = byteArrayConverter;

			this.compressionLevel = compressionLevel;
		}

		#region IDataConverter

		#region Read

		public override bool ReadAndConvert<TValue>(
			IDataConverterCommandContext context,
			out TValue value)
		{
			value = default;

			var result = base.ReadAndConvert<byte[]>(
				context,
				out var compressedData);

			if (result)
			{
				if (!Decompress(
					compressedData,
					out var decompressedData))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT DECOMPRESS DATA");

					return false;
				}

				return byteArrayConverter.ConvertFromBytes<TValue>(
					decompressedData,
					out value);
			}

			logger?.LogError(
				GetType(),
				$"COULD NOT READ BYTE ARRAY");

			return false;
		}

		public override bool ReadAndConvert(
			Type valueType,
			IDataConverterCommandContext context,
			out object value)
		{
			value = default;

			var result = base.ReadAndConvert<byte[]>(
				context,
				out var compressedData);

			if (result)
			{
				if (!Decompress(
					compressedData,
					out var decompressedData))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT DECOMPRESS DATA");

					return false;
				}

				return byteArrayConverter.ConvertFromBytes(
					valueType,
					decompressedData,
					out value);
			}

			logger?.LogError(
				GetType(),
				$"COULD NOT READ BYTE ARRAY");

			return false;
		}

		#endregion

		#region Write

		public override bool ConvertAndWrite<TValue>(
			IDataConverterCommandContext context,
			TValue value)
		{
			if (!byteArrayConverter.ConvertToBytes<TValue>(
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {typeof(TValue).Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return base.ConvertAndWrite<byte[]>(
				context,
				compressedData);
		}

		public override bool ConvertAndWrite(
			Type valueType,
			IDataConverterCommandContext context,
			object value)
		{
			if (!byteArrayConverter.ConvertToBytes(
				valueType,
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {valueType.Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return base.ConvertAndWrite<byte[]>(
				context,
				compressedData);
		}

		#endregion

		#region Append

		public override bool ConvertAndAppend<TValue>(
			IDataConverterCommandContext context,
			TValue value)
		{
			if (!byteArrayConverter.ConvertToBytes<TValue>(
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {typeof(TValue).Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return base.ConvertAndAppend<byte[]>(
				context,
				compressedData);
		}

		public override bool ConvertAndAppend(
			Type valueType,
			IDataConverterCommandContext context,
			object value)
		{
			if (!byteArrayConverter.ConvertToBytes(
				valueType,
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {valueType.Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return base.ConvertAndAppend<byte[]>(
				context,
				compressedData);
		}

		#endregion

		#endregion

		#region IBlockDataConverter

		#region Read

		public override bool ReadBlockAndConvert<TValue>(
			IDataConverterCommandContext context,
			int blockOffset,
			int blockSize,
			out TValue value)
		{
			value = default;

			var result = base.ReadBlockAndConvert<byte[]>(
				context,
				blockOffset,
				blockSize,
				out var compressedData);

			if (result)
			{
				if (!Decompress(
					compressedData,
					out var decompressedData))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT DECOMPRESS DATA");

					return false;
				}

				return byteArrayConverter.ConvertFromBytes<TValue>(
					decompressedData,
					out value);
			}

			logger?.LogError(
				GetType(),
				$"COULD NOT READ BYTE ARRAY");

			return false;
		}

		public override bool ReadBlockAndConvert(
			Type valueType,
			IDataConverterCommandContext context,
			int blockOffset,
			int blockSize,
			out object value)
		{
			value = default;

			var result = base.ReadBlockAndConvert<byte[]>(
				context,
				blockOffset,
				blockSize,
				out var compressedData);

			if (result)
			{
				if (!Decompress(
					compressedData,
					out var decompressedData))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT DECOMPRESS DATA");

					return false;
				}

				return byteArrayConverter.ConvertFromBytes(
					valueType,
					decompressedData,
					out value);
			}

			logger?.LogError(
				GetType(),
				$"COULD NOT READ BYTE ARRAY");

			return false;
		}

		#endregion

		#region Write

		public override bool ConvertAndWriteBlock<TValue>(
			IDataConverterCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize)
		{
			if (!byteArrayConverter.ConvertToBytes<TValue>(
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {typeof(TValue).Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return base.ConvertAndWriteBlock<byte[]>(
				context,
				compressedData,
				blockOffset,
				blockSize);
		}

		public override bool ConvertAndWriteBlock(
			Type valueType,
			IDataConverterCommandContext context,
			object value,
			int blockOffset,
			int blockSize)
		{
			if (!byteArrayConverter.ConvertToBytes(
				valueType,
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {valueType.Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return base.ConvertAndWriteBlock<byte[]>(
				context,
				compressedData,
				blockOffset,
				blockSize);
		}

		#endregion

		#endregion

		#region IAsyncDataConverter

		#region Read

		public override async Task<(bool, TValue)> ReadAsyncAndConvert<TValue>(
			IDataConverterCommandContext context,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			TValue value = default;

			var result = await base.ReadAsyncAndConvert<byte[]>(
				context,
				asyncContext);

			if (result.Item1)
			{
				if (!Decompress(
					result.Item2,
					out var decompressedData))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT DECOMPRESS DATA");

					return (false, default);
				}

				if (!byteArrayConverter.ConvertFromBytes<TValue>(
					decompressedData,
					out value))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT VALUE FROM BYTES FOR TYPE {typeof(TValue).Name}");

					return (false, default(TValue));
				}
			}

			return (result.Item1, value);
		}

		public override async Task<(bool, object)> ReadAsyncAndConvert(
			Type valueType,
			IDataConverterCommandContext context,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			object value = default;

			var result = await base.ReadAsyncAndConvert<byte[]>(
				context,
				asyncContext);

			if (result.Item1)
			{
				if (!Decompress(
					result.Item2,
					out var decompressedData))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT DECOMPRESS DATA");

					return (false, default);
				}

				if (!byteArrayConverter.ConvertFromBytes(
					valueType,
					decompressedData,
					out value))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT VALUE FROM BYTES FOR TYPE {valueType.Name}");

					return (false, default);
				}
			}

			return (result.Item1, value);
		}

		#endregion

		#region Write

		public override async Task<bool> ConvertAndWriteAsync<TValue>(
			IDataConverterCommandContext context,
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (!byteArrayConverter.ConvertToBytes<TValue>(
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {typeof(TValue).Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return await base.ConvertAndWriteAsync<byte[]>(
				context,
				compressedData,
				asyncContext);
		}

		public override async Task<bool> ConvertAndWriteAsync(
			Type valueType,
			IDataConverterCommandContext context,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (!byteArrayConverter.ConvertToBytes(
				valueType,
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {valueType.Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return await base.ConvertAndWriteAsync<byte[]>(
				context,
				compressedData,
				asyncContext);
		}

		#endregion

		#region Append

		public override async Task<bool> ConvertAndAppendAsync<TValue>(
			IDataConverterCommandContext context,
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (!byteArrayConverter.ConvertToBytes<TValue>(
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {typeof(TValue).Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return await base.ConvertAndAppendAsync<byte[]>(
				context,
				compressedData,
				asyncContext);
		}

		public override async Task<bool> ConvertAndAppendAsync(
			Type valueType,
			IDataConverterCommandContext context,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (!byteArrayConverter.ConvertToBytes(
				valueType,
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {valueType.Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return await base.ConvertAndAppendAsync<byte[]>(
				context,
				compressedData,
				asyncContext);
		}

		#endregion

		#endregion

		#region IAsyncBlockDataConverter

		#region Read

		public override async Task<(bool, TValue)> ReadBlockAsyncAndConvert<TValue>(
			IDataConverterCommandContext context,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			TValue value = default;

			var result = await base.ReadBlockAsyncAndConvert<byte[]>(
				context,
				blockOffset,
				blockSize,
				asyncContext);

			if (result.Item1)
			{
				if (!Decompress(
					result.Item2,
					out var decompressedData))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT DECOMPRESS DATA");

					return (false, default);
				}

				if (!byteArrayConverter.ConvertFromBytes<TValue>(
					decompressedData,
					out value))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT VALUE FROM BYTES FOR TYPE {typeof(TValue).Name}");

					return (false, default(TValue));
				}
			}

			return (result.Item1, value);
		}

		public override async Task<(bool, object)> ReadBlockAsyncAndConvert(
			Type valueType,
			IDataConverterCommandContext context,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			object value = default;

			var result = await base.ReadBlockAsyncAndConvert<byte[]>(
				context,
				blockOffset,
				blockSize,
				asyncContext);

			if (result.Item1)
			{
				if (!Decompress(
					result.Item2,
					out var decompressedData))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT DECOMPRESS DATA");

					return (false, default);
				}

				if (!byteArrayConverter.ConvertFromBytes(
					valueType,
					decompressedData,
					out value))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT VALUE FROM BYTES FOR TYPE {valueType.Name}");

					return (false, default);
				}
			}

			return (result.Item1, value);
		}

		#endregion

		#region Write

		public override async Task<bool> ConvertAndWriteBlockAsync<TValue>(
			IDataConverterCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (!byteArrayConverter.ConvertToBytes<TValue>(
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {typeof(TValue).Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return await base.ConvertAndWriteBlockAsync<byte[]>(
				context,
				compressedData,
				blockOffset,
				blockSize,
				asyncContext);
		}

		public override async Task<bool> ConvertAndWriteBlockAsync(
			Type valueType,
			IDataConverterCommandContext context,
			object value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (!byteArrayConverter.ConvertToBytes(
				valueType,
				value,
				out var dataToCompress))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT VALUE TO BYTE ARRAY: {valueType.Name}");

				return false;
			}

			if (!Compress(
				dataToCompress,
				out var compressedData))
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			return await base.ConvertAndWriteBlockAsync<byte[]>(
				context,
				compressedData,
				blockOffset,
				blockSize,
				asyncContext);
		}

		#endregion

		#endregion

		private bool Decompress(
			byte[] compressedData,
			out byte[] decompressedData)
		{
			// Get the original size (first 4 bytes)
			var originalSize = BitConverter.ToInt32(compressedData, 0);

			// Decompress the data
			decompressedData = new byte[originalSize];

			var actualSize = LZ4Codec.Decode(
				compressedData,
				4,
				compressedData.Length - 4,
				decompressedData,
				0,
				decompressedData.Length);

			if (actualSize != originalSize)
			{
				logger?.LogError(
					GetType(),
					$"DECOMPRESSED DATA SIZE ({actualSize}) DOES NOT MATCH ORIGINAL SIZE ({originalSize})");

				return false;
			}

			return true;
		}

		private bool Compress(
			byte[] dataToCompress,
			out byte[] compressedData)
		{
			// Calculate the worst-case compression size
			var maxCompressedLength = LZ4Codec.MaximumOutputSize(dataToCompress.Length);

			// Create buffer for compressed data (4 bytes for original size + compressed data)
			compressedData = new byte[4 + maxCompressedLength];

			// Store the original size in the first 4 bytes
			BitConverter.GetBytes(dataToCompress.Length).CopyTo(compressedData, 0);

			// Compress the data
			var compressedSize = LZ4Codec.Encode(
				dataToCompress,
				0,
				dataToCompress.Length,
				compressedData,
				4,
				maxCompressedLength,
				compressionLevel);

			if (compressedSize <= 0)
			{
				logger?.LogError(
					GetType(),
					"COULD NOT COMPRESS DATA");

				return false;
			}

			// Resize the array to actual size
			Array.Resize(ref compressedData, 4 + compressedSize);

			return true;
		}
	}
}

#endif