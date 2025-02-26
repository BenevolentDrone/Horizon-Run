using System;
using System.Collections.Generic;
using System.Threading;

namespace HereticalSolutions.Repositories
{
	public class ConcurrentBPlusTreeRepository<TKey, TValue>
		: IRepository<TKey, TValue>
	{
		private readonly BPlusTreeRepository<TKey, TValue> repository;

		private readonly SemaphoreSlim semaphoreSlim;

		public ConcurrentBPlusTreeRepository(
			BPlusTreeRepository<TKey, TValue> repository,
			SemaphoreSlim semaphoreSlim)
		{
			//TODO: good idea, ChatGPT. Now I should add this to ALL consturctors
			this.repository = repository ?? throw new ArgumentNullException(nameof(repository));

			this.semaphoreSlim = semaphoreSlim ?? throw new ArgumentNullException(nameof(semaphoreSlim));
		}

		#region IRepository

		#region IReadOnlyRepository

		public bool Has(
			TKey key)
		{
			semaphoreSlim.Wait();

			try
			{
				return repository.Has(
					key);
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		public TValue Get(TKey key)
		{
			semaphoreSlim.Wait();

			try
			{
				return repository.Get(
					key);
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		public bool TryGet(
			TKey key,
			out TValue value)
		{
			semaphoreSlim.Wait();

			try
			{
				return repository.TryGet(
					key,
					out value);
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
					return repository.Count;
				}
				finally
				{
					semaphoreSlim.Release();
				}
			}
		}

		public IEnumerable<TKey> Keys
		{
			get
			{
				semaphoreSlim.Wait();

				try
				{
					return repository.Keys;
				}
				finally
				{
					semaphoreSlim.Release();
				}
			}
		}

		public IEnumerable<TValue> Values
		{
			get
			{
				semaphoreSlim.Wait();

				try
				{
					return repository.Values;
				}
				finally
				{
					semaphoreSlim.Release();
				}
			}
		}

		#endregion

		public TValue this[TKey key]
		{
			get
			{
				semaphoreSlim.Wait();

				try
				{
					return repository[key];
				}
				finally
				{
					semaphoreSlim.Release();
				}
			}
			set
			{
				semaphoreSlim.Wait();

				try
				{
					repository[key] = value;
				}
				finally
				{
					semaphoreSlim.Release();
				}
			}
		}

		public void Add(
			TKey key,
			TValue value)
		{
			semaphoreSlim.Wait();

			try
			{
				repository.Add(
					key,
					value);
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		public bool TryAdd(
			TKey key,
			TValue value)
		{
			semaphoreSlim.Wait();

			try
			{
				return repository.TryAdd(
					key,
					value);
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		public void Update(
			TKey key,
			TValue value)
		{
			semaphoreSlim.Wait();

			try
			{
				repository.Update(
					key,
					value);
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		public bool TryUpdate(
			TKey key,
			TValue value)
		{
			semaphoreSlim.Wait();

			try
			{
				return repository.TryUpdate(
					key,
					value);
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		public void AddOrUpdate(
			TKey key,
			TValue value)
		{
			semaphoreSlim.Wait();

			try
			{
				repository.AddOrUpdate(
					key,
					value);
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		public void Remove(
			TKey key)
		{
			semaphoreSlim.Wait();

			try
			{
				repository.Remove(
					key);
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		public bool TryRemove(
			TKey key)
		{
			semaphoreSlim.Wait();

			try
			{
				return repository.TryRemove(
					key);
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		public void Clear()
		{
			semaphoreSlim.Wait();

			try
			{
				repository.Clear();
			}
			finally
			{
				semaphoreSlim.Release();
			}
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			Clear();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			Clear();
		}

		#endregion
	}
}