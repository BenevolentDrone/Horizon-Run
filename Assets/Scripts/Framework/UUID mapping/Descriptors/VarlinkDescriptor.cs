using System;

namespace HereticalSolutions.UUIDMapping
{
	[Serializable]
	public struct VarlinkDescriptor
	{
		public string VarlinkPath;
		
		public VariantDescriptor[] Variants;
	}
}