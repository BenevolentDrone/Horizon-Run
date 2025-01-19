using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.UUIDMapping;

using HereticalSolutions.Metadata;

using HereticalSolutions.Logging;

namespace HereticalSolutions.VirtualFileSystem
{
	public interface IVirtualFile
	{
		#region Metadata

		AddressDescriptor AddressDescriptor { get; }

		FileDescriptor FileDescriptor { get; }

		IWeaklyTypedMetadata Metadata { get; }

		//TODO: decide on this one
		//Guid[] Dependencies { get; }

		#endregion

		//Assume that the file already exists in the RAM
		/*
		bool Allocated { get; }

		#region Allocate

		bool Allocate();

		Task<bool> AllocateAsync(
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progresslogger);

		#endregion

		#region Free

		bool Free();

		Task<bool> FreeAsync(
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progresslogger);

		#endregion
		*/

		byte[] Bytes { get; }
	}
}