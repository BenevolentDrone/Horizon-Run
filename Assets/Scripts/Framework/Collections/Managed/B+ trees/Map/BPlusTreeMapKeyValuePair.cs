using System.Collections.Generic;

namespace HereticalSolutions.Collections.Managed
{
	public struct BPlusTreeMapKeyValuePair<TKey, TValue>
	{
		public TKey Key { get; }

		public TValue Value { get; }
	
		public BPlusTreeMapKeyValuePair(
			TKey key,
			TValue value)
		{
			Key = key;
	
			Value = value;
		}

		// Custom equality and comparison logic for the key-value pair
		public static bool operator ==(
			BPlusTreeMapKeyValuePair<TKey, TValue> a,
			BPlusTreeMapKeyValuePair<TKey, TValue> b)
			=> EqualityComparer<TKey>.Default.Equals(a.Key, b.Key);

		public static bool operator !=(
			BPlusTreeMapKeyValuePair<TKey, TValue> a,
			BPlusTreeMapKeyValuePair<TKey, TValue> b)
			=> !(a == b);

		public override bool Equals(object obj)
		{
			if (obj is BPlusTreeMapKeyValuePair<TKey, TValue> pair)
			{
				return this == pair;
			}
			
			return false;
		}

		public override int GetHashCode() => Key.GetHashCode();
	}
}