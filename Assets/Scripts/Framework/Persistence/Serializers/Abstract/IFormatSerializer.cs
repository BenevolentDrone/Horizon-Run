using System;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Persistence
{
	public interface IFormatSerializer
	{
		#region Serialize

		bool Serialize<TValue>(
			ISerializationStrategy strategy,
			IReadOnlyObjectRepository arguments,
			TValue value);

		bool Serialize(
			ISerializationStrategy strategy,
			IReadOnlyObjectRepository arguments,
			Type ValueType,
			object valueObject);

		#endregion

		#region Deserialize

		bool Deserialize<TValue>(
			ISerializationStrategy strategy,
			IReadOnlyObjectRepository arguments,
			out TValue value);

		bool Deserialize(
			ISerializationStrategy strategy,
			IReadOnlyObjectRepository arguments,
			Type ValueType,
			out object valueObject);

		#endregion

		#region Populate

		bool Populate<TValue>(
			ISerializationStrategy strategy,
			IReadOnlyObjectRepository arguments,
			ref TValue value);

		bool Populate(
			ISerializationStrategy strategy,
			IReadOnlyObjectRepository arguments,
			Type ValueType,
			ref object valueObject);

		#endregion
	}
}