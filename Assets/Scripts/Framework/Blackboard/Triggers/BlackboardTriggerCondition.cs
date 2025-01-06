using System;

namespace HereticalSolutions.Blackboard
{
    [Serializable]
    public class BlackboardTriggerCondition
    {
        public EComparisons Comparison;
        
        public string Key;

        public EBlackboardValueType ValueType;
        
        public string Value;
    }
}