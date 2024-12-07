using System;

using HereticalSolutions.Repositories;

using HereticalSolutions.Metadata;

namespace HereticalSolutions.Persistence
{
	public interface IFormatSerializer
	{
		#region Serialize

		bool Serialize<TValue>(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments,
			TValue value);

		bool Serialize(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments,
			Type valueType,
			object valueObject);

		#endregion

		#region Deserialize

		bool Deserialize<TValue>(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments,
			out TValue value);

		bool Deserialize(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments,
			Type valueType,
			out object valueObject);

		#endregion

		#region Populate

		bool Populate<TValue>(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments,
			ref TValue value);

		bool Populate(
			ISerializationStrategy strategy,
			IStronglyTypedMetadata arguments,
			Type valueType,
			ref object valueObject);

		#endregion
	}
}