using System;
using System.Collections.Generic;

namespace HereticalSolutions.Collections.Managed
{
	public class BPlusTreeMap<TKey, TValue>
		: IBPlusTreeMap<TKey, TValue>
	{
		private readonly IComparer<TKey> comparer;

		private BPlusTreeMapNode<TKey, TValue> root;

		private readonly int degree;
		
		public BPlusTreeMap(int degree)
		{
			comparer = Comparer<TKey>.Default;

			this.degree = degree;

			root = new BPlusTreeMapNode<TKey, TValue>(
				degree,
				true,
				comparer);
		}

		#region IBPlusTreeMap

		public void Insert(
			TKey key,
			TValue value)
		{
			BPlusTreeMapNode<TKey, TValue> node = root;
	
			// Find the appropriate leaf node where the key should be inserted

			while (!node.IsLeaf)
			{
				node = node.GetChildNode(key);
			}
	
			// Insert the key-value pair in the leaf node

			node.Insert(
				key,
				value);
	
			// Split the node if it exceeds the maximum number of keys

			if (node.IsOverflow())
			{
				Split(node);
			}
		}
	
		public bool Search(
			TKey key,
			out TValue value)
		{
			BPlusTreeMapNode<TKey, TValue> node = root;
	
			// Traverse the tree to find the leaf node containing the key

			while (!node.IsLeaf)
			{
				node = node.GetChildNode(key);
			}
	
			// Check if the key is present in the leaf node

			return node.Contains(
				key,
				out value);
		}
	
		public bool Remove(
			TKey key)
		{
			BPlusTreeMapNode<TKey, TValue> node = root;
	
			// Traverse the tree to find the leaf node containing the key

			while (!node.IsLeaf)
			{
				node = node.GetChildNode(key);
			}
	
			// Remove the key from the leaf node

			bool removed = node.Remove(key);
	
			// If key was removed, check if node is underfilled

			if (removed && node.KeysCount < (degree - 1) / 2)
			{
				Rebalance(node);
			}
	
			return removed;
		}

		public int Count
		{
			get
			{
				int count = 0;

				BPlusTreeMapNode<TKey, TValue> current = root;

				// Traverse down to the first leaf node

				while (!current.IsLeaf)
				{
					current = current.Children[0];  // Go to the leftmost child
				}

				// Traverse through the leaf nodes and count the key-value pairs

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

				// Traverse down to the first leaf node

				while (!current.IsLeaf)
				{
					current = current.Children[0];  // Go to the leftmost child
				}

				// Traverse through the leaf nodes and yield keys

				while (current != null)
				{
					foreach (var pair in current.KeyValuePairs)
					{
						yield return pair.Key;
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

				// Traverse down to the first leaf node

				while (!current.IsLeaf)
				{
					current = current.Children[0];  // Go to the leftmost child
				}

				// Traverse through the leaf nodes and yield values

				while (current != null)
				{
					foreach (var pair in current.KeyValuePairs)
					{
						yield return pair.Value;
					}

					current = current.Next;  // Move to the next leaf node
				}
			}
		}

		public void InOrderTraversal(
			Action<TKey, TValue> action)
		{
			InOrderTraversal(root, action);
		}

		public void Clear()
		{
			root.RecursiveClear();

			root = new BPlusTreeMapNode<TKey, TValue>(
				degree,
				true,
				comparer);
		}

		#endregion

		private void InOrderTraversal(
			BPlusTreeMapNode<TKey, TValue> node,
			Action<TKey, TValue> action)
		{
			if (node.IsLeaf)
			{
				var current = node;

				while (current != null)
				{
					foreach (var pair in current.KeyValuePairs)
					{
						action(pair.Key, pair.Value);
					}

					current = current.Next;
				}
			}
			else
			{
				foreach (var child in node.Children)
				{
					InOrderTraversal(
						child,
						action);
				}
			}
		}

		private void Split(BPlusTreeMapNode<TKey, TValue> node)
		{
			// Create a new node to split the current node

			BPlusTreeMapNode<TKey, TValue> newNode = new BPlusTreeMapNode<TKey, TValue>(
				degree,
				true,
				comparer);

			int midIndex = node.KeysCount / 2;
	
			// Transfer keys and values to the new node

			newNode.KeyValuePairs = new BPlusTreeMapKeyValuePair<TKey, TValue>[degree - 1];

			Array.Copy(
				node.KeyValuePairs,
				midIndex,
				newNode.KeyValuePairs,
				0,
				node.KeysCount - midIndex);

			node.KeysCount = midIndex;
	
			if (node.Children != null)
			{
				newNode.Children = new BPlusTreeMapNode<TKey, TValue>[degree];

				Array.Copy(
					node.Children,
					midIndex,
					newNode.Children,
					0,
					node.Children.Length - midIndex);

				node.Children = node
					.Children
					.AsSpan(
						0,
						midIndex)
					.ToArray();
			}
	
			// Link the leaf nodes if necessary

			if (node.IsLeaf)
			{
				newNode.Next = node.Next;

				node.Next = newNode;
			}
	
			// Create a new root node if necessary

			if (node == root)
			{
				BPlusTreeMapNode<TKey, TValue> newRoot = new BPlusTreeMapNode<TKey, TValue>(
					degree,
					false,
					comparer);

				newRoot.KeyValuePairs = new BPlusTreeMapKeyValuePair<TKey, TValue>[]
				{
					newNode.KeyValuePairs[0]
				};

				newRoot.Children = new BPlusTreeMapNode<TKey, TValue>[]
				{
					node,
					newNode
				};

				root = newRoot;
			}
			else
			{
				// Insert the middle key from the node into the parent

				BPlusTreeMapNode<TKey, TValue> parent = node.Parent;

				parent.Insert(
					newNode.KeyValuePairs[0].Key,
					newNode.KeyValuePairs[0].Value);
	
				// Manually handle the concatenation of arrays

				BPlusTreeMapNode<TKey, TValue>[] newChildren =
					new BPlusTreeMapNode<TKey, TValue>[parent.Children.Length + 1];

				Array.Copy(
					parent.Children,
					0,
					newChildren,
					0,
					parent.Children.Length);

				newChildren[parent.Children.Length] = newNode;

				parent.Children = newChildren;
	
				if (parent.IsOverflow())
				{
					Split(parent);
				}
			}
		}
	
		private void Rebalance(
			BPlusTreeMapNode<TKey, TValue> node)
		{
			// Handle underfilled nodes by either merging or borrowing keys

			BPlusTreeMapNode<TKey, TValue> parent = node.Parent;
			
			int indexInParent = Array.IndexOf(
				parent.Children,
				node);
	
			if (indexInParent > 0
				&& parent.Children[indexInParent - 1].KeysCount > (degree - 1) / 2)
			{
				BorrowFromLeft(
					node,
					parent,
					indexInParent);
			}
			else if (indexInParent < parent.KeysCount
				&& parent.Children[indexInParent + 1].KeysCount > (degree - 1) / 2)
			{
				BorrowFromRight(
					node,
					parent,
					indexInParent);
			}
			else
			{
				Merge(
					node,
					parent,
					indexInParent);
			}
		}
	
		private void BorrowFromLeft(
			BPlusTreeMapNode<TKey, TValue> node,
			BPlusTreeMapNode<TKey, TValue> parent,
			int indexInParent)
		{
			BPlusTreeMapNode<TKey, TValue> leftSibling = parent.Children[indexInParent - 1];

			BPlusTreeMapKeyValuePair<TKey, TValue> borrowedKeyValue = leftSibling.KeyValuePairs[--leftSibling.KeysCount];

			node.Insert(
				borrowedKeyValue.Key,
				borrowedKeyValue.Value);
		}
	
		private void BorrowFromRight(
			BPlusTreeMapNode<TKey, TValue> node,
			BPlusTreeMapNode<TKey, TValue> parent,
			int indexInParent)
		{
			BPlusTreeMapNode<TKey, TValue> rightSibling = parent.Children[indexInParent + 1];

			BPlusTreeMapKeyValuePair<TKey, TValue> borrowedKeyValue = rightSibling.KeyValuePairs[--rightSibling.KeysCount];

			node.Insert(
				borrowedKeyValue.Key,
				borrowedKeyValue.Value);
		}
	
		private void Merge(
			BPlusTreeMapNode<TKey, TValue> node,
			BPlusTreeMapNode<TKey, TValue> parent,
			int indexInParent)
		{
			// Merge the node with its sibling and adjust the parent's keys

			BPlusTreeMapNode<TKey, TValue> leftSibling = parent.Children[indexInParent - 1];

			BPlusTreeMapNode<TKey, TValue> rightSibling = parent.Children[indexInParent + 1];

			leftSibling.Insert(
				parent.KeyValuePairs[indexInParent].Key,
				parent.KeyValuePairs[indexInParent].Value);
			
			Array.Copy(
				rightSibling.KeyValuePairs,
				0,
				leftSibling.KeyValuePairs,
				leftSibling.KeysCount,
				rightSibling.KeysCount);

			leftSibling.KeysCount += rightSibling.KeysCount;

			parent.Remove(
				parent.KeyValuePairs[indexInParent].Key);
		}
	}
}