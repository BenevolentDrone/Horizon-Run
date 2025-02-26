using System.Threading;
using System.Collections.Generic;

namespace HereticalSolutions.Collections.Managed
{
	public class ConcurrentBPlusTree<T>
		: IBPlusTree<T>
	{
		private readonly BPlusTree<T> tree;

		private readonly SemaphoreSlim semaphoreSlim;

		//private readonly object lockObject;

		public ConcurrentBPlusTree(
			BPlusTree<T> tree,
			SemaphoreSlim semaphoreSlim)
		{
			this.tree = tree;

			this.semaphoreSlim = semaphoreSlim;
		}

		#region IBPlusTree

		public bool Search(
			T key)
		{
			semaphoreSlim.Wait();

			try
			{
				return tree.Search(
					key);
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		//insert
		public void Insert(
			T key)
		{
			semaphoreSlim.Wait();

			try
			{
				tree.Insert(
					key);
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		//deleteKey
		public bool Remove(
			T key)
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

		public IEnumerable<T> All
		{
			get
			{
				semaphoreSlim.Wait();

				try
				{
					return tree.All;
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
