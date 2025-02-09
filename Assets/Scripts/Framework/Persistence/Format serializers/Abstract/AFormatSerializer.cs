using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public abstract class AFormatSerializer
		: IFormatSerializer
	{
		protected readonly ILogger logger;

		public AFormatSerializer(
			ILogger logger)
		{
			this.logger = logger;
		}

		#region IFormatSerializer

		public abstract bool Serialize<TValue>(
			ISerializationCommandContext context,
			TValue value);

		public abstract bool Serialize(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject);

		public abstract bool Deserialize<TValue>(
			ISerializationCommandContext context,
			out TValue value);

		public abstract bool Deserialize(
			Type valueType,
			ISerializationCommandContext context,
			out object valueObject);

		public abstract bool Populate<TValue>(
			ISerializationCommandContext context,
			ref TValue value);

		public abstract bool Populate(
			Type valueType,
			ISerializationCommandContext context,
			ref object valueObject);

		#endregion

		protected void EnsureStrategyInitializedForRead(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments)
		{
			IStrategyWithStream strategyWithStream = strategy as IStrategyWithStream;

			IHasReadWriteControl strategyWithState = strategy as IHasReadWriteControl;


			if (strategyWithState != null)
			{
				//If it's a stream strategy and the stream is not open, then open the write stream
				if (strategyWithStream != null && !strategyWithStream.StreamOpen)
				{
					if (arguments.Has<IReadAndWriteAccessArgument>())
					{
						if (!strategyWithState.SupportsSimultaneousReadAndWrite)
						{
							throw new InvalidOperationException(
								$"THE STRATEGY {strategy.GetType().Name} DOES NOT SUPPORT SIMULTANEOUS READ AND WRITE");
						}

						strategyWithState.InitializeReadAndWrite();
					}
					else
					{
						strategyWithState.InitializeRead();
					}
				}
			}
		}

		protected void EnsureStrategyFinalizedForRead(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments)
		{
			IStrategyWithStream strategyWithStream = strategy as IStrategyWithStream;

			IHasReadWriteControl strategyWithState = strategy as IHasReadWriteControl;


			if (strategyWithState != null)
			{
				if (strategyWithStream != null && strategyWithStream.StreamOpen)
				{
					if (strategyWithStream.CurrentMode == EStreamMode.READ_AND_WRITE)
					{
						strategyWithState.FinalizeReadAndWrite();
					}
					else if (strategyWithStream.CurrentMode == EStreamMode.READ)
					{
						strategyWithState.FinalizeRead();
					}
				}
			}
		}

		protected void EnsureStrategyInitializedForWriteOrAppend(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments)
		{
			if (arguments.Has<IAppendArgument>())
			{
				EnsureStrategyInitializedForAppend(
					strategy,
					arguments);
			}
			else
			{
				EnsureStrategyInitializedForWrite(
					strategy,
					arguments);
			}
		}

		protected void EnsureStrategyInitializedForAppend(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments)
		{
			IHasIODestination strategyWithIODestination = strategy as IHasIODestination;

			IStrategyWithStream strategyWithStream = strategy as IStrategyWithStream;

			IHasReadWriteControl strategyWithState = strategy as IHasReadWriteControl;

			if (strategyWithStream == null || !strategyWithStream.StreamOpen)
			{
				//Ensure that if there is no folder, then create it
				strategyWithIODestination.EnsureIODestinationExists();
			}


			if (strategyWithState != null)
			{
				//If it's a stream strategy and the stream is not open, then open the write stream
				if (strategyWithStream != null && !strategyWithStream.StreamOpen)
				{
					if (arguments.Has<IReadAndWriteAccessArgument>())
					{
						if (!strategyWithState.SupportsSimultaneousReadAndWrite)
						{
							throw new InvalidOperationException(
								$"THE STRATEGY {strategy.GetType().Name} DOES NOT SUPPORT SIMULTANEOUS READ AND WRITE");
						}

						strategyWithState.InitializeReadAndWrite();
					}
					else
					{
						strategyWithState.InitializeAppend();
					}
				}
			}
		}

		protected void EnsureStrategyInitializedForWrite(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments)
		{
			IHasIODestination strategyWithIODestination = strategy as IHasIODestination;

			IStrategyWithStream strategyWithStream = strategy as IStrategyWithStream;

			IHasReadWriteControl strategyWithState = strategy as IHasReadWriteControl;


			//If the strategy writes to a file, then
			if (strategyWithIODestination != null)
			{
				if (strategyWithStream == null || !strategyWithStream.StreamOpen)
				{
					//Ensure the file with the same path does not exist
					if (strategyWithIODestination.IODestinationExists())
					{
						strategyWithIODestination.EraseIODestination();
					}

					//Ensure that if there is no folder, then create it
					strategyWithIODestination.EnsureIODestinationExists();
				}
			}

			if (strategyWithState != null)
			{
				//If it's a stream strategy and the stream is not open, then open the write stream
				if (strategyWithStream != null && !strategyWithStream.StreamOpen)
				{
					if (arguments.Has<IReadAndWriteAccessArgument>())
					{
						if (!strategyWithState.SupportsSimultaneousReadAndWrite)
						{
							throw new InvalidOperationException(
								$"THE STRATEGY {strategy.GetType().Name} DOES NOT SUPPORT SIMULTANEOUS READ AND WRITE");
						}

						strategyWithState.InitializeReadAndWrite();
					}
					else
					{
						strategyWithState.InitializeWrite();
					}
				}
			}
		}

		protected void EnsureStrategyFinalizedForWriteOrAppend(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments)
		{
			if (arguments.Has<IAppendArgument>())
			{
				EnsureStrategyFinalizedForAppend(
					strategy,
					arguments);
			}
			else
			{
				EnsureStrategyFinalizedForWrite(
					strategy,
					arguments);
			}
		}

		protected void EnsureStrategyFinalizedForAppend(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments)
		{
			IStrategyWithStream strategyWithStream = strategy as IStrategyWithStream;

			IHasReadWriteControl strategyWithState = strategy as IHasReadWriteControl;


			if (strategyWithState != null)
			{
				if (strategyWithStream != null && strategyWithStream.StreamOpen)
				{
					if (strategyWithStream.CurrentMode == EStreamMode.READ_AND_WRITE)
					{
						strategyWithState.FinalizeReadAndWrite();
					}
					else if (strategyWithStream.CurrentMode == EStreamMode.APPEND)
					{
						strategyWithState.FinalizeAppend();
					}
				}
			}
		}

		protected void EnsureStrategyFinalizedForWrite(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments)
		{
			IHasIODestination strategyWithIODestination = strategy as IHasIODestination;

			IStrategyWithStream strategyWithStream = strategy as IStrategyWithStream;

			IHasReadWriteControl strategyWithState = strategy as IHasReadWriteControl;


			if (strategyWithState != null)
			{
				if (strategyWithStream != null && strategyWithStream.StreamOpen)
				{
					if (strategyWithStream.CurrentMode == EStreamMode.READ_AND_WRITE)
					{
						strategyWithState.FinalizeReadAndWrite();
					}
					else if (strategyWithStream.CurrentMode == EStreamMode.WRITE)
					{
						strategyWithState.FinalizeWrite();
					}
				}
			}
		}

		protected bool TryReadPersistently<TValue>(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments,
			Func<byte[], TValue> convertFromBytesDelegate,
			out TValue value)
		{
			IStrategyWithFilter strategyWithFilter = strategy as IStrategyWithFilter;

			if (strategyWithFilter == null
				|| strategyWithFilter.AllowsType<TValue>())
			{
				return TryRead<TValue>(
					strategy,
					arguments,
					out value);
			}

			if (strategyWithFilter.AllowsType<byte[]>()) //almost anything can be converted to a byte array
			{
				value = default;

				var result = TryRead<byte[]>(
					strategy,
					arguments,
					out var byteArray);

				if (result)
				{
					value = convertFromBytesDelegate.Invoke(byteArray);
				}

				return result;
			}

			value = default;

			return false;
		}

		protected bool TryReadPersistently(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments,
			Func<byte[], object> convertFromBytesDelegate,
			Type valueType,
			out object value)
		{
			IStrategyWithFilter strategyWithFilter = strategy as IStrategyWithFilter;

			if (strategyWithFilter == null
				|| strategyWithFilter.AllowsType(valueType))
			{
				return TryRead(
					strategy,
					arguments,
					valueType,
					out value);
			}

			if (strategyWithFilter.AllowsType<byte[]>()) //almost anything can be converted to a byte array
			{
				value = default;

				var result = TryRead<byte[]>(
					strategy,
					arguments,
					out var byteArray);

				if (result)
				{
					value = convertFromBytesDelegate.Invoke(byteArray);
				}

				return result;
			}

			value = default;

			return false;
		}

		protected bool TryRead<TValue>(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments,
			out TValue value)
		{
			if (arguments.Has<IBlockSerializationArgument>())
			{
				var blockArgument = arguments.Get<IBlockSerializationArgument>();

				IBlockSerializationStrategy blockSerializationStrategy = strategy as IBlockSerializationStrategy;

				return blockSerializationStrategy.ReadBlock<TValue>(
					0,
					blockArgument.BlockSize,
					out value);
			}

			return strategy.Read<TValue>(
				out value);
		}

		protected bool TryRead(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments,
			Type valueType,
			out object value)
		{
			if (arguments.Has<IBlockSerializationArgument>())
			{
				var blockArgument = arguments.Get<IBlockSerializationArgument>();

				IBlockSerializationStrategy blockSerializationStrategy = strategy as IBlockSerializationStrategy;

				return blockSerializationStrategy.ReadBlock(
					valueType,
					0,
					blockArgument.BlockSize,
					out value);
			}

			return strategy.Read(
				valueType,
				out value);
		}

		protected bool TryWriteOrAppendPersistently<TValue>(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments,
			Func<TValue, byte[]> convertToBytesDelegate,
			TValue value)
		{
			IStrategyWithFilter strategyWithFilter = strategy as IStrategyWithFilter;

			if (strategyWithFilter == null
				|| strategyWithFilter.AllowsType<TValue>())
			{
				return TryWriteOrAppend<TValue>(
					strategy,
					arguments,
					value);
			}

			if (strategyWithFilter.AllowsType<byte[]>()) //almost anything can be converted to a byte array
			{
				return TryWriteOrAppend<byte[]>(
					strategy,
					arguments,
					convertToBytesDelegate?.Invoke(value));
			}

			return false;
		}

		protected bool TryWriteOrAppendPersistently(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments,
			Func<object, byte[]> convertToBytesDelegate,
			Type valueType,
			object valueObject)
		{
			IStrategyWithFilter strategyWithFilter = strategy as IStrategyWithFilter;

			if (strategyWithFilter == null
				|| strategyWithFilter.AllowsType(valueType))
			{
				return TryWriteOrAppend(
					strategy,
					arguments,
					valueType,
					valueObject);
			}

			if (strategyWithFilter.AllowsType<byte[]>()) //almost anything can be converted to a byte array
			{
				return TryWriteOrAppend<byte[]>(
					strategy,
					arguments,
					convertToBytesDelegate?.Invoke(valueObject));
			}

			return false;
		}

		protected bool TryWriteOrAppend<TValue>(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments,
			TValue value)
		{
			if (arguments.Has<IAppendArgument>())
			{
				return strategy.Append<TValue>(
					value);
			}

			if (arguments.Has<IBlockSerializationArgument>())
			{
				var blockArgument = arguments.Get<IBlockSerializationArgument>();

				IBlockSerializationStrategy blockSerializationStrategy = strategy as IBlockSerializationStrategy;

				return blockSerializationStrategy.WriteBlock<TValue>(
					value,
					0,
					blockArgument.BlockSize);
			}

			return strategy.Write<TValue>(
				value);
		}

		protected bool TryWriteOrAppend(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments,
			Type valueType,
			object valueObject)
		{
			if (arguments.Has<IAppendArgument>())
			{
				return strategy.Append(
					valueType,
					valueObject);
			}

			if (arguments.Has<IBlockSerializationArgument>())
			{
				var blockArgument = arguments.Get<IBlockSerializationArgument>();

				IBlockSerializationStrategy blockSerializationStrategy = strategy as IBlockSerializationStrategy;

				return blockSerializationStrategy.WriteBlock(
					valueType,
					valueObject,
					0,
					blockArgument.BlockSize);
			}

			return strategy.Write(
				valueType,
				valueObject);
		}
	}
}