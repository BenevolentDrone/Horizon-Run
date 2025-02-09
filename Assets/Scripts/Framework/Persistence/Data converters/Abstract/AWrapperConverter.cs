using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public abstract class AWrapperConverter
		: IDataConverter,
		  IBlockDataConverter,
		  IAsyncDataConverter,
		  IAsyncBlockDataConverter
	{
		protected readonly IDataConverter innerDataConverter;

		protected readonly ILogger logger;

		public AWrapperConverter(
			IDataConverter innerDataConverter,
			ILogger logger)
		{
			this.innerDataConverter = innerDataConverter;

			this.logger = logger;
		}

		#region IDataConverter

		#region Read

		public virtual bool ReadAndConvert<TValue>(
			IDataConverterCommandContext context,
			out TValue value)
		{
			return innerDataConverter.ReadAndConvert<TValue>(
				context,
				out value);
		}

		public virtual bool ReadAndConvert(
			Type valueType,
			IDataConverterCommandContext context,
			out object value)
		{
			return innerDataConverter.ReadAndConvert(
				valueType,
				context,
				out value);
		}

		#endregion

		#region Write

		public virtual bool ConvertAndWrite<TValue>(
			IDataConverterCommandContext context,
			TValue value)
		{
			return innerDataConverter.ConvertAndWrite<TValue>(
				context,
				value);
		}

		public virtual bool ConvertAndWrite(
			Type valueType,
			IDataConverterCommandContext context,
			object value)
		{
			return innerDataConverter.ConvertAndWrite(
				valueType,
				context,
				value);
		}

		#endregion

		#region Append

		public virtual bool ConvertAndAppend<TValue>(
			IDataConverterCommandContext context,
			TValue value)
		{
			return innerDataConverter.ConvertAndAppend<TValue>(
				context,
				value);
		}

		public virtual bool ConvertAndAppend(
			Type valueType,
			IDataConverterCommandContext context,
			object value)
		{
			return innerDataConverter.ConvertAndAppend(
				valueType,
				context,
				value);
		}

		#endregion

		#endregion

		#region IBlockDataConverter

		#region Read

		public virtual bool ReadBlockAndConvert<TValue>(
			IDataConverterCommandContext context,
			int blockOffset,
			int blockSize,
			out TValue value)
		{
			IBlockDataConverter blockDataConverter =
				innerDataConverter as IBlockDataConverter;

			if (blockDataConverter == null)
			{
				logger?.LogError(
					GetType(),
					$"DATA CONVERTER {innerDataConverter.GetType().Name} IS NOT A BLOCK DATA CONVERTER");

				value = default(TValue);

				return false;
			}

			return blockDataConverter.ReadBlockAndConvert<TValue>(
				context,
				blockOffset,
				blockSize,
				out value);
		}

		public virtual bool ReadBlockAndConvert(
			Type valueType,
			IDataConverterCommandContext context,
			int blockOffset,
			int blockSize,
			out object value)
		{
			IBlockDataConverter blockDataConverter =
				innerDataConverter as IBlockDataConverter;

			if (blockDataConverter == null)
			{
				logger?.LogError(
					GetType(),
					$"DATA CONVERTER {innerDataConverter.GetType().Name} IS NOT A BLOCK DATA CONVERTER");

				value = default;

				return false;
			}

			return blockDataConverter.ReadBlockAndConvert(
				valueType,
				context,
				blockOffset,
				blockSize,
				out value);
		}

		#endregion

		#region Write

		public virtual bool ConvertAndWriteBlock<TValue>(
			IDataConverterCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize)
		{
			IBlockDataConverter blockDataConverter =
				innerDataConverter as IBlockDataConverter;

			if (blockDataConverter == null)
			{
				logger?.LogError(
					GetType(),
					$"DATA CONVERTER {innerDataConverter.GetType().Name} IS NOT A BLOCK DATA CONVERTER");

				return false;
			}

			return blockDataConverter.ConvertAndWriteBlock<TValue>(
				context,
				value,
				blockOffset,
				blockSize);
		}

		public virtual bool ConvertAndWriteBlock(
			Type valueType,
			IDataConverterCommandContext context,
			object value,
			int blockOffset,
			int blockSize)
		{
			IBlockDataConverter blockDataConverter =
				innerDataConverter as IBlockDataConverter;

			if (blockDataConverter == null)
			{
				logger?.LogError(
					GetType(),
					$"DATA CONVERTER {innerDataConverter.GetType().Name} IS NOT A BLOCK DATA CONVERTER");

				return false;
			}

			return blockDataConverter.ConvertAndWriteBlock(
				valueType,
				context,
				value,
				blockOffset,
				blockSize);
		}

		#endregion

		#endregion

		#region IAsyncDataConverter

		#region Read

		public virtual async Task<(bool, TValue)> ReadAsyncAndConvert<TValue>(
			IDataConverterCommandContext context,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncDataConverter asyncDataConverter =
				innerDataConverter as IAsyncDataConverter;

			if (asyncDataConverter == null)
			{
				logger?.LogError(
					GetType(),
					$"DATA CONVERTER {innerDataConverter.GetType().Name} IS NOT AN ASYNC DATA CONVERTER");

				return (false, default(TValue));
			}

			return await asyncDataConverter.ReadAsyncAndConvert<TValue>(
				context,
				asyncContext);
		}

		public virtual async Task<(bool, object)> ReadAsyncAndConvert(
			Type valueType,
			IDataConverterCommandContext context,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncDataConverter asyncDataConverter =
				innerDataConverter as IAsyncDataConverter;

			if (asyncDataConverter == null)
			{
				logger?.LogError(
					GetType(),
					$"DATA CONVERTER {innerDataConverter.GetType().Name} IS NOT AN ASYNC DATA CONVERTER");

				return (false, default);
			}

			return await asyncDataConverter.ReadAsyncAndConvert(
				valueType,
				context,
				asyncContext);
		}

		#endregion

		#region Write

		public virtual async Task<bool> ConvertAndWriteAsync<TValue>(
			IDataConverterCommandContext context,
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncDataConverter asyncDataConverter =
				innerDataConverter as IAsyncDataConverter;

			if (asyncDataConverter == null)
			{
				logger?.LogError(
					GetType(),
					$"DATA CONVERTER {innerDataConverter.GetType().Name} IS NOT AN ASYNC DATA CONVERTER");

				return false;
			}

			return await asyncDataConverter.ConvertAndWriteAsync<TValue>(
				context,
				value,
				asyncContext);
		}

		public virtual async Task<bool> ConvertAndWriteAsync(
			Type valueType,
			IDataConverterCommandContext context,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncDataConverter asyncDataConverter =
				innerDataConverter as IAsyncDataConverter;

			if (asyncDataConverter == null)
			{
				logger?.LogError(
					GetType(),
					$"DATA CONVERTER {innerDataConverter.GetType().Name} IS NOT AN ASYNC DATA CONVERTER");

				return false;
			}

			return await asyncDataConverter.ConvertAndWriteAsync(
				valueType,
				context,
				value,
				asyncContext);
		}

		#endregion

		#region Append

		public virtual async Task<bool> ConvertAndAppendAsync<TValue>(
			IDataConverterCommandContext context,
			TValue value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncDataConverter asyncDataConverter =
				innerDataConverter as IAsyncDataConverter;

			if (asyncDataConverter == null)
			{
				logger?.LogError(
					GetType(),
					$"DATA CONVERTER {innerDataConverter.GetType().Name} IS NOT AN ASYNC DATA CONVERTER");

				return false;
			}

			return await asyncDataConverter.ConvertAndAppendAsync<TValue>(
				context,
				value,
				asyncContext);
		}

		public virtual async Task<bool> ConvertAndAppendAsync(
			Type valueType,
			IDataConverterCommandContext context,
			object value,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncDataConverter asyncDataConverter =
				innerDataConverter as IAsyncDataConverter;

			if (asyncDataConverter == null)
			{
				logger?.LogError(
					GetType(),
					$"DATA CONVERTER {innerDataConverter.GetType().Name} IS NOT AN ASYNC DATA CONVERTER");

				return false;
			}

			return await asyncDataConverter.ConvertAndAppendAsync(
				valueType,
				context,
				value,
				asyncContext);
		}

		#endregion

		#endregion

		#region IAsyncBlockDataConverter

		#region Read

		public virtual async Task<(bool, TValue)> ReadBlockAsyncAndConvert<TValue>(
			IDataConverterCommandContext context,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncBlockDataConverter asyncBlockDataConverter =
				innerDataConverter as IAsyncBlockDataConverter;

			if (asyncBlockDataConverter == null)
			{
				logger?.LogError(
					GetType(),
					$"DATA CONVERTER {innerDataConverter.GetType().Name} IS NOT AN ASYNC BLOCK DATA CONVERTER");

				return (false, default(TValue));
			}

			return await asyncBlockDataConverter.ReadBlockAsyncAndConvert<TValue>(
				context,
				blockOffset,
				blockSize,
				asyncContext);
		}

		public virtual async Task<(bool, object)> ReadBlockAsyncAndConvert(
			Type valueType,
			IDataConverterCommandContext context,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncBlockDataConverter asyncBlockDataConverter =
				innerDataConverter as IAsyncBlockDataConverter;

			if (asyncBlockDataConverter == null)
			{
				logger?.LogError(
					GetType(),
					$"DATA CONVERTER {innerDataConverter.GetType().Name} IS NOT AN ASYNC BLOCK DATA CONVERTER");

				return (false, default);
			}

			return await asyncBlockDataConverter.ReadBlockAsyncAndConvert(
				valueType,
				context,
				blockOffset,
				blockSize,
				asyncContext);
		}

		#endregion

		#region Write

		public virtual async Task<bool> ConvertAndWriteBlockAsync<TValue>(
			IDataConverterCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncBlockDataConverter asyncBlockDataConverter =
				innerDataConverter as IAsyncBlockDataConverter;

			if (asyncBlockDataConverter == null)
			{
				logger?.LogError(
					GetType(),
					$"DATA CONVERTER {innerDataConverter.GetType().Name} IS NOT AN ASYNC BLOCK DATA CONVERTER");

				return false;
			}

			return await asyncBlockDataConverter.ConvertAndWriteBlockAsync<TValue>(
				context,
				value,
				blockOffset,
				blockSize,
				asyncContext);
		}

		public virtual async Task<bool> ConvertAndWriteBlockAsync(
			Type valueType,
			IDataConverterCommandContext context,
			object value,
			int blockOffset,
			int blockSize,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IAsyncBlockDataConverter asyncBlockDataConverter =
				innerDataConverter as IAsyncBlockDataConverter;

			if (asyncBlockDataConverter == null)
			{
				logger?.LogError(
					GetType(),
					$"DATA CONVERTER {innerDataConverter.GetType().Name} IS NOT AN ASYNC BLOCK DATA CONVERTER");

				return false;
			}

			return await asyncBlockDataConverter.ConvertAndWriteBlockAsync(
				valueType,
				context,
				value,
				blockOffset,
				blockSize,
				asyncContext);
		}

		#endregion

		#endregion
	}
}