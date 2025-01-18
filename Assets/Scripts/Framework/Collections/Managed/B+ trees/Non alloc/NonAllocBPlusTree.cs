using System;
using System.Collections.Generic;

using HereticalSolutions.Pools;

namespace HereticalSolutions.Collections.Managed
{
	//Courtesy of https://www.geeksforgeeks.org/implementation-of-b-plus-tree-in-c/

	public class NonAllocBPlusTree<T>
		: IBPlusTree<T>
	{
		public static readonly IComparer<T> Comparer = Comparer<T>.Default;
		
		private readonly IPool<NonAllocBPlusTreeNode<T>> nodePool;

		private readonly int degree;

		private readonly int halfDegree;

		private NonAllocBPlusTreeNode<T> root;

		//createBTree
		public NonAllocBPlusTree(
			IPool<NonAllocBPlusTreeNode<T>> nodePool,
			int degree)
		{
			this.nodePool = nodePool;

			this.degree = degree;

			//Over here,
			//https://www.geeksforgeeks.org/insertion-in-a-b-tree/
			//the half-degree is used with ceil() for odd degrees

			//halfDegree = degree / 2;
			halfDegree = (int)Math.Ceiling((float)degree / 2);

			root = new NonAllocBPlusTreeNode<T>(
				degree,
				true);
		}

		#region IBPlusTree

		public bool Search(
			T key)
		{
			return Search(
				root,
				key);
		}

		//insert
		public void Insert(
			T key)
		{
			if (root.KeysCount == degree - 1)
			{
				NonAllocBPlusTreeNode<T> newRoot = AllocateNode(
					false);

				newRoot.Children[0] = root;

				Split(
					newRoot,
					root,
					0);

				Insert(
					newRoot,
					key);

				root = newRoot;
			}
			else
			{
				Insert(
					root,
					key);
			}
		}

		//deleteKey
		public bool Remove(
			T key)
		{
			// Call a helper function to delete the key recursively

			var result = DeleteKey(
				root,
				key);

			// If root has no keys left and it has a child, make its
			// first child the new root

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

				NonAllocBPlusTreeNode<T> current = root;

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
				NonAllocBPlusTreeNode<T> current = root;

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

		//createNode
		private NonAllocBPlusTreeNode<T> AllocateNode(
			bool leaf)
		{
			NonAllocBPlusTreeNode<T> newNode = nodePool.Pop();

			newNode.Initialize(
				degree,
				leaf);

			return newNode;
		}

		private void FreeNode(
			NonAllocBPlusTreeNode<T> node)
		{
			node.Cleanup();

			nodePool.Push(node);
		}

		#endregion

		#region CRUD operations

		#region Searches

		//findKey
		private int FindKeyFromStart(
			NonAllocBPlusTreeNode<T> node,
			T key)
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
			NonAllocBPlusTreeNode<T> node,
			T key)
		{
			int i = node.KeysCount - 1;

			while (i >= 0
				&& Comparer.Compare(
					node.Keys[i],
					key)
					> 0)
			{
				//node.Keys[i + 1] = node.Keys[i];

				i--;
			}

			i++;

			return i;
		}

		//search
		private bool Search(
			NonAllocBPlusTreeNode<T> node,
			T key)
		{
			int i = 0;

			while (i < node.KeysCount)
			{
				int compareResult = Comparer.Compare(
					key,
					node.Keys[i]);

				if (compareResult == 0)
				{
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
						return false;
					}

					return Search(
						node.Children[i],
						key);
				}
			}

			return false;
		}

		#endregion

