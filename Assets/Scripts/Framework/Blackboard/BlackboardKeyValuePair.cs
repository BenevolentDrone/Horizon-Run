using System;

namespace HereticalSolutions.Blackboard
{
    [Serializable]
    public class BlackboardKeyValuePair
    {
        public string Key;

        public EBlackboardValueType ValueType;
        
        public string Value;
    }
}