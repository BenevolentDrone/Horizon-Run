using System;
using System.Collections.Generic;

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Repositories
{
	public class ConcurrentBPlusTreeRepository<TKey, TValue>
		: IRepository<TKey, TValue>
	{
		private readonly IBPlusTreeMap<TKey, TValue> treeMap;

		private readonly object lockObject;

		private readonly ILogger logger;
		
		public ConcurrentBPlusTreeRepository(
			IBPlusTreeMap<TKey, TValue> treeMap,
			ILogger logger = null)
		{
			//TODO: good idea, ChatGPT. Now I should add this to ALL consturctors
			this.treeMap = treeMap ?? throw new ArgumentNullException(nameof(treeMap));

			this.logger = logger;


			lockObject = new object();
		}

		#region IRepository

		#region IReadOnlyRepository

		public bool Has(
			TKey key)
		{
			lock (lockObject)
			{
				return treeMap.Search(
					key,
					out _);
			}
		}

		public TValue Get(TKey key)
		{
			lock (lockObject)
			{
				if (!treeMap.Search(
					key,
					out var value))
				{
					throw new KeyNotFoundException(
						logger?.TryFormatException(
							GetType(),
							$"KEY NOT FOUND: {key}"));
				}

				return value;
			}
		}

		public bool TryGet(
			TKey key,
			out TValue value)
		{
			lock (lockObject)
			{
				return treeMap.Search(
					key,
					out value);
			}
		}

		public int Count
		{
			get
			{
				lock (lockObject)
				{
					return treeMap.Count;
				}
			}
		}

		public IEnumerable<TKey> Keys
		{
			get
			{
				lock (lockObject)
				{
					return treeMap.AllKeys;
				}
			}
		}

		public IEnumerable<TValue> Values
		{
			get
			{
				lock (lockObject)
				{
					return treeMap.AllValues;
				}
			}
		}

		#endregion

		public TValue this[TKey key]
		{
			get
			{
				lock (lockObject)
				{
					if (!treeMap.Search(
						key,
						out var value))
					{
						throw new KeyNotFoundException(
							logger?.TryFormatException(
								GetType(),
								$"KEY NOT FOUND: {key}"));
					}

					return value;
				}
			}
			set
			{
				lock (lockObject)
				{
					treeMap.Insert(
						key,
						value);
				}
			}
		}

		public void Add(
			TKey key,
			TValue value)
		{
			lock (lockObject)
			{
				treeMap.Insert(
					key,
					value);
			}
		}

		public bool TryAdd(
			TKey key,
			TValue value)
		{
			lock (lockObject)
			{
				if (!treeMap.Search(
					key,
					out _))
				{
					treeMap.Insert(
						key,
						value);

					return true;
				}

				return false;
			}
		}

		public void Update(
			TKey key,
			TValue value)
		{
			lock (lockObject)
			{
				if (!treeMap.Remove(key))
				{
					throw new KeyNotFoundException(
						logger?.TryFormatException(
							GetType(),
							$"KEY NOT FOUND: {key}"));
				}

				treeMap.Insert(
					key,
					value);
			}
		}

		public bool TryUpdate(
			TKey key,
			TValue value)
		{
			lock (lockObject)
			{
				if (treeMap.Remove(key))
				{
					treeMap.Insert(
						key,
						value);

					return true;
				}

				return false;
			}
		}

		public void AddOrUpdate(
			TKey key,
			TValue value)
		{
			lock (lockObject)
			{
				treeMap.Insert(
					key,
					value);
			}
		}

		public void Remove(
			TKey key)
		{
			lock (lockObject)
			{
				if (!treeMap.Remove(key))
					throw new KeyNotFoundException(
						logger?.TryFormatException(
							GetType(),
							$"KEY NOT FOUND: {key}"));
			}
		}

		public bool TryRemove(
			TKey key)
		{
			lock (lockObject)
			{
				return treeMap.Remove(key);
			}
		}

		public void Clear()
		{
			lock (lockObject)
			{
				treeMap.Clear();
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