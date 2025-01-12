using System.Collections.Generic;

namespace HereticalSolutions.Collections.Managed
{
	public class BPlusTreeMapKeyValueComparer<TKey, TValue>
		: IComparer<BPlusTreeMapKeyValuePair<TKey, TValue>>
	{
		private readonly IComparer<TKey> comparer;
	
		public BPlusTreeMapKeyValueComparer(
			IComparer<TKey> comparer)
		{
			this.comparer = comparer;
		}
	
		public int Compare(
			BPlusTreeMapKeyValuePair<TKey, TValue> value1,
			BPlusTreeMapKeyValuePair<TKey, TValue> value2)
		{
			return comparer.Compare(
				value1.Key,
				value2.Key);
		}
	}
}