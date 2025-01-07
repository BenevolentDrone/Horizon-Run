using System;

using System.Collections.Generic;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Pools;

namespace HereticalSolutions.Bags
{
	public class NonAllocLinkedListBag<T>
		: IBag<T>,
		  ICleanuppable,
		  IDisposable
	{
		private readonly LinkedList<T> bag;

		private readonly IPool<LinkedListNode<T>> nodePool;

		public NonAllocLinkedListBag(
			LinkedList<T> bag,
			IPool<LinkedListNode<T>> nodePool)
		{
			this.bag = bag;

			this.nodePool = nodePool;
		}

		#region IBag

		public bool Push(
			T instance)
		{
			var node = nodePool.Pop();

			node.Value = instance;

			bag.AddLast(node);

			return true;
		}

		public bool Pop(
			T instance)
		{
			var node = bag.Find(instance);

			if (node == null)
				return false;

			bag.Remove(node);

			node.Value = default(T);

			nodePool.Push(node);

			return true;
		}

		public int Count { get => bag.Count; }

		public IEnumerable<T> All
		{
			get
			{
				var current = bag.First;

				while (current != null)
				{
					yield return current.Value;

					current = current.Next;
				}
			}
		}

		public void Clear()
		{
			bag.Clear();
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			foreach (var item in bag)
				if (item is ICleanuppable)
					(item as ICleanuppable).Cleanup();

			bag.Clear();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			foreach (var item in bag)
				if (item is IDisposable)
					(item as IDisposable).Dispose();

			bag.Clear();
		}

		#endregion
	}
}