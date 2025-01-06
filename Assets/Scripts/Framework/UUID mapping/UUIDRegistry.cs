using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.UUIDMapping
{
	public class UUIDRegistry
		: IUUIDRegistry,
		  IUUIDRegistryWithSymlinks,
		  IUUIDRegistryWithVarlinks,
		  IUUIDRegistryWithWildcards
	{
		private readonly IOneToOneMap<string, Guid> pathToUUIDMap;

		private readonly IRepository<string, SymlinkDescriptor> symlinkRepository;

		private readonly IRepository<string, VarlinkDescriptor> varlinkRepository;

		private readonly ILogger logger;

		#region IUUIDRegistry

		#region Has

		public bool HasUUID(
			Guid uuid)
		{
			return pathToUUIDMap.HasRight(uuid);
		}

		public bool HasPath(
			string path)
		{
			return pathToUUIDMap.HasLeft(path);
		}

		#endregion

		#region Get

		public bool TryGetUUIDByPath(
			string path,
			out Guid uuid)
		{
			return pathToUUIDMap.TryGetRight(path, out uuid);
		}

		public bool TryGetPathByUUID(
			Guid uuid,
			out string path)
		{
			return pathToUUIDMap.TryGetLeft(uuid, out path);
		}

		#endregion

		#region Add

		public bool TryAdd(
			string path,
			Guid uuid)
		{
			return pathToUUIDMap.TryAdd(
				path,
				uuid);
		}

		public bool TryModifyPath(
			Guid uuid,
			string path)
		{
			return pathToUUIDMap.TryUpdateByRight(
				path,
				uuid);
		}

		public bool TryModifyUUID(
			string path,
			Guid uuid)
		{
			return pathToUUIDMap.TryUpdateByLeft(
				path,
				uuid);
		}

		#endregion

		#region Remove

		public bool TryRemove(
			Guid uuid)
		{
			return pathToUUIDMap.TryRemoveByRight(uuid);
		}

		public bool TryRemove(
			string path)
		{
			return pathToUUIDMap.TryRemoveByLeft(path);
		}

		#endregion

		#region All

		public IEnumerable<AddressDescriptor> AllValues
		{
			get
			{
				foreach (var path in pathToUUIDMap.LeftValues)
				{
					if (TryGetUUIDByPath(
						path,
						out var uuid))
					{
						yield return new AddressDescriptor
						{
							UUID = uuid,

							Path = path
						};
					}
				}
			}
		}

		public IEnumerable<Guid> AllUUIDs
		{
			get
			{
				return pathToUUIDMap.RightValues;
			}
		}

		public IEnumerable<string> AllPaths
		{
			get
			{
				return pathToUUIDMap.LeftValues;
			}
		}

		#endregion

		#endregion

		#region IUUIDRegistryWithSymlinks

		#region Has

		public bool IsSymlink(
			string path)
		{
			
		}

		#endregion

		#region Get

		bool TryGetSymlink(
			string path,
			out SymlinkDescriptor symlink);

		#endregion

		#region Add

		bool TryAddSymlink(
			string symlinkPath,
			string targetPath);

		bool TryAddOrUpdateSymlink(
			string symlinkPath,
			string targetPath);

		#endregion

		#region Remove

		bool TryRemoveSymlink(
			string symlinkPath);

		#endregion

		#endregion

		#region IUUIDRegistryWithVarlinks

		#region Has

		bool IsVarlink(
			string path);

		#endregion

		#region Get

		bool TryGetVarlink(
			string path,
			out VarlinkDescriptor varlink);

		#endregion

		#region Add

		bool TryAddVarlink(
			string varlinkPath,
			string targetPath);

		bool TryAddOrUpdateVarlink(
			string varlinkPath,
			string targetPath);

		#endregion

		#region Remove

		bool TryRemoveVarlink(
			string varlinkPath);

		#endregion

		#endregion

		#region IUUIDRegistryWithWildcards

		Guid[] GetAllUUIDsByPath(
			string path);

		uint GetAllUUIDsByPathNonAlloc(
			string path,
			Guid[] uuids);

		#endregion
	}
}