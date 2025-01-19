using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Repositories.Factories
{
    public static class RepositoryFactory
    {
        #region Dictionary object repository

        public static DictionaryInstanceRepository BuildDictionaryInstanceRepository()
        {
            return new DictionaryInstanceRepository(
                BuildDictionaryRepository<Type, object>());
        }
        
        public static DictionaryInstanceRepository BuildDictionaryInstanceRepository(
            IRepository<Type, object> database)
        {
            return new DictionaryInstanceRepository(
                database);
        }

        public static DictionaryInstanceRepository CloneDictionaryInstanceRepository(
            IRepository<Type, object> contents)
        {
            return new DictionaryInstanceRepository(
                ((IClonableRepository<Type, object>)contents).Clone());
        }

        #endregion
        
        #region Dictionary repository
        
        public static DictionaryRepository<TKey, TValue> BuildDictionaryRepository<TKey, TValue>()
        {
            return new DictionaryRepository<TKey, TValue>(
                new Dictionary<TKey, TValue>());
        }
        
        public static DictionaryRepository<TKey, TValue> BuildDictionaryRepository<TKey, TValue>(
            Dictionary<TKey, TValue> database)
        {
            return new DictionaryRepository<TKey, TValue>(
                database);
        }
        
        public static DictionaryRepository<TKey, TValue> BuildDictionaryRepository<TKey, TValue>(
            IEqualityComparer<TKey> comparer)
        {
            return new DictionaryRepository<TKey, TValue>(
                new Dictionary<TKey, TValue>(comparer));
        }
        
        public static DictionaryRepository<TKey, TValue> CloneDictionaryRepository<TKey, TValue>(
            Dictionary<TKey, TValue> contents)
        {
            return new DictionaryRepository<TKey, TValue>(
                new Dictionary<TKey, TValue>(contents));
        }

        #endregion

        #region Dictionary one to one map

        public static DictionaryOneToOneMap<TValue1, TValue2> BuildDictionaryOneToOneMap<TValue1, TValue2>(
            ILoggerResolver loggerResolver)
        {
            ILogger logger = loggerResolver?.GetLogger<DictionaryOneToOneMap<TValue1, TValue2>>();

            return new DictionaryOneToOneMap<TValue1, TValue2>(
                new Dictionary<TValue1, TValue2>(),
                new Dictionary<TValue2, TValue1>(),
                logger);
        }
        
        public static DictionaryOneToOneMap<TValue1, TValue2> BuildDictionaryOneToOneMap<TValue1, TValue2>(
            Dictionary<TValue1, TValue2> leftDatabase,
            Dictionary<TValue2, TValue1> rightDatabase,
            ILoggerResolver loggerResolver)
        {
            ILogger logger = loggerResolver?.GetLogger<DictionaryOneToOneMap<TValue1, TValue2>>();

            return new DictionaryOneToOneMap<TValue1, TValue2>(
                leftDatabase,
                rightDatabase,
                logger);
        }
        
        public static DictionaryOneToOneMap<TValue1, TValue2> BuildDictionaryOneToOneMap<TValue1, TValue2>(
            IEqualityComparer<TValue1> leftComparer,
            IEqualityComparer<TValue2> rightComparer,
            ILoggerResolver loggerResolver)
        {
            ILogger logger = loggerResolver?.GetLogger<DictionaryOneToOneMap<TValue1, TValue2>>();

            return new DictionaryOneToOneMap<TValue1, TValue2>(
                new Dictionary<TValue1, TValue2>(leftComparer),
                new Dictionary<TValue2, TValue1>(rightComparer),
                logger);
        }

        public static DictionaryOneToOneMap<TValue1, TValue2> CloneDictionaryOneToOneMap<TValue1, TValue2>(
            Dictionary<TValue1, TValue2> leftToRightDatabase,
            Dictionary<TValue2, TValue1> rightToLeftDatabase,
            ILogger logger)
        {
            return new DictionaryOneToOneMap<TValue1, TValue2>(
                new Dictionary<TValue1, TValue2>(
                    leftToRightDatabase,
                    leftToRightDatabase.Comparer),
                new Dictionary<TValue2, TValue1>(
                    rightToLeftDatabase,
                    rightToLeftDatabase.Comparer),
                logger);
        }

        #endregion

        #region Concurrent dictionary object repository

        public static ConcurrentDictionaryInstanceRepository BuildConcurrentDictionaryInstanceRepository()
        {
            return new ConcurrentDictionaryInstanceRepository(
                new ConcurrentDictionary<Type, object>());
        }

        public static ConcurrentDictionaryInstanceRepository BuildConcurrentDictionaryInstanceRepository(
            ConcurrentDictionary<Type, object> database)
        {
            return new ConcurrentDictionaryInstanceRepository(
                database);
        }

        public static ConcurrentDictionaryInstanceRepository BuildConcurrentDictionaryInstanceRepository(
            IEqualityComparer<Type> comparer)
        {
            return new ConcurrentDictionaryInstanceRepository(
                new ConcurrentDictionary<Type, object>(comparer));
        }

        public static ConcurrentDictionaryInstanceRepository CloneConcurrentDictionaryInstanceRepository(
            ConcurrentDictionary<Type, object> contents)
        {
            return new ConcurrentDictionaryInstanceRepository(
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

        #region Concurrent dictionary one to one map

        public static ConcurrentDictionaryOneToOneMap<TValue1, TValue2>
            BuildConcurrentDictionaryOneToOneMap<TValue1, TValue2>(
            ILoggerResolver loggerResolver)
        {
            ILogger logger = loggerResolver?.GetLogger<ConcurrentDictionaryOneToOneMap<TValue1, TValue2>>();

            return new ConcurrentDictionaryOneToOneMap<TValue1, TValue2>(
                new Dictionary<TValue1, TValue2>(),
                new Dictionary<TValue2, TValue1>(),
                logger);
        }

        public static ConcurrentDictionaryOneToOneMap<TValue1, TValue2>
            BuildConcurrentDictionaryOneToOneMap<TValue1, TValue2>(
            Dictionary<TValue1, TValue2> leftDatabase,
            Dictionary<TValue2, TValue1> rightDatabase,
            ILoggerResolver loggerResolver)
        {
            ILogger logger = loggerResolver?.GetLogger<ConcurrentDictionaryOneToOneMap<TValue1, TValue2>>();

            return new ConcurrentDictionaryOneToOneMap<TValue1, TValue2>(
                leftDatabase,
                rightDatabase,
                logger);
        }

        public static ConcurrentDictionaryOneToOneMap<TValue1, TValue2>
            BuildConcurrentDictionaryOneToOneMap<TValue1, TValue2>(
            IEqualityComparer<TValue1> leftComparer,
            IEqualityComparer<TValue2> rightComparer,
            ILoggerResolver loggerResolver)
        {
            ILogger logger = loggerResolver?.GetLogger<ConcurrentDictionaryOneToOneMap<TValue1, TValue2>>();

            return new ConcurrentDictionaryOneToOneMap<TValue1, TValue2>(
                new Dictionary<TValue1, TValue2>(leftComparer),
                new Dictionary<TValue2, TValue1>(rightComparer),
                logger);
        }

        public static ConcurrentDictionaryOneToOneMap<TValue1, TValue2>
            CloneConcurrentDictionaryOneToOneMap<TValue1, TValue2>(
            Dictionary<TValue1, TValue2> leftToRightDatabase,
            Dictionary<TValue2, TValue1> rightToLeftDatabase,
            ILogger logger)
        {
            return new ConcurrentDictionaryOneToOneMap<TValue1, TValue2>(
                new Dictionary<TValue1, TValue2>(
                    leftToRightDatabase,
                    leftToRightDatabase.Comparer),
                new Dictionary<TValue2, TValue1>(
                    rightToLeftDatabase,
                    rightToLeftDatabase.Comparer),
                logger);
        }

        #endregion
    }
}