using System;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Collections.Managed
{
	public class NonAllocBPlusTreeMapNode<TKey, TValue>
		: ICleanuppable
	{
		public bool IsLeaf;

		public TKey[] Keys;

		public TValue[] Values;

		public int KeysCount;

		public NonAllocBPlusTreeMapNode<TKey, TValue>[] Children;

		public NonAllocBPlusTreeMapNode<TKey, TValue> Next;

		public NonAllocBPlusTreeMapNode()
		{
			IsLeaf = false;

			Keys = Array.Empty<TKey>();

			Values = Array.Empty<TValue>();

			KeysCount = 0;

			Children = null;

			Next = null;
		}

		public NonAllocBPlusTreeMapNode(
			int degree,
			bool isLeaf)
		{
			Initialize(
				degree,
				isLeaf);
		}

		public void Initialize(
			int degree,
			bool isLeaf)
		{
			IsLeaf = isLeaf;

			if (Keys == null || Keys.Length < degree - 1)
			{
				Keys = new TKey[degree - 1];
			}
			else
			{
				Array.Clear(
					Keys,
					0,
					Keys.Length);
			}

			KeysCount = 0;

			if (Values == null || Values.Length < degree - 1)
			{
				Values = new TValue[degree - 1];
			}
			else
			{
				Array.Clear(
					Values,
					0,
					Values.Length);
			}

			if (Children == null || Children.Length < degree)
			{
				Children = new NonAllocBPlusTreeMapNode<TKey, TValue>[degree];
			}
			else
			{
				Array.Clear(
					Children,
					0,
					Children.Length);
			}

			Next = null;
		}

		#region ICleanUppable

		public void Cleanup()
		{
			if (Children != null)
			{
				Array.Clear(
					Children,
					0,
					Children.Length);
			}

			IsLeaf = false;

			if (Keys != null)
			{
				Array.Clear(
					Keys,
					0,
					Keys.Length);
			}

			KeysCount = 0;

			if (Values != null)
			{
				Array.Clear(
					Values,
					0,
					Values.Length);
			}

			Next = null;
		}

		#endregion
	}
}