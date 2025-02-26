#if ZLIB_SUPPORT

using System;
using System.IO;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

using Ionic.Zlib;
using ZlibCodec = Ionic.Zlib.ZlibCodec;
using CompressionMode = Ionic.Zlib.CompressionMode;
using FlushType = Ionic.Zlib.FlushType;
using ZlibConstants = Ionic.Zlib.ZlibConstants;

namespace HereticalSolutions.Persistence
{
	public class ZLibCompressionConverter
		: AWrapperConverter
	{
		private readonly IByteArrayConverter byteArrayConverter;

		public ZLibCompressionConverter(
			IDataConverter innerDataConverter,
			IByteArrayConverter byteArrayConverter,
			ILogger logger)
			: base(
				innerDataConverter,
				logger)
		{
			this.byteArrayConverter = byteArrayConverter;
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

					return (false, default);
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

					return (false, default);
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
			try
			{
				// Get the original size (first 4 bytes)
				var originalSize = BitConverter.ToInt32(compressedData, 0);

				// Create a memory stream for the decompressed data
				using (var decompressedStream = new MemoryStream())
				{
					// Create a memory stream for the compressed data (skipping the first 4 bytes)
					using (var compressedStream =
						new MemoryStream(
							compressedData,
							4,
							compressedData.Length - 4))
					{
						// Decompress using Ionic.Zlib
						var buffer = new byte[4096];
						int read;
						
						// Use ZlibCodec for decompression
						var codec = new ZlibCodec(
							CompressionMode.Decompress);
						
						codec.InputBuffer = compressedStream.GetBuffer();
						codec.NextIn = 4;
						codec.AvailableBytesIn = (int)compressedStream.Length - 4;
						
						codec.OutputBuffer = buffer;
						
						while (codec.AvailableBytesIn > 0)
						{
							codec.NextOut = 0;
							codec.AvailableBytesOut = buffer.Length;
							
							var inflateStatus = codec.Inflate(FlushType.None);
							
							if (inflateStatus != ZlibConstants.Z_OK && inflateStatus != ZlibConstants.Z_STREAM_END)
							{
								throw new Exception($"Decompression error: {inflateStatus}");
							}
							
							decompressedStream.Write(buffer, 0, buffer.Length - codec.AvailableBytesOut);
							
							if (inflateStatus == ZlibConstants.Z_STREAM_END)
								break;
						}
						
						// Finish decompression
						do
						{
							codec.NextOut = 0;
							codec.AvailableBytesOut = buffer.Length;
							
							var inflateStatus = codec.Inflate(FlushType.Finish);
							
							if (inflateStatus != ZlibConstants.Z_OK && inflateStatus != ZlibConstants.Z_STREAM_END)
							{
								throw new Exception($"Decompression error: {inflateStatus}");
							}
							
							decompressedStream.Write(buffer, 0, buffer.Length - codec.AvailableBytesOut);
							
							if (inflateStatus == ZlibConstants.Z_STREAM_END)
								break;
							
						} while (true);
					}
					
					// Get the decompressed data
					decompressedData = decompressedStream.ToArray();
					
					// Verify the size
					if (decompressedData.Length != originalSize)
					{
						logger?.LogError(
							GetType(),
							$"DECOMPRESSED DATA SIZE ({decompressedData.Length}) DOES NOT MATCH ORIGINAL SIZE ({originalSize})");
						
						return false;
					}
					
					return true;
				}
			}
			catch (Exception ex)
			{
				logger?.LogError(
					GetType(),
					$"ERROR DECOMPRESSING DATA: {ex.Message}");
				
				decompressedData = null;
				return false;
			}
		}

		private bool Compress(
			byte[] dataToCompress,
			out byte[] compressedData)
		{
			try
			{
				// Create a memory stream for the compressed data (with 4 bytes at the beginning for original size)
				using (var compressedStream = new MemoryStream())
				{
					// Write the original size as the first 4 bytes
					compressedStream.Write(BitConverter.GetBytes(dataToCompress.Length), 0, 4);
					
					// Compress using Ionic.Zlib
					var codec = new ZlibCodec(
						CompressionMode.Compress);
					
					codec.InputBuffer = dataToCompress;
					codec.NextIn = 0;
					codec.AvailableBytesIn = dataToCompress.Length;
					
					var buffer = new byte[4096];
					codec.OutputBuffer = buffer;
					
					do
					{
						codec.NextOut = 0;
						codec.AvailableBytesOut = buffer.Length;
						
						var deflateStatus = codec.Deflate(codec.AvailableBytesIn == 0 ? FlushType.Finish : FlushType.None);
						
						if (deflateStatus != ZlibConstants.Z_OK && deflateStatus != ZlibConstants.Z_STREAM_END)
						{
							throw new Exception($"Compression error: {deflateStatus}");
						}
						
						compressedStream.Write(buffer, 0, buffer.Length - codec.AvailableBytesOut);
						
						if (deflateStatus == ZlibConstants.Z_STREAM_END)
							break;
						
					} while (codec.AvailableBytesIn > 0 || codec.AvailableBytesOut == 0);
					
					// Get the compressed data
					compressedData = compressedStream.ToArray();
					
					return true;
				}
			}
			catch (Exception ex)
			{
				logger?.LogError(
					GetType(),
					$"ERROR COMPRESSING DATA: {ex.Message}");
				
				compressedData = null;
				return false;
			}
		}
	}
}

#endif
