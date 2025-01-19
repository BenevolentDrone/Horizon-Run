using System;
using System.Collections.Generic;

using HereticalSolutions.Collections.Managed;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Repositories
{
	public class BPlusTreeRepository<TKey, TValue>
		: IRepository<TKey, TValue>
	{
		private readonly IBPlusTreeMap<TKey, TValue> treeMap;

		private readonly ILogger logger;

		public BPlusTreeRepository(
			IBPlusTreeMap<TKey, TValue> treeMap,
			ILogger logger)
		{
			//TODO: good idea, ChatGPT. Now I should add this to ALL consturctors
			this.treeMap = treeMap ?? throw new ArgumentNullException(nameof(treeMap));

			this.logger = logger;
		}

		#region IRepository

		#region IReadOnlyRepository

		public bool Has(
			TKey key)
		{
			return treeMap.Search(
				key,
				out _);
		}

		public TValue Get(
			TKey key)
		{
			if (!treeMap.Search(
				key,
				out var value))
			{
				throw new KeyNotFoundException(
					logger.TryFormatException(
						GetType(),
						$"KEY NOT FOUND: {key}"));
			}

			return value;
		}

		public bool TryGet(
			TKey key,
			out TValue value)
		{
			return treeMap.Search(
				key,
				out value);
		}

		public int Count => treeMap.Count;

		public IEnumerable<TKey> Keys => treeMap.AllKeys;

		public IEnumerable<TValue> Values => treeMap.AllValues;

		#endregion

		public TValue this[TKey key]
		{
			get
			{
				if (!treeMap.Search(
					key,
					out var value))
				{
					throw new KeyNotFoundException(
						logger.TryFormatException(
							GetType(),
							$"KEY NOT FOUND: {key}"));
				}

				return value;
			}
			set
			{
				treeMap.Insert(
					key,
					value);
			}
		}

		public void Add(
			TKey key,
			TValue value)
		{
			treeMap.Insert(
				key,
				value);
		}

		public bool TryAdd(
			TKey key,
			TValue value)
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

		public void Update(
			TKey key,
			TValue value)
		{
			if (!treeMap.Remove(key))
			{
				throw new KeyNotFoundException(
					logger.TryFormatException(
						GetType(),
						$"KEY NOT FOUND: {key}"));
			}

			treeMap.Insert(
				key,
				value);
		}

		public bool TryUpdate(
			TKey key,
			TValue value)
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

		public void AddOrUpdate(
			TKey key,
			TValue value)
		{
			treeMap.Insert(
				key,
				value);
		}

		public void Remove(
			TKey key)
		{
			if (!treeMap.Remove(key))
				throw new KeyNotFoundException(
					logger.TryFormatException(
						GetType(),
						$"KEY NOT FOUND: {key}"));
		}

		public bool TryRemove(
			TKey key)
		{
			return treeMap.Remove(key);
		}

		public void Clear()
		{
			treeMap.Clear();
		}

		#endregion

		//#region IClonableRepository
		//
		//public IRepository<TKey, TValue> Clone()
		//{
		//	return RepositoriesFactory.CloneBPlusTreeRepository(
		//		treeMap);
		//}
		//
		//#endregion

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