namespace HereticalSolutions.UUIDMapping
{
	public interface IUUIDRegistryWithVarlinks
		: IUUIDRegistry
	{
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
	}
}