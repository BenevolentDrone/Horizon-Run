using System;

using System.Collections.Generic;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Bags
{
	public class ConcurrentLinkedListBag<T>
		: IBag<T>,
		  ICleanuppable,
		  IDisposable
	{
		private readonly LinkedList<T> bag;

		private readonly object lockObject;

		public ConcurrentLinkedListBag(
			LinkedList<T> bag)
		{
			this.bag = bag;


			lockObject = new object();
		}

		#region IBag

		public bool Push(
			T instance)
		{
			lock (lockObject)
			{
				bag.AddLast(instance);

				return true;
			}
		}

		public bool Pop(
			T instance)
		{
			lock (lockObject)
			{
				return bag.Remove(instance);
			}
		}

		public int Count
		{
			get
			{
				lock (lockObject)
				{
					return bag.Count;
				}
			}
		}

		public IEnumerable<T> All
		{
			get
			{
				lock (lockObject)
				{
					var current = bag.First;

					while (current != null)
					{
						yield return current.Value;

						current = current.Next;
					}
				}
			}
		}

		public void Clear()
		{
			lock (lockObject)
			{
				bag.Clear();
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

		#endregion
	}
}