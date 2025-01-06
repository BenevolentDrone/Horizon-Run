using System;
using System.Collections.Generic;

namespace HereticalSolutions.UUIDMapping
{
	public interface IUUIDRegistry
	{
		#region Has

		bool HasUUID(
			Guid uuid);

		bool HasPath(
			string path);

		#endregion

		#region Get

		bool TryGetUUIDByPath(
			string path,
			out Guid uuid);

		bool TryGetPathByUUID(
			Guid uuid,
			out string path);

		#endregion

		#region Add

		bool TryAdd(
			string path,
			Guid uuid);

		bool TryModifyPath(
			Guid uuid,
			string path);

		bool TryModifyUUID(
			string path,
			Guid uuid);

		#endregion

		#region Remove

		bool TryRemove(
			Guid uuid);

		bool TryRemove(
			string path);

		#endregion

		#region All

		IEnumerable<AddressDescriptor> AllValues { get; }

		IEnumerable<Guid> AllUUIDs { get; }

		IEnumerable<string> AllPaths { get; }

		#endregion
	}
}