using System;

namespace HereticalSolutions.UUIDMapping
{
	[Serializable]
	public struct SymlinkDescriptor
	{
		public string SymlinkPath;
		
		public string TargetPath;
	}
}