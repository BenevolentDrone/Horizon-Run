using System;

namespace HereticalSolutions.Persistence
{
	public interface IBlockSerializer
		: IHasIODestination,
		  IHasReadWriteControl
	{
		IReadOnlySerializerContext Context { get; }

		#region Serialize

		bool SerializeBlock<TValue>(
			TValue value,
			int blockOffset,
			int blockSize);

		bool SerializeBlock(
			Type valueType,
			object valueObject,
			int blockOffset,
			int blockSize);

		#endregion

		#region Deserialize

		bool DeserializeBlock<TValue>(
			int blockOffset,
			int blockSize,
			out TValue value);

		bool DeserializeBlock(
			Type valueType,
			int blockOffset,
			int blockSize,
			out object valueObject);

		#endregion

		#region Populate

		bool PopulateBlock<TValue>(
			TValue value,
			int blockOffset,
			int blockSize);

		bool PopulateBlock(
			Type valueType,
			object valueObject,
			int blockOffset,
			int blockSize);

		#endregion
	}
}