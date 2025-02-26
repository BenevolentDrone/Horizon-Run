using System;

namespace HereticalSolutions.Persistence
{
	public interface IByteArrayConverter
	{
		bool ConvertFromBytes<TValue>(
			byte[] byteArray,
			out TValue value);

		bool ConvertFromBytes(
			Type valueType,
			byte[] byteArray,
			out object value);

		bool ConvertToBytes<TValue>(
			TValue value,
			out byte[] byteArray);

		bool ConvertToBytes(
			Type valueType,
			object value,
			out byte[] byteArray);
	}
}