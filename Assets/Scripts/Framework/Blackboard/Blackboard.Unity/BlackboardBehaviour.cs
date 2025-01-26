﻿using System;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

namespace HereticalSolutions.Blackboard
{
    public class BlackboardBehaviour : MonoBehaviour
    {
        [SerializeField]
        private BlackboardKeyValuePair[] serializedData;

        private IRepository<string, BlackboardValue> blackboard = RepositoryFactory.BuildDictionaryRepository<string, BlackboardValue>();

        public Action<BlackboardBehaviour> OnModified { get; set; }

        private ILogger logger;

        private void Awake()
        {
            if (serializedData != null)
            {
                foreach (var keyValuePair in serializedData)
                {
                    blackboard.TryAdd(
                        keyValuePair.Key,
                        new BlackboardValue(
                            keyValuePair.ValueType,
                            keyValuePair.Value,
                            logger));
                }
            }
        }

        public bool Has(
            string key)
        {
            return blackboard.Has(key);
        }

        public BlackboardValue Get(
            string key)
        {
            blackboard.TryGet(key, out var result);

            return result;
        }

        public void AddOrUpdate(
            string key,
            BlackboardValue value)
        {
            if (blackboard.Has(key))
                blackboard.Update(key, value);
            else
                blackboard.Add(key, value);
            
            OnModified?.Invoke(this);
        }

        public void Remove(
            string key)
        {
            blackboard.TryRemove(key);
            
            OnModified?.Invoke(this);
        }
    }
}