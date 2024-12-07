namespace HereticalSolutions.Addressing
{
	public interface IAddressDomain
	{
		bool CheckPathCollision(
			Address address);

		bool CheckUUIDCollision(
			Address address);

		bool GenerateUUID(
			ref Address address);

		bool HasPath(
			Address address);

		bool HasUUID(
			Address address);

		bool TryGetByPath(
			Address address,
			out object target);

		bool TryGetByUUID(
			Address address,
			out object target);

		void RegisterAddress(
			Address address,
			object target);

		void UnregisterAddress(
			Address address);

		void ClearAllAddresses();		
	}
}