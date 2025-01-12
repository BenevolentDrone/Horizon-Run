using System;
using System.Collections.Generic;

namespace HereticalSolutions.Collections.Managed
{
	public class BPlusTree<T>
		: IBPlusTree<T>
	{
		private readonly IComparer<T> comparer;

		private BPlusTreeNode<T> root;

		private int degree;

		public BPlusTree(
			int degree)
		{
			comparer = Comparer<T>.Default;

			this.degree = degree;

			root = new BPlusTreeNode<T>(
				degree,
				true,
				comparer);
		}

		#region IBPlusTree

		public void Insert(
			T key)
		{
			BPlusTreeNode<T> node = root;

			// Find the appropriate leaf node where the key should be inserted

			while (!node.IsLeaf)
			{
				node = node.GetChildNode(
					key);
			}

			// Insert the key in the leaf node

			node.Insert(
				key);

			// Split the node if it exceeds the maximum number of keys

			if (node.IsOverflow())
			{
				Split(node);
			}
		}

		public bool Search(
			T key)
		{
			BPlusTreeNode<T> node = root;

			// Traverse the tree to find the leaf node containing the key

			while (!node.IsLeaf)
			{
				node = node.GetChildNode(
					key);
			}

			// Check if the key is present in the leaf node

			return node.Contains(
				key);
		}

		public bool Remove(
			T key)
		{
			BPlusTreeNode<T> node = root;

			// Traverse the tree to find the leaf node containing the key

			while (!node.IsLeaf)
			{
				node = node.GetChildNode(
					key);
			}

			// Remove the key from the leaf node

			bool removed = node.Remove(
				key);

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

				BPlusTreeNode<T> current = root;

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

		public IEnumerable<T> All
		{
			get
			{
				BPlusTreeNode<T> current = root;

				// Traverse down to the first leaf node

				while (!current.IsLeaf)
				{
					current = current.Children[0];  // Go to the leftmost child
				}

				// Traverse through the leaf nodes and yield keys

				while (current != null)
				{
					foreach (var key in current.Keys)
					{
						yield return key;
					}

					current = current.Next;  // Move to the next leaf node
				}
			}
		}

		public void InOrderTraversal(
			Action<T> action)
		{
			InOrderTraversal(
				root,
				action);
		}

		public void Clear()
		{
			root.RecursiveClear();

			root = new BPlusTreeNode<T>(
				degree,
				true,
				comparer);
		}

		#endregion

		private void InOrderTraversal(
			BPlusTreeNode<T> node,
			Action<T> action)
		{
			if (node.IsLeaf)
			{
				var current = node;

				while (current != null)
				{
					foreach (var key in current.Keys)
					{
						action(key);
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

		private void Split(
			BPlusTreeNode<T> node)
		{
			// Create a new node to split the current node

			BPlusTreeNode<T> newNode = new BPlusTreeNode<T>(
				degree,
				true,
				comparer);

			int midIndex = node.KeysCount / 2;

			// Transfer keys and children to the new node

			newNode.Keys = new T[degree - 1];

			Array.Copy(
				node.Keys,
				midIndex,
				newNode.Keys,
				0,
				node.KeysCount - midIndex);

			node.KeysCount = midIndex;

			if (node.Children != null)
			{
				newNode.Children = new BPlusTreeNode<T>[degree];

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
						midIndex
					).ToArray();
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
				BPlusTreeNode<T> newRoot = new BPlusTreeNode<T>(
					degree,
					false,
					comparer);

				newRoot.Keys = new T[]
				{
					newNode.Keys[0]
				};

				newRoot.Children = new BPlusTreeNode<T>[]
				{
					node,
					newNode
				};

				root = newRoot;
			}
			else
			{
				// Insert the middle key from the node into the parent
				BPlusTreeNode<T> parent = node.Parent;

				parent.Insert(
					newNode.Keys[0]);

				//LINQ
				//parent.Children = parent.Children.Concat(new[] { newNode }).ToArray();

				// Manually handle the concatenation of arrays

				BPlusTreeNode<T>[] newChildren = new BPlusTreeNode<T>[parent.Children.Length + 1];

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
			BPlusTreeNode<T> node)
		{
			// Handle underfilled nodes by either merging or borrowing keys
			BPlusTreeNode<T> parent = node.Parent;

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
			BPlusTreeNode<T> node,
			BPlusTreeNode<T> parent,
			int indexInParent)
		{
			BPlusTreeNode<T> leftSibling = parent.Children[indexInParent - 1];

			T borrowedKey = leftSibling.Keys[--leftSibling.KeysCount];

			node.Insert(
				borrowedKey);
		}

		private void BorrowFromRight(
			BPlusTreeNode<T> node,
			BPlusTreeNode<T> parent,
			int indexInParent)
		{
			BPlusTreeNode<T> rightSibling = parent.Children[indexInParent + 1];

			T borrowedKey = rightSibling.Keys[--rightSibling.KeysCount];

			node.Insert(
				borrowedKey);
		}

		private void Merge(
			BPlusTreeNode<T> node,
			BPlusTreeNode<T> parent,
			int indexInParent)
		{
			// Merge the node with its sibling and adjust the parent's keys

			BPlusTreeNode<T> leftSibling = parent.Children[indexInParent - 1];

			BPlusTreeNode<T> rightSibling = parent.Children[indexInParent + 1];

			leftSibling.Insert(
				parent.Keys[indexInParent]);

			Array.Copy(
				rightSibling.Keys,
				0,
				leftSibling.Keys,
				leftSibling.KeysCount,
				rightSibling.KeysCount);

			leftSibling.KeysCount += rightSibling.KeysCount;

			parent.Remove(
				parent.Keys[indexInParent]);
		}
	}	
}