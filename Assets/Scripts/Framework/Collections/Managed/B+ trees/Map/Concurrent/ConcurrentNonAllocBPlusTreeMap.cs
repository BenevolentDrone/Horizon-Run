using System.Threading;
using System.Collections.Generic;

namespace HereticalSolutions.Collections.Managed
{
	public class ConcurrentNonAllocBPlusTreeMap<TKey, TValue>
		: IBPlusTreeMap<TKey, TValue>
	{
		private readonly NonAllocBPlusTreeMap<TKey, TValue> tree;

		private readonly SemaphoreSlim semaphoreSlim;

		public ConcurrentNonAllocBPlusTreeMap(
			NonAllocBPlusTreeMap<TKey, TValue> tree,
			SemaphoreSlim semaphoreSlim)
		{
			this.tree = tree;

			this.semaphoreSlim = semaphoreSlim;
		}

		#region IBPlusTree

		public bool Search(
			TKey key,
			out TValue value)
		{
			semaphoreSlim.Wait();

			try
			{
				return tree.Search(
					key,
					out value);
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		public void Insert(
			TKey key,
			TValue value)
		{
			semaphoreSlim.Wait();

			try
			{
				tree.Insert(
					key,
					value);
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		public bool Remove(
			TKey key)
		{
			semaphoreSlim.Wait();

			try
			{
				return tree.Remove(
					key);
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		public int Count
		{
			get
			{
				semaphoreSlim.Wait();

				try
				{
					return tree.Count;
				}
				finally
				{
					semaphoreSlim.Release();
				}
			}
		}

		public IEnumerable<TKey> AllKeys
		{
			get
			{
				semaphoreSlim.Wait();

				try
				{
					return tree.AllKeys;
				}
				finally
				{
					semaphoreSlim.Release();
				}
			}
		}

		public IEnumerable<TValue> AllValues
		{
			get
			{
				semaphoreSlim.Wait();

				try
				{
					return tree.AllValues;
				}
				finally
				{
					semaphoreSlim.Release();
				}
			}
		}

		public void Clear()
		{
			semaphoreSlim.Wait();

			try
			{
				tree.Clear();
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		#endregion
	}
}