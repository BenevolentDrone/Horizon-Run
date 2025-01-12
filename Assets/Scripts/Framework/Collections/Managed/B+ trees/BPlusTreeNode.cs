using System;
using System.Collections.Generic;

namespace HereticalSolutions.Collections.Managed
{
	public class BPlusTreeNode<T>
	{
		private readonly IComparer<T> comparer;

		public bool IsLeaf { get; set; }

		public T[] Keys { get; set; }

		public BPlusTreeNode<T>[] Children { get; set; }

		public BPlusTreeNode<T> Parent { get; set; }

		public BPlusTreeNode<T> Next { get; set; }

		public int KeysCount { get; set; }

		public BPlusTreeNode(
			int degree,
			bool isLeaf,
			IComparer<T> comparer)
		{
			this.comparer = comparer;

			IsLeaf = isLeaf;

			Keys = new T[degree - 1];

			Children = isLeaf
				? null
				: new BPlusTreeNode<T>[degree];
		}

		public void Insert(
			T key)
		{
			//int index = Array.BinarySearch(
			//	Keys,
			//	0,
			//	KeysCount,
			//	key,
			//	comparer);

			int index = Array.IndexOf(
				Keys,
				key,
				0,
				KeysCount);

			if (index < 0)
				index = ~index;

			Array.Copy(
				Keys,
				index,
				Keys,
				index + 1,
				KeysCount - index);

			Keys[index] = key;

			KeysCount++;
		}

		public bool Contains(
			T key)
		{
			//return Array.BinarySearch(
			//	Keys,
			//	0,
			//	KeysCount,
			//	key,
			//	comparer) >= 0;

			return Array.IndexOf(
				Keys,
				key,
				0,
				KeysCount) >= 0;
		}

		public bool Remove(
			T key)
		{
			//int index = Array.BinarySearch(
			//	Keys,
			//	0,
			//	KeysCount,
			//	key,
			//	comparer);

			int index = Array.IndexOf(
				Keys,
				key,
				0,
				KeysCount);

			if (index >= 0)
			{
				Array.Copy(
					Keys,
					index + 1,
					Keys,
					index,
					KeysCount - index - 1);

				KeysCount--;

				return true;
			}

			return false;
		}

		public bool IsOverflow()
		{
			return KeysCount >= Keys.Length;
		}

		public BPlusTreeNode<T> GetChildNode(
			T key)
		{
			for (int i = 0; i < KeysCount; i++)
			{
				if (comparer.Compare(key, Keys[i]) < 0)
				{
					return Children[i];
				}
			}

			return Children[KeysCount];
		}

		public void RecursiveClear()
		{
			if (Children != null)
			{
				foreach (var child in Children)
				{
					child?.RecursiveClear(); // Recursively clear child nodes
				}

				Children = null; // Clear references to child nodes
			}

			// Optional: Clear other references to allow GC to collect the node

			IsLeaf = false;

			Keys = null;

			Parent = null;

			Next = null;

			KeysCount = -1;
		}
	}
}