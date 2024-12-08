using System;

using HereticalSolutions.Metadata;

namespace HereticalSolutions.Persistence
{
	public static class PersistenceHelpers
	{
		public static void EnsureStrategyInitializedForRead(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments)
		{
			IStrategyWithStream strategyWithStream = strategy as IStrategyWithStream;

			IStrategyWithState strategyWithState = strategy as IStrategyWithState;


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

		public static void EnsureStrategyFinalizedForRead(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments)
		{
			IStrategyWithStream strategyWithStream = strategy as IStrategyWithStream;

			IStrategyWithState strategyWithState = strategy as IStrategyWithState;


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

		public static void EnsureStrategyInitializedForWriteOrAppend(
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

		public static void EnsureStrategyInitializedForAppend(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments)
		{
			IStrategyWithIODestination strategyWithIODestination = strategy as IStrategyWithIODestination;

			IStrategyWithStream strategyWithStream = strategy as IStrategyWithStream;

			IStrategyWithState strategyWithState = strategy as IStrategyWithState;

			if (strategyWithStream == null || !strategyWithStream.StreamOpen)
			{
				//Ensure that if there is no folder, then create it
				strategyWithIODestination.EnsureIOTargetDestinationExists();
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

		public static void EnsureStrategyInitializedForWrite(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments)
		{
			IStrategyWithIODestination strategyWithIODestination = strategy as IStrategyWithIODestination;

			IStrategyWithStream strategyWithStream = strategy as IStrategyWithStream;

			IStrategyWithState strategyWithState = strategy as IStrategyWithState;


			//If the strategy writes to a file, then
			if (strategyWithIODestination != null)
			{
				if (strategyWithStream == null || !strategyWithStream.StreamOpen)
				{
					//Ensure the file with the same path does not exist
					if (strategyWithIODestination.IOTargetExists())
					{
						strategyWithIODestination.EraseIOTarget();
					}

					//Ensure that if there is no folder, then create it
					strategyWithIODestination.EnsureIOTargetDestinationExists();
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

		public static void EnsureStrategyFinalizedForWriteOrAppend(
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

		public static void EnsureStrategyFinalizedForAppend(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments)
		{
			IStrategyWithStream strategyWithStream = strategy as IStrategyWithStream;

			IStrategyWithState strategyWithState = strategy as IStrategyWithState;


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

		public static void EnsureStrategyFinalizedForWrite(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments)
		{
			IStrategyWithIODestination strategyWithIODestination = strategy as IStrategyWithIODestination;

			IStrategyWithStream strategyWithStream = strategy as IStrategyWithStream;

			IStrategyWithState strategyWithState = strategy as IStrategyWithState;


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

		public static bool TryReadPersistently<TValue>(
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

		public static bool TryReadPersistently(
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

		public static bool TryRead<TValue>(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments,
			out TValue value)
		{
			if (arguments.Has<IBlockSerializationArgument>())
			{
				var blockArgument = arguments.Get<IBlockSerializationArgument>();

				IBlockSerializationStrategy blockSerializationStrategy = strategy as IBlockSerializationStrategy;

				return blockSerializationStrategy.BlockRead<TValue>(
					0,
					blockArgument.BlockSize,
					out value);
			}
			
			return strategy.Read<TValue>(
				out value);
		}

		public static bool TryRead(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments,
			Type valueType,
			out object value)
		{
			if (arguments.Has<IBlockSerializationArgument>())
			{
				var blockArgument = arguments.Get<IBlockSerializationArgument>();

				IBlockSerializationStrategy blockSerializationStrategy = strategy as IBlockSerializationStrategy;

				return blockSerializationStrategy.BlockRead(
					valueType,
					0,
					blockArgument.BlockSize,
					out value);
			}

			return strategy.Read(
				valueType,
				out value);
		}

		public static bool TryWriteOrAppendPersistently<TValue>(
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

		public static bool TryWriteOrAppendPersistently(
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

		public static bool TryWriteOrAppend<TValue>(
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

				return blockSerializationStrategy.BlockWrite<TValue>(
					value,
					0,
					blockArgument.BlockSize);
			}

			return strategy.Write<TValue>(
				value);
		}

		public static bool TryWriteOrAppend(
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

				return blockSerializationStrategy.BlockWrite(
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