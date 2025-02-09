using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public class InvokeStrategyConverter
		: IDataConverter,
		  IBlockDataConverter,
		  IAsyncDataConverter,
		  IAsyncBlockDataConverter
	{
		private readonly ILogger logger;

		public InvokeStrategyConverter(
			ILogger logger)
		{
			this.logger = logger;
		}

		#region IDataConverter

		#region Read

		public bool ReadAndConvert<TValue>(
			IDataConverterCommandContext context,
			out TValue value)
		{
			return context.SerializationStrategy.Read<TValue>(
				out value);
		}

		public bool ReadAndConvert(
			Type valueType,
			IDataConverterCommandContext context,
			out object value)
		{
			return context.SerializationStrategy.Read(
				valueType,
				out value);
		}

		#endregion

		#region Write

		public bool ConvertAndWrite<TValue>(
			IDataConverterCommandContext context,
			TValue value)
		{
			return context.SerializationStrategy.Write<TValue>(
				value);
		}

		public bool ConvertAndWrite(
			Type valueType,
			IDataConverterCommandContext context,
			object value)
		{
			return context.SerializationStrategy.Write(
				valueType,
				value);
		}

		#endregion

		#region Append

		public bool ConvertAndAppend<TValue>(
			IDataConverterCommandContext context,
			TValue value)
		{
			return context.SerializationStrategy.Append<TValue>(
				value);
		}

		public bool ConvertAndAppend(
			Type valueType,
			IDataConverterCommandContext context,
			object value)
		{
			return context.SerializationStrategy.Append(
				valueType,
				value);
		}

		#endregion

		#endregion

		#region IBlockDataConverter

		#region Read

		public bool ReadBlockAndConvert<TValue>(
			IDataConverterCommandContext context,
			int blockOffset,
			int blockSize,
			out TValue value)
		{
			IBlockSerializationStrategy blockSerializationStrategy =
				context.SerializationStrategy as IBlockSerializationStrategy;

			if (blockSerializationStrategy == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} IS NOT A BLOCK SERIALIZATION STRATEGY");

				value = default(TValue);

				return false;
			}

			return blockSerializationStrategy.ReadBlock<TValue>(
				blockOffset,
				blockSize,
				out value);
		}

		public bool ReadBlockAndConvert(
			Type valueType,
			IDataConverterCommandContext context,
			int blockOffset,
			int blockSize,
			out object value)
		{
			IBlockSerializationStrategy blockSerializationStrategy =
				context.SerializationStrategy as IBlockSerializationStrategy;

			if (blockSerializationStrategy == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} IS NOT A BLOCK SERIALIZATION STRATEGY");

				value = default;

				return false;
			}

			return blockSerializationStrategy.ReadBlock(
				valueType,
				blockOffset,
				blockSize,
				out value);
		}

		#endregion

		#region Write

		public bool ConvertAndWriteBlock<TValue>(
			IDataConverterCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize)
		{
			IBlockSerializationStrategy blockSerializationStrategy =
				context.SerializationStrategy as IBlockSerializationStrategy;

			if (blockSerializationStrategy == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} IS NOT A BLOCK SERIALIZATION STRATEGY");

				return false;
			}

			return blockSerializationStrategy.WriteBlock<TValue>(
				value,
				blockOffset,
				blockSize);
		}

		public bool ConvertAndWriteBlock(
			Type valueType,
			IDataConverterCommandContext context,
			object value,
			int blockOffset,
			int blockSize)
		{
			IBlockSerializationStrategy blockSerializationStrategy =
				context.SerializationStrategy as IBlockSerializationStrategy;

			if (blockSerializationStrategy == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} IS NOT A BLOCK SERIALIZATION STRATEGY");

				return false;
			}

			return blockSerializationStrategy.WriteBlock(
				valueType,
				value,
				blockOffset,
				blockSize);
		}

		#endregion

		#endregion

		#region IAsyncDataConverter

		#region Read

		public async Task<(bool, TValue)> ReadAsyncAndConvert<TValue>(
			IDataConverterCommandContext context,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncSerializationStrategy asyncSerializationStrategy =
				context.SerializationStrategy as IAsyncSerializationStrategy;

			if (asyncSerializationStrategy == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} IS NOT AN ASYNC SERIALIZATION STRATEGY");

				return (false, default(TValue));
			}

			return await asyncSerializationStrategy.ReadAsync<TValue>(
				asyncContext);
		}

		public async Task<(bool, object)> ReadAsyncAndConvert(
			Type valueType,
			IDataConverterCommandContext context,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncSerializationStrategy asyncSerializationStrategy =
				context.SerializationStrategy as IAsyncSerializationStrategy;

			if (asyncSerializationStrategy == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} IS NOT AN ASYNC SERIALIZATION STRATEGY");

				return (false, default);
			}

			return await asyncSerializationStrategy.ReadAsync(
				valueType,
				asyncContext);
		}

		#endregion

		#region Write

		public async Task<bool> ConvertAndWriteAsync<TValue>(
			IDataConverterCommandContext context,
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncSerializationStrategy asyncSerializationStrategy =
				context.SerializationStrategy as IAsyncSerializationStrategy;

			if (asyncSerializationStrategy == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} IS NOT AN ASYNC SERIALIZATION STRATEGY");

				return false;
			}

			return await asyncSerializationStrategy.WriteAsync<TValue>(
				value,
				asyncContext);
		}

		public async Task<bool> ConvertAndWriteAsync(
			Type valueType,
			IDataConverterCommandContext context,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncSerializationStrategy asyncSerializationStrategy =
				context.SerializationStrategy as IAsyncSerializationStrategy;

			if (asyncSerializationStrategy == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} IS NOT AN ASYNC SERIALIZATION STRATEGY");

				return false;
			}

			return await asyncSerializationStrategy.WriteAsync(
				valueType,
				value,
				asyncContext);
		}

		#endregion

		#region Append

		public async Task<bool> ConvertAndAppendAsync<TValue>(
			IDataConverterCommandContext context,
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncSerializationStrategy asyncSerializationStrategy =
				context.SerializationStrategy as IAsyncSerializationStrategy;

			if (asyncSerializationStrategy == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} IS NOT AN ASYNC SERIALIZATION STRATEGY");

				return false;
			}

			return await asyncSerializationStrategy.AppendAsync<TValue>(
				value,
				asyncContext);
		}

		public async Task<bool> ConvertAndAppendAsync(
			Type valueType,
			IDataConverterCommandContext context,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncSerializationStrategy asyncSerializationStrategy =
				context.SerializationStrategy as IAsyncSerializationStrategy;

			if (asyncSerializationStrategy == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} IS NOT AN ASYNC SERIALIZATION STRATEGY");

				return false;
			}

			return await asyncSerializationStrategy.AppendAsync(
				valueType,
				value,
				asyncContext);
		}

		#endregion

		#endregion

		#region IAsyncBlockDataConverter
		
		#region Read

		public async Task<(bool, TValue)> ReadBlockAsyncAndConvert<TValue>(
			IDataConverterCommandContext context,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncBlockSerializationStrategy asyncBlockSerializationStrategy =
				context.SerializationStrategy as IAsyncBlockSerializationStrategy;

			if (asyncBlockSerializationStrategy == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} IS NOT AN ASYNC BLOCK SERIALIZATION STRATEGY");

				return (false, default(TValue));
			}

			return await asyncBlockSerializationStrategy.ReadBlockAsync<TValue>(
				blockOffset,
				blockSize,
				asyncContext);
		}

		public async Task<(bool, object)> ReadBlockAsyncAndConvert(
			Type valueType,
			IDataConverterCommandContext context,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncBlockSerializationStrategy asyncBlockSerializationStrategy =
				context.SerializationStrategy as IAsyncBlockSerializationStrategy;

			if (asyncBlockSerializationStrategy == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} IS NOT AN ASYNC BLOCK SERIALIZATION STRATEGY");

				return (false, default);
			}

			return await asyncBlockSerializationStrategy.ReadBlockAsync(
				valueType,
				blockOffset,
				blockSize,
				asyncContext);
		}

		#endregion

		#region Write

		public async Task<bool> ConvertAndWriteBlockAsync<TValue>(
			IDataConverterCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncBlockSerializationStrategy asyncBlockSerializationStrategy =
				context.SerializationStrategy as IAsyncBlockSerializationStrategy;

			if (asyncBlockSerializationStrategy == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} IS NOT AN ASYNC BLOCK SERIALIZATION STRATEGY");

				return false;
			}

			return await asyncBlockSerializationStrategy.WriteBlockAsync<TValue>(
				value,
				blockOffset,
				blockSize,
				asyncContext);
		}

		public async Task<bool> ConvertAndWriteBlockAsync(
			Type valueType,
			IDataConverterCommandContext context,
			object value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncBlockSerializationStrategy asyncBlockSerializationStrategy =
							context.SerializationStrategy as IAsyncBlockSerializationStrategy;

			if (asyncBlockSerializationStrategy == null)
			{
				logger?.LogError(
					GetType(),
					$"SERIALIZATION STREATEGY {context.SerializationStrategy.GetType().Name} IS NOT AN ASYNC BLOCK SERIALIZATION STRATEGY");

				return false;
			}

			return await asyncBlockSerializationStrategy.WriteBlockAsync(
				valueType,
				value,
				blockOffset,
				blockSize,
				asyncContext);
		}

		#endregion

		#endregion
	}
}