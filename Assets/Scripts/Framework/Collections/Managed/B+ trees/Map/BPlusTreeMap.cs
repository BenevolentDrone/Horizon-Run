using System;
using System.Collections.Generic;

namespace HereticalSolutions.Collections.Managed
{
	public class BPlusTreeMap<TKey, TValue>
		: IBPlusTreeMap<TKey, TValue>
	{
		public static readonly IComparer<TKey> Comparer = Comparer<TKey>.Default;

		private readonly int degree;

		private readonly int halfDegree;

		private BPlusTreeMapNode<TKey, TValue> root;

		public BPlusTreeMap(
			int degree)
		{
			this.degree = degree;

			halfDegree = (int)Math.Ceiling((float)degree / 2);

			root = new BPlusTreeMapNode<TKey, TValue>(
				degree,
				true);
		}

		#region IBPlusTree

		public bool Search(
			TKey key,
			out TValue value)
		{
			return Search(
				root,
				key,
				out value);
		}

		public void Insert(
			TKey key,
			TValue value)
		{
			if (root.KeysCount == degree - 1)
			{
				BPlusTreeMapNode<TKey, TValue> newRoot = AllocateNode(false);

				newRoot.Children[0] = root;

				Split(
					newRoot,
					root,
					0);

				Insert(
					newRoot,
					key,
					value);

				root = newRoot;
			}
			else
			{
				Insert(
					root,
					key,
					value);
			}
		}

		public bool Remove(
			TKey key)
		{
			bool result = DeleteKey(
				root,
				key);

			if (root.KeysCount == 0
				&& !root.IsLeaf)
			{
				var previousRoot = root;

				root = root.Children[0];

				FreeNode(previousRoot);
			}

			return result;
		}

		public int Count
		{
			get
			{
				int count = 0;

				BPlusTreeMapNode<TKey, TValue> current = root;

				while (!current.IsLeaf)
				{
					current = current.Children[0];  // Go to the leftmost child
				}

				while (current != null)
				{
					count += current.KeysCount;

					current = current.Next;  // Move to the next leaf node
				}

				return count;
			}
		}

		public IEnumerable<TKey> AllKeys
		{
			get
			{
				BPlusTreeMapNode<TKey, TValue> current = root;

				while (!current.IsLeaf)
				{
					current = current.Children[0];  // Go to the leftmost child
				}

				while (current != null)
				{
					for (int i = 0; i < current.KeysCount; i++)
					{
						yield return current.Keys[i];
					}

					current = current.Next;  // Move to the next leaf node
				}
			}
		}

		public IEnumerable<TValue> AllValues
		{
			get
			{
				BPlusTreeMapNode<TKey, TValue> current = root;

				while (!current.IsLeaf)
				{
					current = current.Children[0];  // Go to the leftmost child
				}

				while (current != null)
				{
					for (int i = 0; i < current.KeysCount; i++)
					{
						yield return current.Values[i];
					}

					current = current.Next;  // Move to the next leaf node
				}
			}
		}

		public void Clear()
		{
			RecursiveClear(
				root);

			root = AllocateNode(
				true);
		}

		#endregion

		#region Node operations

		#region Allocation and deallocation

		private BPlusTreeMapNode<TKey, TValue> AllocateNode(
			bool leaf)
		{
			return new BPlusTreeMapNode<TKey, TValue>(
				degree,
				leaf);
		}

		private void FreeNode(
			BPlusTreeMapNode<TKey, TValue> node)
		{
			node.Cleanup();
		}

		#endregion

		#region CRUD operations

		#region Searches

		private int FindKeyFromStart(
			BPlusTreeMapNode<TKey, TValue> node,
			TKey key)
		{
			int i = 0;

			while (i < node.KeysCount
				&& Comparer.Compare(
					key,
					node.Keys[i])
					> 0)
			{
				i++;
			}

			return i;
		}

		private int FindKeyFromFinish(
			BPlusTreeMapNode<TKey, TValue> node,
			TKey key)
		{
			int i = node.KeysCount - 1;

			while (i >= 0
				&& Comparer.Compare(
					node.Keys[i],
					key)
					> 0)
			{
				i--;
			}

			return i + 1;
		}

		private bool Search(
			BPlusTreeMapNode<TKey, TValue> node,
			TKey key,
			out TValue value)
		{
			int i = 0;

			while (i < node.KeysCount)
			{
				int compareResult = Comparer.Compare(
					key,
					node.Keys[i]);

				if (compareResult == 0)
				{
					value = node.Values[i];

					return true;
				}

				if (compareResult > 0)
				{
					i++;
				}

				if (compareResult < 0)
				{
					if (node.IsLeaf)
					{
						value = default;

						return false;
					}

					return Search(
						node.Children[i],
						key,
						out value);
				}
			}

			value = default;

			return false;
		}

		#endregion

		private void Insert(
			BPlusTreeMapNode<TKey, TValue> node,
			TKey key,
			TValue value)
		{
			if (node.IsLeaf)
			{
				int i = FindKeyFromFinish(
					node,
					key);

				Array.Copy(
					node.Keys,
					i,
					node.Keys,
					i + 1,
					node.KeysCount - i);

				Array.Copy(
					node.Values,
					i,
					node.Values,
					i + 1,
					node.KeysCount - i);

				node.Keys[i] = key;

				node.Values[i] = value;

				node.KeysCount++;
			}
			else
			{
				int i = FindKeyFromFinish(
					node,
					key);

				if (node.Children[i].KeysCount == degree - 1)
				{
					Split(
						node,
						node.Children[i],
						i);

					if (Comparer.Compare(
						node.Keys[i],
						key)
						< 0)
					{
						i++;
					}
				}

				Insert(
					node.Children[i],
					key,
					value);
			}
		}

		private bool DeleteKey(
			BPlusTreeMapNode<TKey, TValue> node,
			TKey key)
		{
			int keyIndex = FindKeyFromStart(
				node,
				key);

			if (keyIndex < node.KeysCount
				&& Comparer.Compare(
					node.Keys[keyIndex],
					key)
					== 0)
			{
				if (node.IsLeaf)
				{
					RemoveFromLeaf(
						node,
						keyIndex);

					return true;
				}

				TKey predecessor = GetPredecessor(
					node,
					keyIndex);

				node.Keys[keyIndex] = predecessor;

				DeleteKey(
					node.Children[keyIndex],
					predecessor);

				return true;
			}

			if (node.IsLeaf)
				return false;

			bool isLastChild = (keyIndex == node.KeysCount);

			if (node.Children[keyIndex].KeysCount < halfDegree)
			{
				Rebalance(
					node,
					keyIndex);
			}

			if (isLastChild
				&& keyIndex > node.KeysCount)
			{
				return DeleteKey(
					node.Children[keyIndex - 1],
					key);
			}

			return DeleteKey(
				node.Children[keyIndex],
				key);
		}

		private void RemoveFromLeaf(
			BPlusTreeMapNode<TKey, TValue> node,
			int keyIndex)
		{
			Array.Copy(
				node.Keys,
				keyIndex + 1,
				node.Keys,
				keyIndex,
				node.KeysCount - keyIndex - 1);

			Array.Copy(
				node.Values,
				keyIndex + 1,
				node.Values,
				keyIndex,
				node.KeysCount - keyIndex - 1);

			node.KeysCount--;
		}

		private TKey GetPredecessor(
			BPlusTreeMapNode<TKey, TValue> node,
			int keyIndex)
		{
			BPlusTreeMapNode<TKey, TValue> current = node.Children[keyIndex];

			while (!current.IsLeaf)
			{
				current = current.Children[current.KeysCount];
			}

			return current.Keys[current.KeysCount - 1];
		}

		#endregion

		#region Splits and rebalances

		private void Split(
			BPlusTreeMapNode<TKey, TValue> parent,
			BPlusTreeMapNode<TKey, TValue> node,
			int i)
		{
			BPlusTreeMapNode<TKey, TValue> newSibling = AllocateNode(
				node.IsLeaf);

			newSibling.KeysCount = halfDegree - 1;

			Array.Copy(
				node.Keys,
				halfDegree,
				newSibling.Keys,
				0,
				halfDegree - 1);

			Array.Copy(
				node.Values,
				halfDegree,
				newSibling.Values,
				0,
				halfDegree - 1);

			if (!node.IsLeaf)
			{
				Array.Copy(
					node.Children,
					halfDegree,
					newSibling.Children,
					0,
					halfDegree);
			}

			node.KeysCount = halfDegree - 1;

			Array.Copy(
				parent.Children,
				i + 1,
				parent.Children,
				i + 2,
				parent.KeysCount - i);

			parent.Children[i + 1] = newSibling;

			Array.Copy(
				parent.Keys,
				i,
				parent.Keys,
				i + 1,
				parent.KeysCount - i);

			parent.Keys[i] = node.Keys[halfDegree - 1];

			parent.KeysCount++;
		}

		private void Rebalance(
			BPlusTreeMapNode<TKey, TValue> node,
			int keyIndex)
		{
			if (keyIndex != 0
				&& node.Children[keyIndex - 1].KeysCount >= halfDegree)
			{
				BorrowFromPrev(
					node,
					keyIndex);
			}
			else if (keyIndex != node.KeysCount
				&& node.Children[keyIndex + 1].KeysCount >= halfDegree)
			{
				BorrowFromNext(
					node,
					keyIndex);
			}
			else
			{
				if (keyIndex != node.KeysCount)
				{
					Merge(
						node,
						keyIndex);
				}
				else
				{
					Merge(
						node,
						keyIndex - 1);
				}
			}
		}

		private void BorrowFromPrev(
			BPlusTreeMapNode<TKey, TValue> node,
			int keyIndex)
		{
			BPlusTreeMapNode<TKey, TValue> child = node.Children[keyIndex];

			BPlusTreeMapNode<TKey, TValue> sibling = node.Children[keyIndex - 1];

			Array.Copy(
				child.Keys,
				0,
				child.Keys,
				1,
				child.KeysCount);

			Array.Copy(
				child.Values,
				0,
				child.Values,
				1,
				child.KeysCount);

			if (!child.IsLeaf)
			{
				Array.Copy(
					child.Children,
					0,
					child.Children,
					1,
					child.KeysCount + 1);
			}

			child.Keys[0] = node.Keys[keyIndex - 1];

			child.Values[0] = sibling.Values[sibling.KeysCount - 1];

			if (!child.IsLeaf)
			{
				child.Children[0] = sibling.Children[sibling.KeysCount];
			}

			node.Keys[keyIndex - 1] = sibling.Keys[sibling.KeysCount - 1];

			child.KeysCount++;

			sibling.KeysCount--;
		}

		private void BorrowFromNext(
			BPlusTreeMapNode<TKey, TValue> node,
			int keyIndex)
		{
			BPlusTreeMapNode<TKey, TValue> child = node.Children[keyIndex];

			BPlusTreeMapNode<TKey, TValue> sibling = node.Children[keyIndex + 1];

			child.Keys[child.KeysCount] = node.Keys[keyIndex];

			child.Values[child.KeysCount] = sibling.Values[0];

			if (!child.IsLeaf)
			{
				child.Children[child.KeysCount + 1] = sibling.Children[0];
			}

			node.Keys[keyIndex] = sibling.Keys[0];

			Array.Copy(
				sibling.Keys,
				1,
				sibling.Keys,
				0,
				sibling.KeysCount - 1);

			Array.Copy(
				sibling.Values,
				1,
				sibling.Values,
				0,
				sibling.KeysCount - 1);

			if (!sibling.IsLeaf)
			{
				Array.Copy(
					sibling.Children,
					1,
					sibling.Children,
					0,
					sibling.KeysCount);
			}

			child.KeysCount++;

			sibling.KeysCount--;
		}

		private void Merge(
			BPlusTreeMapNode<TKey, TValue> node,
			int keyIndex)
		{
			BPlusTreeMapNode<TKey, TValue> child = node.Children[keyIndex];
			
			BPlusTreeMapNode<TKey, TValue> sibling = node.Children[keyIndex + 1];

			child.Keys[child.KeysCount] = node.Keys[keyIndex];

			child.Values[child.KeysCount] = sibling.Values[0];

			if (!child.IsLeaf)
			{
				child.Children[child.KeysCount + 1] = sibling.Children[0];
			}


			Array.Copy(
				sibling.Keys,
				0,
				child.Keys,
				child.KeysCount + 1,
				sibling.KeysCount);

			Array.Copy(
				sibling.Values,
				0,
				child.Values,
				child.KeysCount + 1,
				sibling.Values.Length);

			if (!child.IsLeaf)
			{
				Array.Copy(
					sibling.Children,
					0,
					child.Children,
					child.KeysCount + 1,
					sibling.KeysCount + 1);
			}

			child.KeysCount += sibling.KeysCount + 1;


			Array.Copy(
				node.Keys,
				keyIndex + 1,
				node.Keys,
				keyIndex,
				node.KeysCount - keyIndex - 1);

			Array.Copy(
				node.Children,
				keyIndex + 2,
				node.Children,
				keyIndex + 1,
				node.KeysCount - keyIndex - 1);

			node.KeysCount--;

			FreeNode(sibling);
		}

		#endregion

		#region Cleanup

		public void RecursiveClear(
			BPlusTreeMapNode<TKey, TValue> node)
		{
			if (node.Children != null)
			{
				foreach (var child in node.Children)
				{
					RecursiveClear(
						child);
				}
			}

			FreeNode(node);
		}

		#endregion

		#endregion
	}
}