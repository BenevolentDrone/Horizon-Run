using System;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Collections.Managed
{
	public class BPlusTreeNode<T>
		: ICleanuppable
	{
		public bool IsLeaf;

		public T[] Keys;

		public int KeysCount;

		public BPlusTreeNode<T>[] Children;

		public BPlusTreeNode<T> Next;

		public BPlusTreeNode()
		{
			IsLeaf = false;

			Keys = Array.Empty<T>();

			KeysCount = 0;

			Children = null;

			Next = null;
		}

		public BPlusTreeNode(
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
				Children = new BPlusTreeNode<T>[degree];
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