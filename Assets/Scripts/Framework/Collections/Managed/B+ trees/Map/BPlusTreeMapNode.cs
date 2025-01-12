using System;
using System.Collections.Generic;

namespace HereticalSolutions.Collections.Managed
{
	public class BPlusTreeMapNode<TKey, TValue>
	{
		private readonly IComparer<TKey> comparer;

		public bool IsLeaf { get; set; }

		public BPlusTreeMapKeyValuePair<TKey, TValue>[] KeyValuePairs { get; set; }

		public BPlusTreeMapNode<TKey, TValue>[] Children { get; set; }

		public BPlusTreeMapNode<TKey, TValue> Parent { get; set; }

		public BPlusTreeMapNode<TKey, TValue> Next { get; set; }

		public int KeysCount { get; set; }
		
		public BPlusTreeMapNode(
			int degree,
			bool isLeaf,
			IComparer<TKey> comparer)
		{
			IsLeaf = isLeaf;

			KeyValuePairs = new BPlusTreeMapKeyValuePair<TKey, TValue>[degree - 1];

			Children = isLeaf
				? null
				: new BPlusTreeMapNode<TKey, TValue>[degree];
			
			this.comparer = comparer;
		}

		public void Insert(
			TKey key,
			TValue value)
		{
			int index = IndexOf(key);

			if (index < 0)
				index = ~index;  // Insert at the position indicated by BinarySearch

			Array.Copy(
				KeyValuePairs,
				index,
				KeyValuePairs,
				index + 1,
				KeysCount - index);

			KeyValuePairs[index] = new BPlusTreeMapKeyValuePair<TKey, TValue>(
				key,
				value);

			KeysCount++;
		}

		public bool Contains(
			TKey key,
			out TValue value)
		{
			int index = IndexOf(key);

			if (index >= 0)
			{
				value = KeyValuePairs[index].Value;

				return true;
			}

			value = default(TValue);

			return false;
		}

		public bool Remove(
			TKey key)
		{
			int index = IndexOf(key);

			if (index >= 0)
			{
				Array.Copy(
					KeyValuePairs,
					index + 1,
					KeyValuePairs,
					index,
					KeysCount - index - 1);

				KeysCount--;

				return true;
			}

			return false;
		}

		public bool IsOverflow() => KeysCount >= KeyValuePairs.Length;

		public BPlusTreeMapNode<TKey, TValue> GetChildNode(
			TKey key)
		{
			for (int i = 0; i < KeysCount; i++)
			{
				if (comparer
					.Compare(
						key,
						KeyValuePairs[i].Key)
					< 0)
				{
					return Children[i];
				}
			}

			return Children[KeysCount];
		}

		public void RecursiveClear()
		{
			if (!IsLeaf)
			{
				// Clear all child nodes recursively

				foreach (var child in Children)
				{
					child?.RecursiveClear();
				}
			}

			// Clear the KeyValuePairs and Children arrays

			IsLeaf = false;

			KeyValuePairs = null;

			Children = null;

			Parent = null;

			Next = null;

			KeysCount = -1;
		}

		private int IndexOf(
			TKey key)
		{
			//Manual quicksearch (at least the way ChatGPT understands quicksearch)

			int left = 0;
			int right = KeysCount - 1;

			while (left <= right)
			{
				int mid = left + (right - left) / 2;
				
				int cmp = comparer.Compare(key, KeyValuePairs[mid].Key);

				if (cmp == 0)
				{
					return mid;  // Exact match found
				}
				else if (cmp < 0)
				{
					right = mid - 1;
				}
				else
				{
					left = mid + 1;
				}
			}

			return ~left;  // Return the insertion point (bitwise negation of the left index)

			//Allocates with predicate
			//return Array.FindIndex(
			//	KeyValuePairs,
			//	0,
			//	KeysCount,
			//	pair => comparer.Compare(
			//		pair.Key,
			//		key) == 0);

			//Allocates with new() EVERY time
			//return Array
			//	.BinarySearch(
			//		KeyValuePairs,
			//		0,
			//		KeysCount,
			//		new BPlusTreeMapKeyValuePair<TKey, TValue>(
			//			key,
			//			default),
			//		new BPlusTreeMapKeyValueComparer<TKey, TValue>(
			//			comparer));
		}
	}
}