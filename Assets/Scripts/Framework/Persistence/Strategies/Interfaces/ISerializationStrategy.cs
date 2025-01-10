using System;

namespace HereticalSolutions.Persistence
{
	public interface ISerializationStrategy
	{
		#region Read

		bool Read<TValue>(
			out TValue value);

		bool Read(
			Type valueType,
			out object value);

		#endregion

		#region Write

		bool Write<TValue>(
			TValue value);

		bool Write(
			Type valueType,
			object value);

		#endregion

		#region Append

		bool Append<TValue>(
			TValue value);

		bool Append(
			Type valueType,
			object value);

		#endregion
	}
}