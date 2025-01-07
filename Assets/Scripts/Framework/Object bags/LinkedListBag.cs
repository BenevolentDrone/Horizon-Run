using System;

using System.Collections.Generic;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Bags
{
	public class LinkedListBag<T>
		: IBag<T>,
		  ICleanuppable,
		  IDisposable
	{
		private readonly LinkedList<T> bag;

		public LinkedListBag(
			LinkedList<T> bag)
		{
			this.bag = bag;
		}

		#region IBag

		public bool Push(
			T instance)
		{
			bag.AddLast(instance);

			return true;
		}

		public bool Pop(
			T instance)
		{
			return bag.Remove(instance);
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

		public void Clear()
		{
			bag.Clear();
		}

		#endregion
	}
}