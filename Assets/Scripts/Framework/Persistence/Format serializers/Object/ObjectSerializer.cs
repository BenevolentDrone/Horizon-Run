using System;

using HereticalSolutions.Metadata;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	[FormatSerializer]
	public class ObjectSerializer
		: IFormatSerializer
	{
		private readonly ILogger logger;

		public ObjectSerializer(
			ILogger logger)
		{
			this.logger = logger;
		}

		#region IFormatSerializer

		public bool Serialize<TValue>(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments,
			TValue value)
		{
			PersistenceHelpers.EnsureStrategyInitializedForWriteOrAppend(
				strategy,
				arguments);

			return PersistenceHelpers.TryWriteOrAppend<TValue>(
				strategy,
				arguments,
				value);
		}

		public bool Serialize(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments,
			Type valueType,
			object valueObject)
		{
			PersistenceHelpers.EnsureStrategyInitializedForWriteOrAppend(
				strategy,
				arguments);

			return PersistenceHelpers.TryWriteOrAppend(
				strategy,
				arguments,
				valueType,
				valueObject);
		}

		public bool Deserialize<TValue>(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments,
			out TValue value)
		{
			PersistenceHelpers.EnsureStrategyInitializedForRead(
				strategy,
				arguments);

			if (!PersistenceHelpers.TryRead<TValue>(
				strategy,
				arguments,
				out value))
			{
				value = default(TValue);

				return false;
			}

			return true;
		}

		//TODO: fix deserialize as <GENERIC> is NOT working at all - the value does NOT get populated properly
		public bool Deserialize(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments,
			Type valueType,
			out object valueObject)
		{
			PersistenceHelpers.EnsureStrategyInitializedForRead(
				strategy,
				arguments);

			if (!PersistenceHelpers.TryRead(
				strategy,
				arguments,
				valueType,
				out valueObject))
			{
				valueObject = default(object);

				return false;
			}

			return true;
		}

		public bool Populate<TValue>(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments,
			ref TValue value)
		{
			PersistenceHelpers.EnsureStrategyInitializedForRead(
				strategy,
				arguments);

			if (!PersistenceHelpers.TryRead<TValue>(
				strategy,
				arguments,
				out TValue newValue))
			{
				return false;
			}

			value = newValue;

			return true;
		}

		public bool Populate(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments,
			Type valueType,
			ref object valueObject)
		{
			PersistenceHelpers.EnsureStrategyInitializedForRead(
				strategy,
				arguments);

			if (!PersistenceHelpers.TryRead(
				strategy,
				arguments,
				valueType,
				out object newValue))
			{
				return false;
			}

			valueObject = newValue;

			return true;
		}

		#endregion
	}
}