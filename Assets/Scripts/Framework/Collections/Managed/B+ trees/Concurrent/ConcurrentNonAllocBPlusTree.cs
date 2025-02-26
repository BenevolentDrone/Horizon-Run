using System.Threading;
using System.Collections.Generic;

namespace HereticalSolutions.Collections.Managed
{
	//Courtesy of https://www.geeksforgeeks.org/implementation-of-b-plus-tree-in-c/

	public class ConcurrentNonAllocBPlusTree<T>
		: IBPlusTree<T>
	{
		private readonly NonAllocBPlusTree<T> tree;

		private readonly SemaphoreSlim semaphoreSlim;
		
		public ConcurrentNonAllocBPlusTree(
			NonAllocBPlusTree<T> tree,
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