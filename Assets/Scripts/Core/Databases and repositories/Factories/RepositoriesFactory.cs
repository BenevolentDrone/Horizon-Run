using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace HereticalSolutions.Repositories.Factories
{
    /// <summary>
    /// Represents a factory class for creating repositories.
    /// </summary>
    public static class RepositoriesFactory
    {
        #region Dictionary object repository

        /// <summary>
        /// Builds a new instance of <see cref="DictionaryObjectRepository"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="DictionaryObjectRepository"/>.</returns>
        public static DictionaryObjectRepository BuildDictionaryObjectRepository()
        {
            return new DictionaryObjectRepository(
                BuildDictionaryRepository<Type, object>());
        }
        
        /// <summary>
        /// Builds a new instance of <see cref="DictionaryObjectRepository"/> with the specified <paramref name="database"/>.
        /// </summary>
        /// <param name="database">The repository database.</param>
        /// <returns>A new instance of <see cref="DictionaryObjectRepository"/>.</returns>
        public static DictionaryObjectRepository BuildDictionaryObjectRepository(
            IRepository<Type, object> database)
        {
            return new DictionaryObjectRepository(
                database);
        }

        /// <summary>
        /// Clones an existing <see cref="IRepository{Type, object}"/> and creates a new instance of <see cref="DictionaryObjectRepository"/>.
        /// </summary>
        /// <param name="contents">The repository contents to clone.</param>
        /// <returns>A new instance of <see cref="DictionaryObjectRepository"/>.</returns>
        public static DictionaryObjectRepository CloneDictionaryObjectRepository(
            IRepository<Type, object> contents)
        {
            return new DictionaryObjectRepository(
                ((IClonableRepository<Type, object>)contents).Clone());
        }

        #endregion
        
        #region Dictionary repository
        
        /// <summary>
        /// Builds a new instance of <see cref="DictionaryRepository{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of the repository key.</typeparam>
        /// <typeparam name="TValue">The type of the repository value.</typeparam>
        /// <returns>A new instance of <see cref="DictionaryRepository{TKey, TValue}"/>.</returns>
        public static DictionaryRepository<TKey, TValue> BuildDictionaryRepository<TKey, TValue>()
        {
            return new DictionaryRepository<TKey, TValue>(
                new Dictionary<TKey, TValue>());
        }
        
        /// <summary>
        /// Builds a new instance of <see cref="DictionaryRepository{TKey, TValue}"/> with the specified <paramref name="database"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of the repository key.</typeparam>
        /// <typeparam name="TValue">The type of the repository value.</typeparam>
        /// <param name="database">The repository database.</param>
        /// <returns>A new instance of <see cref="DictionaryRepository{TKey, TValue}"/>.</returns>
        public static DictionaryRepository<TKey, TValue> BuildDictionaryRepository<TKey, TValue>(
            Dictionary<TKey, TValue> database)
        {
            return new DictionaryRepository<TKey, TValue>(
                database);
        }
        
        /// <summary>
        /// Builds a new instance of <see cref="DictionaryRepository{TKey, TValue}"/> with the specified <paramref name="comparer"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of the repository key.</typeparam>
        /// <typeparam name="TValue">The type of the repository value.</typeparam>
        /// <param name="comparer">The key comparer.</param>
        /// <returns>A new instance of <see cref="DictionaryRepository{TKey, TValue}"/>.</returns>
        public static DictionaryRepository<TKey, TValue> BuildDictionaryRepository<TKey, TValue>(
            IEqualityComparer<TKey> comparer)
        {
            return new DictionaryRepository<TKey, TValue>(
                new Dictionary<TKey, TValue>(comparer));
        }
        
        /// <summary>
        /// Clones an existing <see cref="Dictionary{TKey, TValue}"/> and creates a new instance of <see cref="DictionaryRepository{TKey, TValue}"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of the repository key.</typeparam>
        /// <typeparam name="TValue">The type of the repository value.</typeparam>
        /// <param name="contents">The repository contents to clone.</param>
        /// <returns>A new instance of <see cref="DictionaryRepository{TKey, TValue}"/>.</returns>
        public static DictionaryRepository<TKey, TValue> CloneDictionaryRepository<TKey, TValue>(
            Dictionary<TKey, TValue> contents)
        {
            return new DictionaryRepository<TKey, TValue>(
                new Dictionary<TKey, TValue>(contents));
        }

        #endregion

        #region Dictionary one to one map

        public static DictionaryOneToOneMap<TValue1, TValue2> BuildDictionaryOneToOneMap<TValue1, TValue2>()
        {
            return new DictionaryOneToOneMap<TValue1, TValue2>(
                new Dictionary<TValue1, TValue2>(),
                new Dictionary<TValue2, TValue1>());
        }
        
        public static DictionaryOneToOneMap<TValue1, TValue2> BuildDictionaryOneToOneMap<TValue1, TValue2>(
            Dictionary<TValue1, TValue2> leftDatabase,
            Dictionary<TValue2, TValue1> rightDatabase)
        {
            return new DictionaryOneToOneMap<TValue1, TValue2>(
                leftDatabase,
                rightDatabase);
        }
        
        public static DictionaryOneToOneMap<TValue1, TValue2> BuildDictionaryOneToOneMap<TValue1, TValue2>(
            IEqualityComparer<TValue1> leftComparer,
            IEqualityComparer<TValue2> rightComparer)
        {
            return new DictionaryOneToOneMap<TValue1, TValue2>(
                new Dictionary<TValue1, TValue2>(leftComparer),
                new Dictionary<TValue2, TValue1>(rightComparer));
        }

        #endregion
        
        #region Concurrent dictionary object repository

        public static ConcurrentDictionaryObjectRepository BuildConcurrentDictionaryObjectRepository()
        {
            return new ConcurrentDictionaryObjectRepository(
                new ConcurrentDictionary<Type, object>());
        }

        public static ConcurrentDictionaryObjectRepository BuildConcurrentDictionaryObjectRepository(
            ConcurrentDictionary<Type, object> database)
        {
            return new ConcurrentDictionaryObjectRepository(
                database);
        }

        public static ConcurrentDictionaryObjectRepository BuildConcurrentDictionaryObjectRepository(
            IEqualityComparer<Type> comparer)
        {
            return new ConcurrentDictionaryObjectRepository(
                new ConcurrentDictionary<Type, object>(comparer));
        }

        public static ConcurrentDictionaryObjectRepository CloneConcurrentDictionaryObjectRepository(
            ConcurrentDictionary<Type, object> contents)
        {
            return new ConcurrentDictionaryObjectRepository(
                new ConcurrentDictionary<Type, object>(contents));
        }

        #endregion

        #region Concurrent dictionary repository

        public static ConcurrentDictionaryRepository<TKey, TValue> BuildConcurrentDictionaryRepository<TKey, TValue>()
        {
            return new ConcurrentDictionaryRepository<TKey, TValue>(
                new ConcurrentDictionary<TKey, TValue>());
        }

        public static ConcurrentDictionaryRepository<TKey, TValue> BuildConcurrentDictionaryRepository<TKey, TValue>(
            ConcurrentDictionary<TKey, TValue> database)
        {
            return new ConcurrentDictionaryRepository<TKey, TValue>(
                database);
        }

        public static ConcurrentDictionaryRepository<TKey, TValue> BuildConcurrentDictionaryRepository<TKey, TValue>(
            IEqualityComparer<TKey> comparer)
        {
            return new ConcurrentDictionaryRepository<TKey, TValue>(
                new ConcurrentDictionary<TKey, TValue>(comparer));
        }

        public static ConcurrentDictionaryRepository<TKey, TValue> CloneConcurrentDictionaryRepository<TKey, TValue>(
            ConcurrentDictionary<TKey, TValue> contents)
        {
            return new ConcurrentDictionaryRepository<TKey, TValue>(
                new ConcurrentDictionary<TKey, TValue>(contents));
        }

        #endregion
    }
}