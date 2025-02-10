using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	public abstract class ABinarySerializer
		: AFormatSerializer,
		  IBlockFormatSerializer,
		  IAsyncFormatSerializer,
		  IAsyncBlockFormatSerializer
	{
		public ABinarySerializer(
			ILogger logger)
			: base(
				logger)
		{
		}

		#region IFormatSerializer

		public override bool Serialize<TValue>(
			ISerializationCommandContext context,
			TValue value)
		{
			EnsureStrategyInitializedForSerialization(
				context);

			if (CanSerializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				SerializeWithStream<TValue>(
					strategyWithStream,
					value);

				EnsureStrategyFinalizedForSerialization(
					context);

				return true;
			}

			byte[] byteArrayValue = SerializeToByteArray<TValue>(
				value);

			var result = TrySerialize<byte[]>(
				context,
				byteArrayValue);

			EnsureStrategyFinalizedForSerialization(
				context);

			return result;
		}

		public override bool Serialize(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject)
		{
			EnsureStrategyInitializedForSerialization(
				context);

			if (CanSerializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				SerializeWithStream(
					strategyWithStream,
					valueType,
					valueObject);

				EnsureStrategyFinalizedForSerialization(
					context);

				return true;
			}

			byte[] byteArrayValue = SerializeToByteArray(
				valueType,
				valueObject);

			var result = TrySerialize<byte[]>(
				context,
				byteArrayValue);

			EnsureStrategyFinalizedForSerialization(
				context);

			return result;
		}

		public override bool Deserialize<TValue>(
			ISerializationCommandContext context,
			out TValue value)
		{
			EnsureStrategyInitializedForDeserialization(
				context);

			bool result = false;

			if (CanSerializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = DeserializeWithStream<TValue>(
					strategyWithStream,
					out value);

				EnsureStrategyFinalizedForDeserialization(
					context);

				return result;
			}

			if (!TryDeserialize<byte[]>(
				context,
				out byte[] byteArrayValue))
			{
				value = default(TValue);

				EnsureStrategyFinalizedForDeserialization(
					context);

				return false;
			}

			result = DeserializeFromByteArray<TValue>(
				byteArrayValue,
				out value);

			EnsureStrategyFinalizedForDeserialization(
				context);

			return result;
		}

		public override bool Deserialize(
			Type valueType,
			ISerializationCommandContext context,
			out object valueObject)
		{
			EnsureStrategyInitializedForDeserialization(
				context);

			bool result = false;

			if (CanSerializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = DeserializeWithStream(
					strategyWithStream,
					valueType,
					out valueObject);

				EnsureStrategyFinalizedForDeserialization(
					context);

				return result;
			}

			if (!TryDeserialize<byte[]>(
				context,
				out byte[] byteArrayValue))
			{
				valueObject = default(object);

				EnsureStrategyFinalizedForDeserialization(
					context);

				return false;
			}

			result = DeserializeFromByteArray(
				byteArrayValue,
				valueType,
				out valueObject);

			EnsureStrategyFinalizedForDeserialization(
				context);

			return result;
		}

		public override bool Populate<TValue>(
			ISerializationCommandContext context,
			ref TValue value)
		{
			EnsureStrategyInitializedForDeserialization(
				context);

			bool result = false;

			if (CanSerializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = DeserializeWithStream<TValue>(
					strategyWithStream,
					out var newValue1);

				if (result)
				{
					value = newValue1;
				}

				EnsureStrategyFinalizedForDeserialization(
					context);

				return result;
			}

			if (!TryDeserialize<byte[]>(
				context,
				out byte[] byteArrayValue))
			{
				EnsureStrategyFinalizedForDeserialization(
					context);

				return false;
			}

			result = DeserializeFromByteArray<TValue>(
				byteArrayValue,
				out var newValue2);

			if (result)
			{
				value = newValue2;
			}

			EnsureStrategyFinalizedForDeserialization(
				context);

			return result;
		}

		public override bool Populate(
			Type valueType,
			ISerializationCommandContext context,
			ref object valueObject)
		{
			EnsureStrategyInitializedForDeserialization(
				context);

			bool result = false;

			if (CanSerializeWithStream
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is IStrategyWithStream strategyWithStream)
			{
				result = DeserializeWithStream(
					strategyWithStream,
					valueType,
					out var newValueObject1);

				if (result)
				{
					valueObject = newValueObject1;
				}

				EnsureStrategyFinalizedForDeserialization(
					context);

				return result;
			}

			if (!TryDeserialize<byte[]>(
				context,
				out byte[] byteArrayValue))
			{
				EnsureStrategyFinalizedForDeserialization(
					context);

				return false;
			}

			result = DeserializeFromByteArray(
				byteArrayValue,
				valueType,
				out var newValueObject2);

			if (result)
			{
				valueObject = newValueObject2;
			}

			EnsureStrategyFinalizedForDeserialization(
				context);

			return result;
		}

		#endregion

		#region Serialize / deserialize with text reader / writer

		protected virtual bool CanSerializeWithStream => false;

		protected virtual void SerializeWithStream<TValue>(
			IStrategyWithStream strategyWithStream,
			TValue value)
		{
			throw new NotImplementedException();
		}

		protected virtual void SerializeWithStream(
			IStrategyWithStream strategyWithStream,
			Type valueType,
			object valueObject)
		{
			throw new NotImplementedException();
		}

		protected virtual bool DeserializeWithStream<TValue>(
			IStrategyWithStream strategyWithStream,
			out TValue value)
		{
			throw new NotImplementedException();
		}

		protected virtual bool DeserializeWithStream(
			IStrategyWithStream strategyWithStream,
			Type valueType,
			out object valueObject)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Serialize / deserialize to / from byte array

		protected abstract byte[] SerializeToByteArray<TValue>(
			TValue value);

		protected abstract byte[] SerializeToByteArray(
			Type valueType,
			object valueObject);

		protected abstract bool DeserializeFromByteArray<TValue>(
			byte[] byteArrayValue,
			out TValue value);

		protected abstract bool DeserializeFromByteArray(
			byte[] byteArrayValue,
			Type valueType,
			out object valueObject);

		#endregion
	}
}