using System;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Collections.Managed
{
	public class NonAllocBPlusTreeNode<T>
		: ICleanuppable
	{
		public bool IsLeaf;

		public T[] Keys;

		public int KeysCount;

		public NonAllocBPlusTreeNode<T>[] Children;

		public NonAllocBPlusTreeNode<T> Next;

		public NonAllocBPlusTreeNode()
		{
			IsLeaf = false;

			Keys = Array.Empty<T>();

			KeysCount = 0;

			Children = null;

			Next = null;
		}

		public NonAllocBPlusTreeNode(
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
				Keys = new T[degree - 1];
			}
			else
			{
				Array.Clear(
					Keys,
					0,
					Keys.Length);
			}

			KeysCount = 0;

			if (Children == null || Children.Length < degree)
			{
				Children = new NonAllocBPlusTreeNode<T>[degree];
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

			Next = null;
		}

		#endregion
	}
}