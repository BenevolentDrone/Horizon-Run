using System;

namespace HereticalSolutions.UUIDMapping
{
	[Serializable]
	public struct FileDescriptor
	{
		public uint Offset;

		public uint Size;

		public string DataType;
	}
}