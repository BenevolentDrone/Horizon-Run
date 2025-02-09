using System;

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
			ISerializationCommandContext context,
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
			Type valueType,
			ISerializationCommandContext context,
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
			ISerializationCommandContext context,
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
			Type valueType,
			ISerializationCommandContext context,
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
			ISerializationCommandContext context,
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
			Type valueType,
			ISerializationCommandContext context,
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