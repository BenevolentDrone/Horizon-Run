using System;

namespace HereticalSolutions.Persistence
{
	public interface IDataConverter
	{
		#region Read

		bool ReadAndConvert<TValue>(
			IDataConverterCommandContext context,
			out TValue value);

		bool ReadAndConvert(
			Type valueType,
			IDataConverterCommandContext context,
			out object value);

		#endregion

		#region Write

		bool ConvertAndWrite<TValue>(
			IDataConverterCommandContext context,
			TValue value);

		bool ConvertAndWrite(
			Type valueType,
			IDataConverterCommandContext context,
			object value);

		#endregion

		#region Append

		bool ConvertAndAppend<TValue>(
			IDataConverterCommandContext context,
			TValue value);

		bool ConvertAndAppend(
			Type valueType,
			IDataConverterCommandContext context,
			object value);

		#endregion
	}
}