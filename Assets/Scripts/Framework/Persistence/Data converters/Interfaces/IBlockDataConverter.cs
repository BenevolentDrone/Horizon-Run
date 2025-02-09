using System;

namespace HereticalSolutions.Persistence
{
	public interface IBlockDataConverter
	{
		#region Read

		bool ReadBlockAndConvert<TValue>(
			IDataConverterCommandContext context,
			int blockOffset,
			int blockSize,
			out TValue value);

		bool ReadBlockAndConvert(
			Type valueType,
			IDataConverterCommandContext context,
			int blockOffset,
			int blockSize,
			out object value);

		#endregion

		#region Write

		bool ConvertAndWriteBlock<TValue>(
			IDataConverterCommandContext context,
			TValue value,
			int blockOffset,
			int blockSize);

		bool ConvertAndWriteBlock(
			Type valueType,
			IDataConverterCommandContext context,
			object value,
			int blockOffset,
			int blockSize);

		#endregion
	}
}