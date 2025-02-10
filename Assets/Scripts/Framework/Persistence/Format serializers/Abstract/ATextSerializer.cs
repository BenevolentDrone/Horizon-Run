using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
    public abstract class ATextSerializer
		: AFormatSerializer,
          IBlockFormatSerializer,
          IAsyncFormatSerializer,
          IAsyncBlockFormatSerializer
    {
        public ATextSerializer(
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

            if (CanSerializeWithTextWriter
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                SerializeWithTextWriter<TValue>(
                    textStreamStrategy,
                    value);

                EnsureStrategyFinalizedForSerialization(
                    context);

                return true;
            }
            
            string stringValue = SerializeToString<TValue>(
                value);

            var result = TrySerialize<string>(
                context,
                stringValue);

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

            if (CanSerializeWithTextWriter
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                SerializeWithTextWriter(
                    textStreamStrategy,
                    valueType,
                    valueObject);

                EnsureStrategyFinalizedForSerialization(
                    context);

                return true;
            }

            string stringValue = SerializeToString(
                valueType,
                valueObject);

            var result = TrySerialize<string>(
                context,
                stringValue);

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

            if (CanSerializeWithTextWriter
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = DeserializeWithTextReader<TValue>(
                    textStreamStrategy,
                    out value);

                EnsureStrategyFinalizedForDeserialization(
                    context);

                return result;
            }
            
            if (!TryDeserialize<string>(
                context,
                out string stringValue))
            {
                value = default(TValue);

                EnsureStrategyFinalizedForDeserialization(
                    context);

                return false;
            }

            result = DeserializeFromString<TValue>(
                stringValue,
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

            if (CanSerializeWithTextWriter
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = DeserializeWithTextReader(
                    textStreamStrategy,
                    valueType,
                    out valueObject);

                EnsureStrategyFinalizedForDeserialization(
                    context);

                return result;
            }

            if (!TryDeserialize<string>(
                context,
                out string stringValue))
            {
                valueObject = default(object);

                EnsureStrategyFinalizedForDeserialization(
                    context);

                return false;
            }

            result = DeserializeFromString(
                stringValue,
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

            if (CanSerializeWithTextWriter
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = DeserializeWithTextReader<TValue>(
                    textStreamStrategy,
                    out var newValue1);

                if (result)
                {
                    value = newValue1;
                }

                EnsureStrategyFinalizedForDeserialization(
                    context);

                return result;
            }

            if (!TryDeserialize<string>(
                context,
                out string stringValue))
            {
                EnsureStrategyFinalizedForDeserialization(
                    context);

                return false;
            }

            result = DeserializeFromString<TValue>(
                stringValue,
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

            if (CanSerializeWithTextWriter
				&& !context.Arguments.Has<IDataConversionArgument>()
				&& context.SerializationStrategy is TextStreamStrategy textStreamStrategy)
            {
                result = DeserializeWithTextReader(
                    textStreamStrategy,
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

            if (!TryDeserialize<string>(
                context,
                out string stringValue))
            {
                EnsureStrategyFinalizedForDeserialization(
                    context);

                return false;
            }

            result = DeserializeFromString(
                stringValue,
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

		protected virtual bool CanSerializeWithTextWriter => false;

        protected virtual void SerializeWithTextWriter<TValue>(
            TextStreamStrategy textStreamStrategy,
            TValue value)
		{
			throw new NotImplementedException();
		}

		protected virtual void SerializeWithTextWriter(
            TextStreamStrategy textStreamStrategy,
            Type valueType,
            object valueObject)
		{
			throw new NotImplementedException();
		}

		protected virtual bool DeserializeWithTextReader<TValue>(
            TextStreamStrategy textStreamStrategy,
            out TValue value)
		{
			throw new NotImplementedException();
		}

		protected virtual bool DeserializeWithTextReader(
            TextStreamStrategy textStreamStrategy,
            Type valueType,
            out object valueObject)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Serialize / deserialize to / from string

		protected abstract string SerializeToString<TValue>(
            TValue value);

		protected abstract string SerializeToString(
            Type valueType,
            object valueObject);

		protected abstract bool DeserializeFromString<TValue>(
            string stringValue,
            out TValue value);

		protected abstract bool DeserializeFromString(
            string stringValue,
            Type valueType,
            out object valueObject);

		#endregion
    }
}