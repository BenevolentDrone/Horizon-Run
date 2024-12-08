using System;

namespace HereticalSolutions.Addressing
{
	public interface IAddressDomain
	{
		#region Conversions

		bool TryGetUUIDByPath(
			string path,
			out Guid uuid);

		bool TryGetPathByUUID(
			Guid uuid,
			out string path);

		#endregion

		#region Collisions

		bool CheckPathCollision(
			Address address);

		bool CheckUUIDCollision(
			Address address);

		#endregion

		#region Generation

		bool GenerateUUID(
			ref Address address);

		#endregion

		#region Has

		bool HasPath(
			Address address);

		bool HasUUID(
			Address address);

		#endregion

		#region Get

		bool TryGetByPath(
			Address address,
			out object target);

		bool TryGetByUUID(
			Address address,
			out object target);

		#endregion

		#region Register

		void RegisterAddress(
			Address address,
			object target);

		#endregion

		#region Unregister

		void UnregisterAddress(
			Address address);

		#endregion

		void ClearAllAddresses();		
	}
}