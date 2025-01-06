using System;

namespace HereticalSolutions.UUIDMapping
{
	public interface IUUIDRegistryWithWildcards
		: IUUIDRegistry
	{
		Guid[] GetAllUUIDsByPath(
			string path);

		uint GetAllUUIDsByPathNonAlloc(
			string path,
			Guid[] uuids);
	}
}