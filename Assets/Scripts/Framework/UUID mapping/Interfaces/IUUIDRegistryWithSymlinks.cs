namespace HereticalSolutions.UUIDMapping
{
	public interface IUUIDRegistryWithSymlinks
		: IUUIDRegistry
	{
		#region Has

		bool IsSymlink(
			string path);

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
	}
}