using System;
using System.Collections.Generic;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Repositories
{
    public class DictionaryOneToOneMap<TValue1, TValue2> :
        IOneToOneMap<TValue1, TValue2>,
        ICleanUppable,
        IDisposable
    {
        private readonly Dictionary<TValue1, TValue2> leftToRightDatabase;
        
        private readonly Dictionary<TValue2, TValue1> rightToLeftDatabase;

        private readonly ILogger logger;

        public DictionaryOneToOneMap(
            Dictionary<TValue1, TValue2> leftToRightDatabase,
            Dictionary<TValue2, TValue1> rightToLeftDatabase,
            ILogger logger = null)
        {
            this.leftToRightDatabase = leftToRightDatabase;
            
            this.rightToLeftDatabase = rightToLeftDatabase;
        }

        #region IOneToOneMap

        #region IReadOnlyOneToOneMap

        public bool HasLeft(TValue1 key)
        {
            return leftToRightDatabase.ContainsKey(key);
        }

        public bool HasRight(TValue2 key)
        {
            return rightToLeftDatabase.ContainsKey(key);
        }

        public TValue2 GetRight(TValue1 key)
        {
            return leftToRightDatabase[key];
        }
        
        public TValue1 GetLeft(TValue2 key)
        {
            return rightToLeftDatabase[key];
        }
        
        public bool TryGetRight(
            TValue1 key,
            out TValue2 value)
        {
            return leftToRightDatabase.TryGetValue(
                key,
                out value);
        }
        
        public bool TryGetLeft(
            TValue2 key,
            out TValue1 value)
        {
            return rightToLeftDatabase.TryGetValue(
                key,
                out value);
        }

        public int Count
        {
            get
            {
                if (leftToRightDatabase.Count != rightToLeftDatabase.Count)
                    throw new Exception(
                        logger.TryFormat<DictionaryOneToOneMap<TValue1, TValue2>>(
                            $"LEFT TO RIGHT MAP COUNT ({leftToRightDatabase.Count}) IS NOT EQUAL TO RIGHT TO LEFT MAP COUNT ({rightToLeftDatabase.Count})"));

                return leftToRightDatabase.Count;
            }
        }

        public IEnumerable<Tuple<TValue1, TValue2>> Values
        {
            get
            {
                Tuple<TValue1, TValue2>[] result = new Tuple<TValue1, TValue2>[Count];

                var keys = leftToRightDatabase.Keys;

                int index = 0;

                foreach (var key in keys)
                {
                    result[index] = new Tuple<TValue1, TValue2>(
                        key,
                        leftToRightDatabase[key]);

                    index++;
                }

                return result;
            }
        }

        public IEnumerable<TValue1> LeftValues => leftToRightDatabase.Keys;
        
        public IEnumerable<TValue2> RightValues => rightToLeftDatabase.Keys;

        #endregion

        public void Add(
            TValue1 leftValue,
            TValue2 rightValue)
        {
            leftToRightDatabase.Add(
                leftValue,
                rightValue);
            
            rightToLeftDatabase.Add(
                rightValue,
                leftValue);
        }

        public bool TryAdd(
            TValue1 leftValue,
            TValue2 rightValue)
        {
            bool leftAdded = leftToRightDatabase.TryAdd(
                leftValue,
                rightValue);
                
            bool rightAdded = rightToLeftDatabase.TryAdd(
                rightValue,
                leftValue);

            if (leftAdded && rightAdded)
                return true;

            if (leftAdded && !rightAdded)
                leftToRightDatabase.Remove(leftValue);
            
            if (!leftAdded && rightAdded)
                rightToLeftDatabase.Remove(rightValue);
            
            return false;
        }

        public void UpdateByLeft(
            TValue1 leftValue,
            TValue2 rightValue)
        {
            var previousRightValue = leftToRightDatabase[leftValue];
            
            rightToLeftDatabase.Remove(previousRightValue);
            
            rightToLeftDatabase.Add(
                rightValue,
                leftValue);
            
            leftToRightDatabase[leftValue] = rightValue;
        }

        public void UpdateByRight(
            TValue1 leftValue,
            TValue2 rightValue)
        {
            var previousLeftValue = rightToLeftDatabase[rightValue];
            
            leftToRightDatabase.Remove(previousLeftValue);
            
            leftToRightDatabase.Add(
                leftValue,
                rightValue);
            
            rightToLeftDatabase[rightValue] = leftValue;
        }

        public bool TryUpdateByLeft(
            TValue1 leftValue,
            TValue2 rightValue)
        {
            if (!HasLeft(leftValue))
                return false;

            UpdateByLeft(
                leftValue,
                rightValue);

            return true;
        }

        public bool TryUpdateByRight(
            TValue1 leftValue,
            TValue2 rightValue)
        {
            if (!HasRight(rightValue))
                return false;

            UpdateByRight(
                leftValue,
                rightValue);

            return true;
        }

        public void AddOrUpdateByLeft(
            TValue1 leftValue,
            TValue2 rightValue)
        {
            if (HasLeft(leftValue))
                UpdateByLeft(
                    leftValue,
                    rightValue);
            else
                Add(
                    leftValue,
                    rightValue);
        }

        public void AddOrUpdateByRight(
            TValue1 leftValue,
            TValue2 rightValue)
        {
            if (HasRight(rightValue))
                UpdateByRight(
                    leftValue,
                    rightValue);
            else
                Add(
                    leftValue,
                    rightValue);
        }

        public void RemoveByLeft(TValue1 leftValue)
        {
            var previousRightValue = leftToRightDatabase[leftValue];
            
            rightToLeftDatabase.Remove(previousRightValue);
            
            leftToRightDatabase.Remove(leftValue);
        }

        public void RemoveByRight(TValue2 rightValue)
        {
            var previousLeftValue = rightToLeftDatabase[rightValue];
            
            leftToRightDatabase.Remove(previousLeftValue);
            
            rightToLeftDatabase.Remove(rightValue);
        }

        public bool TryRemoveByLeft(TValue1 leftValue)
        {
            if (!HasLeft(leftValue))
                return false;

            RemoveByLeft(leftValue);

            return true;
        }

        public bool TryRemoveByRight(TValue2 rightValue)
        {
            if (!HasRight(rightValue))
                return false;

            RemoveByRight(rightValue);

            return true;
        }

        public void Clear()
        {
            leftToRightDatabase.Clear();
            
            rightToLeftDatabase.Clear();
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            foreach (var value in leftToRightDatabase.Values)
            {
                if (value is ICleanUppable)
                    (value as ICleanUppable).Cleanup();
            }
            
            foreach (var value in rightToLeftDatabase.Values)
            {
                if (value is ICleanUppable)
                    (value as ICleanUppable).Cleanup();
            }

            Clear();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            foreach (var value in leftToRightDatabase.Values)
            {
                if (value is IDisposable)
                    (value as IDisposable).Dispose();
            }
            
            foreach (var value in rightToLeftDatabase.Values)
            {
                if (value is IDisposable)
                    (value as IDisposable).Dispose();
            }

            Clear();

            if (leftToRightDatabase is IDisposable)
                (leftToRightDatabase as IDisposable).Dispose();
            
            if (rightToLeftDatabase is IDisposable)
                (rightToLeftDatabase as IDisposable).Dispose();
        }

        #endregion
    }
}