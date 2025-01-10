using System;

namespace HereticalSolutions.Persistence
{
	public interface IBlockSerializationStrategy
	{
		#region Read

		bool BlockRead<TValue>(
			int blockOffset,
			int blockSize,
			out TValue value);

		bool BlockRead(
			Type valueType,
			int blockOffset,
			int blockSize,
			out object value);

		#endregion

		#region Write

		bool BlockWrite<TValue>(
			TValue value,
			int blockOffset,
			int blockSize);

		bool BlockWrite(
			Type valueType,
			object value,
			int blockOffset,
			int blockSize);

		#endregion
	}
}