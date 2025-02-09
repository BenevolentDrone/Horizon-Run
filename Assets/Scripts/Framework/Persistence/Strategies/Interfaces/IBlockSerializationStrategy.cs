using System;

namespace HereticalSolutions.Persistence
{
	public interface IBlockSerializationStrategy
	{
		#region Read

		bool ReadBlock<TValue>(
			int blockOffset,
			int blockSize,
			out TValue value);

		bool ReadBlock(
			Type valueType,
			int blockOffset,
			int blockSize,
			out object value);

		#endregion

		#region Write

		bool WriteBlock<TValue>(
			TValue value,
			int blockOffset,
			int blockSize);

		bool WriteBlock(
			Type valueType,
			object value,
			int blockOffset,
			int blockSize);

		#endregion
	}
}