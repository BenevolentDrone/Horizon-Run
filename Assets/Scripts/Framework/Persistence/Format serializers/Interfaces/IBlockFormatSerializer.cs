using System;

namespace HereticalSolutions.Persistence
{
	public interface IBlockFormatSerializer
	{
		#region Serialize

		bool SerializeBlock<TValue>(
			ISerializationCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize);

		bool SerializeBlock(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject,
			int blockOffset,
			int blockSize);

		#endregion

		#region Deserialize

		bool DeserializeBlock<TValue>(
			ISerializationCommandContext context,
			int blockOffset,
			int blockSize,
			out TValue value);

		bool DeserializeBlock(
			Type valueType,
			ISerializationCommandContext context,
			int blockOffset,
			int blockSize,
			out object valueObject);

		#endregion

		#region Populate

		bool PopulateBlock<TValue>(
			ISerializationCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize);

		bool PopulateBlock(
			Type valueType,
			ISerializationCommandContext context,
			object valueObject,
			int blockOffset,
			int blockSize);

		#endregion
	}
}