		//insertNonFull
		private void Insert(
			NonAllocBPlusTreeNode<T> node,
			T key)
		{
			//int i = node.KeysCount - 1;

			if (node.IsLeaf)
			{
				//while (i >= 0
				//	&& Comparer.Compare(
				//		node.Keys[i],
				//		key)
				//		> 0)
				//{
				//	//node.Keys[i + 1] = node.Keys[i];
				//
				//	i--;
				//}
				//
				//Array.Copy(
				//	node.Keys,
				//	i + 1,
				//	node.Keys,
				//	i + 2,
				//	node.KeysCount - i - 1);
				//
				//node.Keys[i + 1] = key;

				int i = FindKeyFromFinish(
					node,
					key);

				Array.Copy(
					node.Keys,
					i,
					node.Keys,
					i + 1,
					node.KeysCount - i);

				node.Keys[i] = key;


				node.KeysCount++;
			}
			else
			{
				//while (i >= 0
				//	&& Comparer.Compare(
				//		node.Keys[i],
				//		key)
				//		> 0)
				//{
				//	i--;
				//}
				//
				//i++;

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
					key);
			}
		}

		//deleteKeyHelper
		private bool DeleteKey(
			NonAllocBPlusTreeNode<T> node,
			T key)
		{
			// Find the index of the key in the node

			int keyIndex = FindKeyFromStart(
				node,
				key);

			// If key is present in this node

			if (keyIndex < node.KeysCount
				&& Comparer.Compare(
					node.Keys[keyIndex],
					key) == 0)
			{
				if (node.IsLeaf)
				{
					// If the node is a leaf, simply remove the key

					RemoveFromLeaf(
						node,
						keyIndex);

					return true;
				}

				// If the node is not a leaf, replace the key
				// with its predecessor/successor

				T predecessor = GetPredecessor(
					node,
					keyIndex);

				node.Keys[keyIndex] = predecessor;

				// Recursively delete the predecessor
				DeleteKey(
					node.Children[keyIndex],
					predecessor);

				return true;
			}

			// If the key is not present in this node, go down
			// the appropriate child

			if (node.IsLeaf)
			{
				// Key not found in the tree

				return false;
			}

			bool isLastChild = (keyIndex == node.KeysCount);

			// If the child where the key is supposed to be lies
			// has less than t keys, fill that child

			if (node.Children[keyIndex].KeysCount < halfDegree)
			{
				Rebalance(
					node,
					keyIndex);
			}

			// If the last child has been merged, it must have
			// merged with the previous child

			// So, we need to recursively delete the key from
			// the previous child

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

		//removeFromLeaf
		private void RemoveFromLeaf(
			NonAllocBPlusTreeNode<T> node,
			int keyIndex)
		{
			//for (int i = keyIndex + 1; i < node.KeysCount; ++i)
			//{
			//	node.Keys[i - 1] = node.Keys[i];
			//}

			Array.Copy(
				node.Keys,
				keyIndex + 1,
				node.Keys,
				keyIndex,
				node.KeysCount - keyIndex - 1);

			node.KeysCount--;
		}

		//getPredecessor
		private T GetPredecessor(
			NonAllocBPlusTreeNode<T> node,
			int keyIndex)
		{
			NonAllocBPlusTreeNode<T> current = node.Children[keyIndex];

			while (!current.IsLeaf)
			{
				current = current.Children[current.KeysCount];
			}

			return current.Keys[current.KeysCount - 1];
		}

		#endregion

		#region Splits and rebalances

		//splitChild
		private void Split(
			NonAllocBPlusTreeNode<T> parent,
			NonAllocBPlusTreeNode<T> node,
			int i)
		{
			NonAllocBPlusTreeNode<T> newSibling = AllocateNode(
				node.IsLeaf);

			newSibling.KeysCount = halfDegree - 1;

			//for (int j = 0; j < t - 1; j++)
			//{
			//	newChild.Keys[j] = child.Keys[j + t];
			//}

			Array.Copy(
				node.Keys,
				halfDegree,
				newSibling.Keys,
				0,
				halfDegree - 1);

			if (!node.IsLeaf)
			{
				//for (int j = 0; j < t; j++)
				//{
				//	newChild.Children[j] = child.Children[j + t];
				//}

				Array.Copy(
					node.Children,
					halfDegree,
					newSibling.Children,
					0,
					halfDegree);
			}

			node.KeysCount = halfDegree - 1;

			//We'll trust this one with the safety of the shift operation
			//https://stackoverflow.com/questions/11149668/is-array-copy-safe-when-the-source-and-destination-are-the-same-array

			//for (int j = parent.KeysCount; j >= i + 1; j--)
			//{
			//	parent.Children[j + 1] = parent.Children[j];
			//}

			Array.Copy(
				parent.Children,
				i + 1,
				parent.Children,
				i + 2,
				parent.KeysCount - i);

			parent.Children[i + 1] = newSibling;

			//for (int j = parent.KeysCount - 1; j >= i; j--)
			//{
			//	parent.Keys[j + 1] = parent.Keys[j];
			//}

			Array.Copy(
				parent.Keys,
				i,
				parent.Keys,
				i + 1,
				parent.KeysCount - i);

			parent.Keys[i] = node.Keys[halfDegree - 1];

			parent.KeysCount++;
		}

		//fill
		private void Rebalance(
			NonAllocBPlusTreeNode<T> node,
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

		//borrowFromPrev
		private void BorrowFromPrev(
			NonAllocBPlusTreeNode<T> node,
			int keyIndex)
		{
			NonAllocBPlusTreeNode<T> child = node.Children[keyIndex];

			NonAllocBPlusTreeNode<T> sibling = node.Children[keyIndex - 1];

			// Move all keys in child one step ahead

			//for (int i = child.KeysCount - 1; i >= 0; --i)
			//{
			//	child.Keys[i + 1] = child.Keys[i];
			//}

			Array.Copy(
				child.Keys,
				0,
				child.Keys,
				1,
				child.KeysCount);

			// If child is not a leaf, move its child pointers one
			// step ahead

			if (!child.IsLeaf)
			{
				//for (int i = child.KeysCount; i >= 0; --i)
				//{
				//	child.Children[i + 1] = child.Children[i];
				//}

				Array.Copy(
					child.Children,
					0,
					child.Children,
					1,
					child.KeysCount + 1);
			}

			// Setting child's first key equal to node's key[keyIndex -
			// 1]

			child.Keys[0] = node.Keys[keyIndex - 1];

			// Moving sibling's last child as child's first child

			if (!child.IsLeaf)
			{
				child.Children[0] = sibling.Children[sibling.KeysCount];
			}

			// Moving the key from the sibling to the parent

			node.Keys[keyIndex - 1] = sibling.Keys[sibling.KeysCount - 1];

			// Incrementing and decrementing the key counts of child
			// and sibling respectively

			child.KeysCount++;

			sibling.KeysCount--;
		}

		//borrowFromNext
		private void BorrowFromNext(
			NonAllocBPlusTreeNode<T> node,
			int keyIndex)
		{
			NonAllocBPlusTreeNode<T> child = node.Children[keyIndex];

			NonAllocBPlusTreeNode<T> sibling = node.Children[keyIndex + 1];

			// Setting child's (t - 1)th key equal to node's
			// key[keyIndex]

			child.Keys[child.KeysCount] = node.Keys[keyIndex];

			// If child is not a leaf, move its child pointers one
			// step ahead

			if (!child.IsLeaf)
			{
				child.Children[(child.KeysCount) + 1]
					= sibling.Children[0];
			}

			// Setting node's keyIndex-th key equal to sibling's first
			// key

			node.Keys[keyIndex] = sibling.Keys[0];

			// Moving all keys in sibling one step behind

			//for (int i = 1; i < sibling.KeysCount; ++i)
			//{
			//	sibling.Keys[i - 1] = sibling.Keys[i];
			//}

			Array.Copy(
				sibling.Keys,
				1,
				sibling.Keys,
				0,
				sibling.KeysCount - 1);

			// If sibling is not a leaf, move its child pointers one
			// step behind

			if (!sibling.IsLeaf)
			{
				//for (int i = 1; i <= sibling.KeysCount; ++i)
				//{
				//	sibling.Children[i - 1] = sibling.Children[i];
				//}

				Array.Copy(
					sibling.Children,
					1,
					sibling.Children,
					0,
					sibling.KeysCount);
			}

			// Incrementing and decrementing the key counts of child
			// and sibling respectively

			child.KeysCount++;

			sibling.KeysCount--;
		}

		//merge
		private void Merge(
			NonAllocBPlusTreeNode<T> node,
			int keyIndex)
		{
			NonAllocBPlusTreeNode<T> child = node.Children[keyIndex];

			NonAllocBPlusTreeNode<T> sibling = node.Children[keyIndex + 1];

			// Pulling a key from the current node and inserting it
			// into (t-1)th position of child

			child.Keys[child.KeysCount] = node.Keys[keyIndex];

			// If child is not a leaf, move its child pointers one
			// step ahead

			if (!child.IsLeaf)
			{
				child.Children[child.KeysCount + 1]
					= sibling.Children[0];
			}

			// Copying the keys from sibling to child

			//for (int i = 0; i < sibling.KeysCount; ++i)
			//{
			//	child.Keys[i + child.KeysCount + 1] = sibling.Keys[i];
			//}

			Array.Copy(
				sibling.Keys,
				0,
				child.Keys,
				child.KeysCount + 1,
				sibling.KeysCount);

			// If child is not a leaf, copy the children pointers as
			// well

			if (!child.IsLeaf)
			{
				//for (int i = 0; i <= sibling.KeysCount; ++i)
				//{
				//	child.Children[i + child.KeysCount + 1]
				//		= sibling.Children[i];
				//}

				Array.Copy(
					sibling.Children,
					0,
					child.Children,
					child.KeysCount + 1,
					sibling.KeysCount + 1);
			}

			// Update the key count of child and current node

			child.KeysCount += sibling.KeysCount + 1;


			// Move all keys after keyIndex in the current node one step
			// before, so as to fill the gap created by moving
			// keys[keyIndex] to child

			//for (int i = keyIndex + 1; i < node.KeysCount; ++i)
			//{
			//	node.Keys[i - 1] = node.Keys[i];
			//}

			Array.Copy(
				node.Keys,
				keyIndex + 1,
				node.Keys,
				keyIndex,
				node.KeysCount - keyIndex - 1);

			// Move the child pointers after (keyIndex + 1) in the
			// current node one step before

			//for (int i = keyIndex + 2; i <= node.KeysCount; ++i)
			//{
			//	node.Children[i - 1] = node.Children[i];
			//}

			Array.Copy(
				node.Children,
				keyIndex + 2,
				node.Children,
				keyIndex + 1,
				node.KeysCount - keyIndex - 1);

			node.KeysCount--;

			// Free the memory occupied by sibling
			FreeNode(sibling);
		}

		#endregion

		#region Cleanup

		public void RecursiveClear(
			NonAllocBPlusTreeNode<T> node)
		{
			if (node.Children != null)
			{
				for (int i = 0; i < node.Children.Length; i++)
				{
					RecursiveClear(
						node.Children[i]);
				}
			}

			//node.Cleanup();

			FreeNode(
				node);
		}

		#endregion

		#endregion
	}
}