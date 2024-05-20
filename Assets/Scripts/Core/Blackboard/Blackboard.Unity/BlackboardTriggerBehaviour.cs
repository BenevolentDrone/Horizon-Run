using System;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Blackboard
{
    public class BlackboardTriggerBehaviour : MonoBehaviour
    {
        [Inject]
        private ILoggerResolver loggerResolver;

        [SerializeField]
        private BlackboardBehaviour blackboardBehaviour;

        [SerializeField]
        private BlackboardTriggerCondition[] conditions;
        
        private ILogger logger;

        private bool triggered;

        public bool Triggered
        {
            get => triggered;
        }

        public Action<BlackboardTriggerBehaviour> OnTriggered { get; set; }

        private void Awake()
        {
            logger = loggerResolver.GetLogger<BlackboardTriggerBehaviour>();
        }

        private void Start()
        {
            if (blackboardBehaviour == null)
                throw new Exception(
                    logger.TryFormat<BlackboardTriggerBehaviour>(
                        "BLACKBOARD BEHAVIOUR IS NULL"));
            
            blackboardBehaviour.OnModified += PollTriggerConditions;
            
            PollTriggerConditions(blackboardBehaviour);
        }

        private void OnDestroy()
        {
            if (blackboardBehaviour != null)
                blackboardBehaviour.OnModified -= PollTriggerConditions;
        }

        private void PollTriggerConditions(BlackboardBehaviour behaviour)
        {
            if (triggered)
                return;

            foreach (var condition in conditions)
            {
                if (!Satisfied(condition))
                    return;
            }

            triggered = true;

            OnTriggered?.Invoke(this);
        }
        
        private bool Satisfied(BlackboardTriggerCondition condition)
        {
            if (!blackboardBehaviour.Has(condition.Key))
                return false;

            return blackboardBehaviour
                .Get(condition.Key)
                .Compare(
                    condition.Comparison,
                    condition.ValueType,
                    condition.Value);
        }
    }
}