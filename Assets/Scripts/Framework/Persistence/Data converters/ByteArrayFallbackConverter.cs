using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public class ByteArrayFallbackConverter
		: AWrapperConverter
	{
		private readonly IReadOnlyRepository<Type, Delegate>
			convertFromBytesDelegateRepository;

		private readonly IReadOnlyRepository<Type, Delegate>
			convertToBytesDelegateRepository;

		public ByteArrayFallbackConverter(
			IReadOnlyRepository<Type, Delegate> convertFromBytesDelegateRepository,
			IReadOnlyRepository<Type, Delegate> convertToBytesDelegateRepository,
			IDataConverter innerDataConverter,
			ILogger logger)
			: base(
				innerDataConverter,
				logger)
		{
			this.convertFromBytesDelegateRepository = convertFromBytesDelegateRepository;

			this.convertToBytesDelegateRepository = convertToBytesDelegateRepository;
		}

		#region IDataConverter

		#region Read

		public override bool ReadAndConvert<TValue>(
			IDataConverterCommandContext context,
			out TValue value)
		{
			bool useFallback = ShouldUseFallback<TValue>(
				context,
				out bool success);

			if (!success)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} NEITHER ALLOWS TYPE {nameof(TValue)} NOR TYPE {typeof(byte[])}");

				value = default;

				return false;
			}
			
			if (!useFallback)
				return base.ReadAndConvert<TValue>(
					context,
					out value);

			value = default;

			var result = base.ReadAndConvert<byte[]>(
				context,
				out var byteArray);

			if (result)
			{
				if (!convertFromBytesDelegateRepository.TryGet(
					typeof(TValue),
					out var @delegate))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT FIND DELEGATE TO CONVERT VALUE FROM BYTES FOR TYPE {nameof(TValue)}");

					value = default;

					return false;
				}

				Func<byte[], TValue> convertFromBytesDelegate =
					(Func<byte[], TValue>)@delegate;

				if (convertFromBytesDelegate == null)
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE FROM BYTES DELEGATE FOR TYPE {nameof(TValue)}");

					value = default;

					return false;
				}

				value = convertFromBytesDelegate.Invoke(
					byteArray);
			}

			return result;
		}

		public override bool ReadAndConvert(
			Type valueType,
			IDataConverterCommandContext context,
			out object value)
		{
			bool useFallback = ShouldUseFallback(
				valueType,
				context,
				out bool success);

			if (!success)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} NEITHER ALLOWS TYPE {valueType.Name} NOR TYPE {typeof(byte[])}");

				value = default;

				return false;
			}

			if (!useFallback)
				return base.ReadAndConvert(
					valueType,
					context,
					out value);

			value = default;

			var result = base.ReadAndConvert<byte[]>(
				context,
				out var byteArray);

			if (result)
			{
				if (!convertFromBytesDelegateRepository.TryGet(
					valueType,
					out var @delegate))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT FIND DELEGATE TO CONVERT VALUE FROM BYTES FOR TYPE {valueType.Name}");

					value = default;

					return false;
				}

				Func<byte[], object> convertFromBytesDelegate =
					(Func<byte[], object>)@delegate;

				if (convertFromBytesDelegate == null)
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE FROM BYTES DELEGATE FOR TYPE {valueType}");

					value = default;

					return false;
				}

				value = convertFromBytesDelegate.Invoke(
					byteArray);
			}

			return result;
		}

		#endregion

		#region Write

		public override bool ConvertAndWrite<TValue>(
			IDataConverterCommandContext context,
			TValue value)
		{
			bool useFallback = ShouldUseFallback<TValue>(
				context,
				out bool success);

			if (!success)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} NEITHER ALLOWS TYPE {nameof(TValue)} NOR TYPE {typeof(byte[])}");

				return false;
			}

			if (!useFallback)
				return base.ConvertAndWrite<TValue>(
					context,
					value);

			if (!convertToBytesDelegateRepository.TryGet(
				typeof(TValue),
				out var @delegate))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT FIND DELEGATE TO CONVERT VALUE TO BYTES FOR TYPE {nameof(TValue)}");

				return false;
			}

			Func<TValue, byte[]> convertToBytesDelegate =
				(Func<TValue, byte[]>)@delegate;

			if (convertToBytesDelegate == null)
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE TO BYTES DELEGATE FOR TYPE {nameof(TValue)}");

				return false;
			}

			return base.ConvertAndWrite<byte[]>(
				context,
				convertToBytesDelegate.Invoke(
					value));
		}

		public override bool ConvertAndWrite(
			Type valueType,
			IDataConverterCommandContext context,
			object value)
		{
			bool useFallback = ShouldUseFallback(
				valueType,
				context,
				out bool success);

			if (!success)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} NEITHER ALLOWS TYPE {valueType.Name} NOR TYPE {typeof(byte[])}");

				return false;
			}

			if (!useFallback)
				return base.ConvertAndWrite(
					valueType,
					context,
					value);

			if (!convertToBytesDelegateRepository.TryGet(
				valueType,
				out var @delegate))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT FIND DELEGATE TO CONVERT VALUE TO BYTES FOR TYPE {valueType.Name}");

				return false;
			}

			Func<object, byte[]> convertToBytesDelegate =
				(Func<object, byte[]>)@delegate;

			if (convertToBytesDelegate == null)
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE TO BYTES DELEGATE FOR TYPE {valueType.Name}");

				return false;
			}

			return base.ConvertAndWrite<byte[]>(
				context,
				convertToBytesDelegate.Invoke(
					value));
		}

		#endregion

		#region Append

		public override bool ConvertAndAppend<TValue>(
			IDataConverterCommandContext context,
			TValue value)
		{
			bool useFallback = ShouldUseFallback<TValue>(
				context,
				out bool success);

			if (!success)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} NEITHER ALLOWS TYPE {nameof(TValue)} NOR TYPE {typeof(byte[])}");

				return false;
			}

			if (!useFallback)
				return base.ConvertAndAppend<TValue>(
					context,
					value);

			if (!convertToBytesDelegateRepository.TryGet(
				typeof(TValue),
				out var @delegate))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT FIND DELEGATE TO CONVERT VALUE TO BYTES FOR TYPE {nameof(TValue)}");

				return false;
			}

			Func<TValue, byte[]> convertToBytesDelegate =
				(Func<TValue, byte[]>)@delegate;

			if (convertToBytesDelegate == null)
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE TO BYTES DELEGATE FOR TYPE {nameof(TValue)}");

				return false;
			}

			return base.ConvertAndAppend<byte[]>(
				context,
				convertToBytesDelegate.Invoke(
					value));
		}

		public override bool ConvertAndAppend(
			Type valueType,
			IDataConverterCommandContext context,
			object value)
		{
			bool useFallback = ShouldUseFallback(
				valueType,
				context,
				out bool success);

			if (!success)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} NEITHER ALLOWS TYPE {valueType.Name} NOR TYPE {typeof(byte[])}");

				return false;
			}

			if (!useFallback)
				return base.ConvertAndAppend(
					valueType,
					context,
					value);

			if (!convertToBytesDelegateRepository.TryGet(
				valueType,
				out var @delegate))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT FIND DELEGATE TO CONVERT VALUE TO BYTES FOR TYPE {valueType.Name}");

				return false;
			}

			Func<object, byte[]> convertToBytesDelegate =
				(Func<object, byte[]>)@delegate;

			if (convertToBytesDelegate == null)
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE TO BYTES DELEGATE FOR TYPE {valueType.Name}");

				return false;
			}

			return base.ConvertAndAppend<byte[]>(
				context,
				convertToBytesDelegate.Invoke(
					value));
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
			bool useFallback = ShouldUseFallback<TValue>(
				context,
				out bool success);

			if (!success)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} NEITHER ALLOWS TYPE {nameof(TValue)} NOR TYPE {typeof(byte[])}");

				value = default;

				return false;
			}

			if (!useFallback)
				return base.ReadBlockAndConvert<TValue>(
					context,
					blockOffset,
					blockSize,
					out value);

			value = default;

			var result = base.ReadBlockAndConvert<byte[]>(
				context,
				blockOffset,
				blockSize,
				out var byteArray);

			if (result)
			{
				if (!convertFromBytesDelegateRepository.TryGet(
					typeof(TValue),
					out var @delegate))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT FIND DELEGATE TO CONVERT VALUE FROM BYTES FOR TYPE {nameof(TValue)}");

					value = default;

					return false;
				}

				Func<byte[], TValue> convertFromBytesDelegate =
					(Func<byte[], TValue>)@delegate;

				if (convertFromBytesDelegate == null)
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE FROM BYTES DELEGATE FOR TYPE {nameof(TValue)}");

					value = default;

					return false;
				}

				value = convertFromBytesDelegate.Invoke(
					byteArray);
			}

			return result;
		}

		public override bool ReadBlockAndConvert(
			Type valueType,
			IDataConverterCommandContext context,
			int blockOffset,
			int blockSize,
			out object value)
		{
			bool useFallback = ShouldUseFallback(
				valueType,
				context,
				out bool success);

			if (!success)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} NEITHER ALLOWS TYPE {valueType.Name} NOR TYPE {typeof(byte[])}");

				value = default;

				return false;
			}

			if (!useFallback)
				return base.ReadBlockAndConvert(
					valueType,
					context,
					blockOffset,
					blockSize,
					out value);

			value = default;

			var result = base.ReadBlockAndConvert<byte[]>(
				context,
				blockOffset,
				blockSize,
				out var byteArray);

			if (result)
			{
				if (!convertFromBytesDelegateRepository.TryGet(
					valueType,
					out var @delegate))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT FIND DELEGATE TO CONVERT VALUE FROM BYTES FOR TYPE {valueType.Name}");

					value = default;

					return false;
				}

				Func<byte[], object> convertFromBytesDelegate =
					(Func<byte[], object>)@delegate;

				if (convertFromBytesDelegate == null)
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE FROM BYTES DELEGATE FOR TYPE {valueType}");

					value = default;

					return false;
				}

				value = convertFromBytesDelegate.Invoke(
					byteArray);
			}

			return result;
		}

		#endregion

		#region Write

		public override bool ConvertAndWriteBlock<TValue>(
			IDataConverterCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize)
		{
			bool useFallback = ShouldUseFallback<TValue>(
				context,
				out bool success);

			if (!success)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} NEITHER ALLOWS TYPE {nameof(TValue)} NOR TYPE {typeof(byte[])}");

				return false;
			}

			if (!useFallback)
				return base.ConvertAndWriteBlock<TValue>(
					context,
					value,
					blockOffset,
					blockSize);

			if (!convertToBytesDelegateRepository.TryGet(
				typeof(TValue),
				out var @delegate))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT FIND DELEGATE TO CONVERT VALUE TO BYTES FOR TYPE {nameof(TValue)}");

				return false;
			}

			Func<TValue, byte[]> convertToBytesDelegate =
				(Func<TValue, byte[]>)@delegate;

			if (convertToBytesDelegate == null)
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE TO BYTES DELEGATE FOR TYPE {nameof(TValue)}");

				return false;
			}

			return base.ConvertAndWriteBlock<byte[]>(
				context,
				convertToBytesDelegate.Invoke(
					value),
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
			bool useFallback = ShouldUseFallback(
				valueType,
				context,
				out bool success);

			if (!success)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} NEITHER ALLOWS TYPE {valueType.Name} NOR TYPE {typeof(byte[])}");

				return false;
			}

			if (!useFallback)
				return base.ConvertAndWriteBlock(
					valueType,
					context,
					value,
					blockOffset,
					blockSize);

			if (!convertToBytesDelegateRepository.TryGet(
				valueType,
				out var @delegate))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT FIND DELEGATE TO CONVERT VALUE TO BYTES FOR TYPE {valueType.Name}");

				return false;
			}

			Func<object, byte[]> convertToBytesDelegate =
				(Func<object, byte[]>)@delegate;

			if (convertToBytesDelegate == null)
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE TO BYTES DELEGATE FOR TYPE {valueType.Name}");

				return false;
			}

			return base.ConvertAndWriteBlock<byte[]>(
				context,
				convertToBytesDelegate.Invoke(
					value),
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
			bool useFallback = ShouldUseFallback<TValue>(
				context,
				out bool success);

			if (!success)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} NEITHER ALLOWS TYPE {nameof(TValue)} NOR TYPE {typeof(byte[])}");

				return (false, default(TValue));
			}

			if (!useFallback)
				return await base.ReadAsyncAndConvert<TValue>(
					context,
					asyncContext);

			TValue value = default;

			var result = await base.ReadAsyncAndConvert<byte[]>(
				context,
				asyncContext);

			if (result.Item1)
			{
				if (!convertFromBytesDelegateRepository.TryGet(
					typeof(TValue),
					out var @delegate))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT FIND DELEGATE TO CONVERT VALUE FROM BYTES FOR TYPE {nameof(TValue)}");

					return (false, default(TValue));
				}

				Func<byte[], TValue> convertFromBytesDelegate =
					(Func<byte[], TValue>)@delegate;

				if (convertFromBytesDelegate == null)
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE FROM BYTES DELEGATE FOR TYPE {nameof(TValue)}");

					return (false, default(TValue));
				}

				value = convertFromBytesDelegate.Invoke(
					result.Item2);
			}

			return (result.Item1, value);
		}

		public override async Task<(bool, object)> ReadAsyncAndConvert(
			Type valueType,
			IDataConverterCommandContext context,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			bool useFallback = ShouldUseFallback(
				valueType,
				context,
				out bool success);

			if (!success)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} NEITHER ALLOWS TYPE {valueType.Name} NOR TYPE {typeof(byte[])}");

				return (false, default);
			}

			if (!useFallback)
				return await base.ReadAsyncAndConvert(
					valueType,
					context,
					asyncContext);

			object value = default;

			var result = await base.ReadAsyncAndConvert<byte[]>(
				context,
				asyncContext);

			if (result.Item1)
			{
				if (!convertFromBytesDelegateRepository.TryGet(
					valueType,
					out var @delegate))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT FIND DELEGATE TO CONVERT VALUE FROM BYTES FOR TYPE {valueType.Name}");

					return (false, default);
				}

				Func<byte[], object> convertFromBytesDelegate =
					(Func<byte[], object>)@delegate;

				if (convertFromBytesDelegate == null)
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE FROM BYTES DELEGATE FOR TYPE {valueType.Name}");

					return (false, default);
				}

				value = convertFromBytesDelegate.Invoke(
					result.Item2);
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
			bool useFallback = ShouldUseFallback<TValue>(
				context,
				out bool success);

			if (!success)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} NEITHER ALLOWS TYPE {nameof(TValue)} NOR TYPE {typeof(byte[])}");

				return false;
			}

			if (!useFallback)
				return await base.ConvertAndWriteAsync<TValue>(
					context,
					value,
					asyncContext);

			if (!convertToBytesDelegateRepository.TryGet(
				typeof(TValue),
				out var @delegate))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT FIND DELEGATE TO CONVERT VALUE TO BYTES FOR TYPE {nameof(TValue)}");

				return false;
			}

			Func<TValue, byte[]> convertToBytesDelegate =
				(Func<TValue, byte[]>)@delegate;

			if (convertToBytesDelegate == null)
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE TO BYTES DELEGATE FOR TYPE {nameof(TValue)}");

				return false;
			}

			return await base.ConvertAndWriteAsync<byte[]>(
				context,
				convertToBytesDelegate.Invoke(
					value),
				asyncContext);
		}

		public override async Task<bool> ConvertAndWriteAsync(
			Type valueType,
			IDataConverterCommandContext context,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			bool useFallback = ShouldUseFallback(
				valueType,
				context,
				out bool success);

			if (!success)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} NEITHER ALLOWS TYPE {valueType.Name} NOR TYPE {typeof(byte[])}");

				return false;
			}

			if (!useFallback)
				return await base.ConvertAndWriteAsync(
					valueType,
					context,
					value,
					asyncContext);

			if (!convertToBytesDelegateRepository.TryGet(
				valueType,
				out var @delegate))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT FIND DELEGATE TO CONVERT VALUE TO BYTES FOR TYPE {valueType.Name}");

				return false;
			}

			Func<object, byte[]> convertToBytesDelegate =
				(Func<object, byte[]>)@delegate;

			if (convertToBytesDelegate == null)
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE TO BYTES DELEGATE FOR TYPE {valueType.Name}");

				return false;
			}

			return await base.ConvertAndWriteAsync<byte[]>(
				context,
				convertToBytesDelegate.Invoke(
					value),
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
			bool useFallback = ShouldUseFallback<TValue>(
				context,
				out bool success);

			if (!success)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} NEITHER ALLOWS TYPE {nameof(TValue)} NOR TYPE {typeof(byte[])}");

				return false;
			}

			if (!useFallback)
				return await base.ConvertAndAppendAsync<TValue>(
					context,
					value,
					asyncContext);

			if (!convertToBytesDelegateRepository.TryGet(
				typeof(TValue),
				out var @delegate))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT FIND DELEGATE TO CONVERT VALUE TO BYTES FOR TYPE {nameof(TValue)}");

				return false;
			}

			Func<TValue, byte[]> convertToBytesDelegate =
				(Func<TValue, byte[]>)@delegate;

			if (convertToBytesDelegate == null)
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE TO BYTES DELEGATE FOR TYPE {nameof(TValue)}");

				return false;
			}

			return await base.ConvertAndAppendAsync<byte[]>(
				context,
				convertToBytesDelegate.Invoke(
					value),
				asyncContext);
		}

		public override async Task<bool> ConvertAndAppendAsync(
			Type valueType,
			IDataConverterCommandContext context,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			bool useFallback = ShouldUseFallback(
				valueType,
				context,
				out bool success);

			if (!success)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} NEITHER ALLOWS TYPE {valueType.Name} NOR TYPE {typeof(byte[])}");

				return false;
			}

			if (!useFallback)
				return await base.ConvertAndAppendAsync(
					valueType,
					context,
					value,
					asyncContext);

			if (!convertToBytesDelegateRepository.TryGet(
				valueType,
				out var @delegate))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT FIND DELEGATE TO CONVERT VALUE TO BYTES FOR TYPE {valueType.Name}");

				return false;
			}

			Func<object, byte[]> convertToBytesDelegate =
				(Func<object, byte[]>)@delegate;

			if (convertToBytesDelegate == null)
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE TO BYTES DELEGATE FOR TYPE {valueType.Name}");

				return false;
			}

			return await base.ConvertAndAppendAsync<byte[]>(
				context,
				convertToBytesDelegate.Invoke(
					value),
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
			bool useFallback = ShouldUseFallback<TValue>(
				context,
				out bool success);

			if (!success)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} NEITHER ALLOWS TYPE {nameof(TValue)} NOR TYPE {typeof(byte[])}");

				return (false, default(TValue));
			}

			if (!useFallback)
				return await base.ReadBlockAsyncAndConvert<TValue>(
					context,
					blockOffset,
					blockSize,
					asyncContext);

			TValue value = default;

			var result = await base.ReadBlockAsyncAndConvert<byte[]>(
				context,
				blockOffset,
				blockSize,
				asyncContext);

			if (result.Item1)
			{
				if (!convertFromBytesDelegateRepository.TryGet(
					typeof(TValue),
					out var @delegate))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT FIND DELEGATE TO CONVERT VALUE FROM BYTES FOR TYPE {nameof(TValue)}");

					return (false, default(TValue));
				}

				Func<byte[], TValue> convertFromBytesDelegate =
					(Func<byte[], TValue>)@delegate;

				if (convertFromBytesDelegate == null)
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE FROM BYTES DELEGATE FOR TYPE {nameof(TValue)}");

					return (false, default(TValue));
				}

				value = convertFromBytesDelegate.Invoke(
					result.Item2);
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
			bool useFallback = ShouldUseFallback(
				valueType,
				context,
				out bool success);

			if (!success)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} NEITHER ALLOWS TYPE {valueType.Name} NOR TYPE {typeof(byte[])}");

				return (false, default);
			}

			if (!useFallback)
				return await base.ReadBlockAsyncAndConvert(
					valueType,
					context,
					blockOffset,
					blockSize,
					asyncContext);

			object value = default;

			var result = await base.ReadBlockAsyncAndConvert<byte[]>(
				context,
				blockOffset,
				blockSize,
				asyncContext);

			if (result.Item1)
			{
				if (!convertFromBytesDelegateRepository.TryGet(
					valueType,
					out var @delegate))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT FIND DELEGATE TO CONVERT VALUE FROM BYTES FOR TYPE {valueType.Name}");

					return (false, default);
				}

				Func<byte[], object> convertFromBytesDelegate =
					(Func<byte[], object>)@delegate;

				if (convertFromBytesDelegate == null)
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE FROM BYTES DELEGATE FOR TYPE {valueType.Name}");

					return (false, default);
				}

				value = convertFromBytesDelegate.Invoke(
					result.Item2);
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
			bool useFallback = ShouldUseFallback<TValue>(
				context,
				out bool success);

			if (!success)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} NEITHER ALLOWS TYPE {nameof(TValue)} NOR TYPE {typeof(byte[])}");

				return false;
			}

			if (!useFallback)
				return await base.ConvertAndWriteBlockAsync<TValue>(
					context,
					value,
					blockOffset,
					blockSize,
					asyncContext);

			if (!convertToBytesDelegateRepository.TryGet(
				typeof(TValue),
				out var @delegate))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT FIND DELEGATE TO CONVERT VALUE TO BYTES FOR TYPE {nameof(TValue)}");

				return false;
			}

			Func<TValue, byte[]> convertToBytesDelegate =
				(Func<TValue, byte[]>)@delegate;

			if (convertToBytesDelegate == null)
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE TO BYTES DELEGATE FOR TYPE {nameof(TValue)}");

				return false;
			}

			return await base.ConvertAndWriteBlockAsync<byte[]>(
				context,
				convertToBytesDelegate.Invoke(
					value),
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
			bool useFallback = ShouldUseFallback(
				valueType,
				context,
				out bool success);

			if (!success)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} NEITHER ALLOWS TYPE {valueType.Name} NOR TYPE {typeof(byte[])}");

				return false;
			}

			if (!useFallback)
				return await base.ConvertAndWriteBlockAsync(
					valueType,
					context,
					value,
					blockOffset,
					blockSize,
					asyncContext);

			if (!convertToBytesDelegateRepository.TryGet(
				valueType,
				out var @delegate))
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT FIND DELEGATE TO CONVERT VALUE TO BYTES FOR TYPE {valueType.Name}");

				return false;
			}

			Func<object, byte[]> convertToBytesDelegate =
				(Func<object, byte[]>)@delegate;

			if (convertToBytesDelegate == null)
			{
				logger?.LogError(
					GetType(),
					$"COULD NOT CONVERT DELEGATE TO CONVERT VALUE TO BYTES DELEGATE FOR TYPE {valueType.Name}");

				return false;
			}

			return await base.ConvertAndWriteBlockAsync<byte[]>(
				context,
				convertToBytesDelegate.Invoke(
					value),
				blockOffset,
				blockSize,
				asyncContext);
		}

		#endregion

		#endregion

		private bool ShouldUseFallback<TValue>(
			IDataConverterCommandContext context,
			out bool success)
		{
			IStrategyWithFilter strategyWithFilter =
				context.SerializationStrategy as IStrategyWithFilter;

			if (strategyWithFilter == null
				|| strategyWithFilter.AllowsType<TValue>())
			{
				success = true;

				return false;
			}

			//Almost anything can be converted to a byte array
			//Though if we've already tried a byte array then there's no point in trying again
			if (typeof(TValue) != typeof(byte[])
				&& strategyWithFilter.AllowsType<byte[]>())
			{
				success = true;

				return true;
			}

			success = false;

			return false;
		}

		private bool ShouldUseFallback(
			Type valueType,
			IDataConverterCommandContext context,
			out bool success)
		{
			IStrategyWithFilter strategyWithFilter =
				context.SerializationStrategy as IStrategyWithFilter;

			if (strategyWithFilter == null
				|| strategyWithFilter.AllowsType(
					valueType))
			{
				success = true;

				return false;
			}

			//Almost anything can be converted to a byte array
			//Though if we've already tried a byte array then there's no point in trying again
			if (valueType != typeof(byte[])
				&& strategyWithFilter.AllowsType<byte[]>())
			{
				success = true;

				return true;
			}

			success = false;

			return false;
		}
	}
}