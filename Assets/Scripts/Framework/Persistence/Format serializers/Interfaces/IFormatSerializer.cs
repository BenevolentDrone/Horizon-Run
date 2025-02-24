using System;

namespace HereticalSolutions.Persistence
{
	public interface IFormatSerializer
	{
		#region Serialize

		bool Serialize<TValue>(
			ISerializationCommandContext context,
			TValue value);

		bool Serialize(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject);

		#endregion

		#region Deserialize

		bool Deserialize<TValue>(
			ISerializationCommandContext context,
			out TValue value);

		bool Deserialize(
			Type valueType,
			ISerializationCommandContext context,
			out object valueObject);

		#endregion

		#region Populate

		bool Populate<TValue>(
			ISerializationCommandContext context,
			TValue value);

		bool Populate(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject);

		#endregion
	}
}