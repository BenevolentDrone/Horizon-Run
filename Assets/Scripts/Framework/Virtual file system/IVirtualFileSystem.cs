using System;

namespace HereticalSolutions.VirtualFileSystem
{
	public interface IVirtualFileSystem
	{
		#region Has

		bool HasFile(
			Guid uuid);

		bool HasFile(
			string path);

		#endregion

		#region Get

		bool TryGetFile(
			Guid uuid,
			out IVirtualFile file);

		IVirtualFile[] GetAllFilesByPath(
			string path);

		uint GetAllFilesByPathNonAlloc(
			string path,
			Guid[] uuids);

		#endregion

		#region Add

		bool TryAddFile(
			string path,
			Guid uuid,
			IVirtualFile file);

		bool AddOrOverwriteFile(
			string path,
			Guid uuid,
			IVirtualFile file);

		#endregion

		#region Remove

		#endregion

		#region All

		#endregion
	}
}