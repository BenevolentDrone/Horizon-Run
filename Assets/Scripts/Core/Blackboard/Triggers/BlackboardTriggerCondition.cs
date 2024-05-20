namespace HereticalSolutions.Blackboard
{
    [System.Serializable]
    public class BlackboardTriggerCondition
    {
        public EComparisons Comparison;
        
        public string Key;

        public EBlackboardValueType ValueType;
        
        public string Value;
    }
}