using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.UUIDMapping
{
	//TODO: symlinks, warlinks and wildcards should NOT be t he part of UUID mapping. They don't even deal with UUIDs, only paths
	//TODO: actually, the PATHING should be extended and UUID mapping should be the part of it and its own namespace
	public class UUIDRegistry
		: IUUIDRegistry
		  //IUUIDRegistryWithSymlinks,
		  //IUUIDRegistryWithVarlinks,
		  //IUUIDRegistryWithWildcards
	{
		private readonly IOneToOneMap<string, Guid> pathToUUIDMap;

		//private readonly IRepository<string, SymlinkDescriptor> symlinkRepository;

		//private readonly IRepository<string, VarlinkDescriptor> varlinkRepository;

		private readonly ILogger logger;

		#region IUUIDRegistry

		#region Has

		public bool HasUUID(
			Guid uuid)
		{
			return pathToUUIDMap.HasRight(
				uuid);
		}

		public bool HasPath(
			string path)
		{
			return pathToUUIDMap.HasLeft(
				path);
		}

		#endregion

		#region Get

		public bool TryGetUUIDByPath(
			string path,
			out Guid uuid)
		{
			return pathToUUIDMap.TryGetRight(
				path,
				out uuid);
		}

		public bool TryGetPathByUUID(
			Guid uuid,
			out string path)
		{
			return pathToUUIDMap.TryGetLeft(
				uuid,
				out path);
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
			return pathToUUIDMap.TryRemoveByRight(
				uuid);
		}

		public bool TryRemove(
			string path)
		{
			return pathToUUIDMap.TryRemoveByLeft(
				path);
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

		/*
		#region IUUIDRegistryWithSymlinks

		#region Has

		public bool IsSymlink(
			string path)
		{
			return symlinkRepository.Has(
				path);
		}

		#endregion

		#region Get

		public bool TryGetSymlink(
			string path,
			out SymlinkDescriptor symlink)
		{
			return symlinkRepository.TryGet(
				path,
				out symlink);
		}

		#endregion

		#region Add

		public bool TryAddSymlink(
			string symlinkPath,
			string targetPath)
		{
			return symlinkRepository.TryAdd(
				symlinkPath,
				new SymlinkDescriptor
				{
					SymlinkPath = symlinkPath,

					TargetPath = targetPath
				});
		}

		public bool TryAddOrUpdateSymlink(
			string symlinkPath,
			string targetPath)
		{
			if (symlinkRepository.Has(
				symlinkPath))
			{
				return symlinkRepository.TryUpdate(
					symlinkPath,
					new SymlinkDescriptor
					{
						SymlinkPath = symlinkPath,

						TargetPath = targetPath
					});
			}

			return symlinkRepository.TryUpdate(
				symlinkPath,
				new SymlinkDescriptor
				{
					SymlinkPath = symlinkPath,

					TargetPath = targetPath
				});
		}

		#endregion

		#region Remove

		public bool TryRemoveSymlink(
			string symlinkPath)
		{
			return symlinkRepository.TryRemove(
				symlinkPath);
		}

		#endregion

		#endregion

		#region IUUIDRegistryWithVarlinks

		#region Has

		public bool IsVarlink(
			string path)
		{
			return varlinkRepository.Has(
				path);
		}

		#endregion

		#region Get

		public bool TryGetVarlink(
			string path,
			out VarlinkDescriptor varlink)
		{
			return varlinkRepository.TryGet(
				path,
				out varlink);
		}

		#endregion

		#region Add

		public bool TryAddVarlink(
			string varlinkPath,
			string targetPath)
		{
			return varlinkRepository.TryAdd(
				varlinkPath,
				new VarlinkDescriptor
				{
					VarlinkPath = varlinkPath,

					TargetPath = targetPath
				});
		}

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
		*/
	}
}