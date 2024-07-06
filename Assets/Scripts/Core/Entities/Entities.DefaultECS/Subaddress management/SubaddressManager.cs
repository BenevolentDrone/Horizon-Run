using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Entities
{
	public class SubaddressManager
		: ISubaddressManager
	{
		private readonly IOneToOneMap<string, ushort> subaddressMap;
		
		private readonly ILogger logger;

		private ushort lastAllocatedSubaddressIndex = 0;
		
		public SubaddressManager(
			IOneToOneMap<string, ushort> subaddressMap,
			ILogger logger = null)
		{
			this.subaddressMap = subaddressMap;

			this.logger = logger;
			
			lastAllocatedSubaddressIndex = 0;
		}

		#region ISubaddressManager

		public void MemorizeSubaddressPart(string subaddressPart)
		{
			if (subaddressMap.HasLeft(subaddressPart))
				return;

			lastAllocatedSubaddressIndex++;
			
			subaddressMap.Add(
				subaddressPart,
				lastAllocatedSubaddressIndex);
		}

		public bool TryGetSubaddressPart(
			string subaddressPart,
			out ushort subaddressPartIndex)
		{
			return subaddressMap.TryGetRight(
				subaddressPart,
				out subaddressPartIndex);
		}

		public bool TryGetSubaddressPart(
			ushort subaddressPartIndex,
			out string subaddressPart)
		{
			return subaddressMap.TryGetLeft(
				subaddressPartIndex,
				out subaddressPart);
		}

		#endregion
	}
}