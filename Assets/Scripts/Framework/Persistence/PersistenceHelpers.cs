using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public static class PersistenceHelpers
	{
		public static void EnsureStrategyInitializedForRead(
			ISerializationStrategy strategy,
			IReadOnlyObjectRepository arguments)
		{
			IStrategyWithStream strategyWithStream = strategy as IStrategyWithStream;

			IStrategyWithState strategyWithState = strategy as IStrategyWithState;


			if (strategyWithState != null)
			{
				//If it's a stream strategy and the stream is not open, then open the write stream
				if (strategyWithStream != null && !strategyWithStream.StreamOpen)
				{
					strategyWithState.InitializeRead();
				}
			}

			return true;
		}

		public static void EnsureStrategyFinalizedForRead(
			ISerializationStrategy strategy,
			IReadOnlyObjectRepository arguments)
		{
			IStrategyWithStream strategyWithStream = strategy as IStrategyWithStream;

			IStrategyWithState strategyWithState = strategy as IStrategyWithState;


			if (strategyWithState != null)
			{
				if (strategyWithStream != null && strategyWithStream.StreamOpen)
				{
					strategyWithState.FinalizeRead();
				}
			}

			return true;
		}

		public static void EnsureStrategyInitializedForWriteOrAppend(
			ISerializationStrategy strategy,
			IReadOnlyObjectRepository arguments)
		{
			if (arguments.Get<ISerializationArgument>().Append)
			{
				EnsureStrategyInitializedForAppend(
					strategy);
			}
			else
			{
				EnsureStrategyInitializedForWrite(
					strategy);
			}
		}

		public static void EnsureStrategyInitializedForAppend(
			ISerializationStrategy strategy,
			IReadOnlyObjectRepository arguments)
		{
			IStrategyWithStream strategyWithStream = strategy as IStrategyWithStream;

			IStrategyWithState strategyWithState = strategy as IStrategyWithState;


			if (strategyWithState != null)
			{
				//If it's a stream strategy and the stream is not open, then open the write stream
				if (strategyWithStream != null && !strategyWithStream.StreamOpen)
				{
					strategyWithState.InitializeAppend();
				}
			}
		}

		public static void EnsureStrategyInitializedForWrite(
			ISerializationStrategy strategy,
			IReadOnlyObjectRepository arguments)
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
				}
			}

			if (strategyWithState != null)
			{
				//If it's a stream strategy and the stream is not open, then open the write stream
				if (strategyWithStream != null && !strategyWithStream.StreamOpen)
				{
					strategyWithState.InitializeWrite();
				}
			}
		}

		public static void EnsureStrategyFinalizedForWriteOrAppend(
			ISerializationStrategy strategy,
			IReadOnlyObjectRepository arguments)
		{
			if (arguments.Get<ISerializationArgument>().Append)
			{
				EnsureStrategyFinalizedForAppend(
					strategy);
			}
			else
			{
				EnsureStrategyFinalizedForWrite(
					strategy);
			}
		}

		public static void EnsureStrategyFinalizedForAppend(
			ISerializationStrategy strategy,
			IReadOnlyObjectRepository arguments)
		{
			IStrategyWithStream strategyWithStream = strategy as IStrategyWithStream;

			IStrategyWithState strategyWithState = strategy as IStrategyWithState;


			if (strategyWithState != null)
			{
				if (strategyWithStream != null && strategyWithStream.StreamOpen)
				{
					strategyWithState.FinalizeAppend();
				}
			}
		}

		public static void EnsureStrategyFinalizedForWrite(
			ISerializationStrategy strategy,
			IReadOnlyObjectRepository arguments)
		{
			IStrategyWithIODestination strategyWithIODestination = strategy as IStrategyWithIODestination;

			IStrategyWithStream strategyWithStream = strategy as IStrategyWithStream;

			IStrategyWithState strategyWithState = strategy as IStrategyWithState;


			if (strategyWithState != null)
			{
				if (strategyWithStream != null && strategyWithStream.StreamOpen)
				{
					strategyWithState.FinalizeWrite();
				}
			}
		}

		public static bool TryReadPersistently<TValue>(
			ISerializationStrategy strategy,
			IReadOnlyObjectRepository arguments,
			Func<byte[], TValue> convertFromBytesDelegate,
			out TValue value)
		{
			if (strategy.AllowedValueTypes.Contains(typeof(TValue))
				|| strategy.AllowedValueTypes.Contains(typeof(object))) //object is like a fallback. if the strategy allows object, then it can be cast to anything
			{
				return TryRead<TValue>(
					strategy,
					arguments,
					out value);
			}

			if (strategy.AllowedValueTypes.Contains(typeof(byte[]))) //almost anything can be converted to a byte array
			{
				var result = TryRead<byte[]>(
					strategy,
					arguments,
					out var byteArray);

				if (result)
				{
					value = convertFromBytesDelegate?.Invoke(byteArray);
				}
				
				return result;
			}

			value = default;

			return false;
		}

		public static bool TryRead<TValue>(
			ISerializationStrategy strategy,
			IReadOnlyObjectRepository arguments,
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

		public static bool TryWriteOrAppendPersistently<TValue>(
			ISerializationStrategy strategy,
			IReadOnlyObjectRepository arguments,
			Func<TValue, byte[]> convertToBytesDelegate,
			TValue value)
		{
			if (strategy.AllowedValueTypes.Contains(typeof(TValue))
				|| strategy.AllowedValueTypes.Contains(typeof(object))) //object is like a fallback. if the strategy allows object, then it can be cast to anything
			{
				return TryWriteOrAppend<TValue>(
					strategy,
					arguments,
					value);
			}

			if (strategy.AllowedValueTypes.Contains(typeof(byte[]))) //almost anything can be converted to a byte array
			{
				return TryWriteOrAppend<byte[]>(
					strategy,
					arguments,
					convertToBytesDelegate?.Invoke(value));
			}

			return false;
		}

		public static bool TryWriteOrAppend<TValue>(
			ISerializationStrategy strategy,
			IReadOnlyObjectRepository arguments,
			TValue value)
		{
			if (arguments.Get<ISerializationArgument>().Append)
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
	}
}