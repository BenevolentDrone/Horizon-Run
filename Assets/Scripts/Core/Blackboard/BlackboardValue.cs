using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Blackboard
{
    public class BlackboardValue
    {
        private readonly ILogger logger;

        public EBlackboardValueType ValueType { get; private set; }

        private object value;

        public BlackboardValue(
            EBlackboardValueType valueType,
            object value,
            ILogger logger = null)
        {
            this.logger = logger;

            ValueType = valueType;

            this.value = value;
        }
        
        public BlackboardValue(
            EBlackboardValueType valueType,
            string serializedValue,
            ILogger logger = null)
        {
            this.logger = logger;

            ValueType = valueType;

            value = serializedValue.Parse(
                valueType,
                logger);
        }

        public object GetValueObject()
        {
            return value;
        }

        public TValue GetValue<TValue>()
        {
            return (TValue)value;
        }

        public void SetValueObject(object newValue)
        {
            value = newValue;
        }

        public void SetValue<TValue>(TValue value)
        {
            this.value = value;
        }

        public bool Compare(
            EComparisons comparisonType,
            EBlackboardValueType valueToCompareType,
            object valueToCompare)
        {
            switch (comparisonType)
            {
                case EComparisons.EXISTS:
                    return true; //key exists, that's enough

                case EComparisons.EQUALS_TO:
                    if (ValueType == EBlackboardValueType.FLAG)
                        throw new Exception(
                            logger.TryFormat<BlackboardValue>(
                                $"FLAGS CANNOT BE COMPARED IN ANY FORM OTHER THAN EXISTS"));

                    switch (ValueType)
                    {
                        case EBlackboardValueType.STRING:
                            if (valueToCompareType != EBlackboardValueType.STRING)
                                throw new Exception(
                                    logger.TryFormat<BlackboardValue>(
                                        $"CANNOT COMPARE string TO {valueToCompareType} BY EQUALITY"));

                            return GetValue<string>().EqualRaw(valueToCompare);

                        case EBlackboardValueType.INT:
                            if (valueToCompareType != EBlackboardValueType.INT
                                && valueToCompareType != EBlackboardValueType.FLOAT)
                                throw new Exception(
                                    logger.TryFormat<BlackboardValue>(
                                        $"CANNOT COMPARE int TO {valueToCompareType} BY EQUALITY"));

                            if (valueToCompareType == EBlackboardValueType.INT)
                                return GetValue<int>().EqualRaw(valueToCompare);

                            return value.EqualInFloat(
                                ValueType,
                                valueToCompare,
                                valueToCompareType,
                                logger);

                        case EBlackboardValueType.FLOAT:
                            if (valueToCompareType != EBlackboardValueType.INT
                                && valueToCompareType != EBlackboardValueType.FLOAT)
                                throw new Exception(
                                    logger.TryFormat<BlackboardValue>(
                                        $"CANNOT COMPARE float TO {valueToCompareType} BY EQUALITY"));

                            return value.EqualInFloat(
                                ValueType,
                                valueToCompare,
                                valueToCompareType,
                                logger);

                        case EBlackboardValueType.BOOL:
                            if (valueToCompareType != EBlackboardValueType.BOOL)
                                throw new Exception(
                                    logger.TryFormat<BlackboardValue>(
                                        $"CANNOT COMPARE bool TO {valueToCompareType} BY EQUALITY"));

                            return GetValue<bool>().EqualRaw(valueToCompare);

                        default:
                            throw new Exception(
                                logger.TryFormat<BlackboardValue>(
                                    "INVALID VALUE TYPE"));
                    }

                case EComparisons.NOT_EQUALS_TO:
                    return !Compare(
                        EComparisons.EQUALS_TO,
                        valueToCompareType,
                        valueToCompare);

                case EComparisons.LESS_THAN:
                    if (ValueType == EBlackboardValueType.FLAG)
                        throw new Exception(
                            logger.TryFormat<BlackboardValue>(
                                $"FLAGS CANNOT BE COMPARED IN ANY FORM OTHER THAN EXISTS"));

                    if (ValueType == EBlackboardValueType.STRING)
                        throw new Exception(
                            logger.TryFormat<BlackboardValue>(
                                $"STRINGS CANNOT BE COMPARED IN ANY FORM OTHER THAN EXISTS, EQUAL AND NOT_EQUAL"));

                    if (ValueType == EBlackboardValueType.BOOL)
                        throw new Exception(
                            logger.TryFormat<BlackboardValue>(
                                $"BOOLS CANNOT BE COMPARED IN ANY FORM OTHER THAN EXISTS, EQUAL AND NOT_EQUAL"));

                    switch (ValueType)
                    {
                        case EBlackboardValueType.INT:
                            if (valueToCompareType != EBlackboardValueType.INT
                                && valueToCompareType != EBlackboardValueType.FLOAT)
                                throw new Exception(
                                    logger.TryFormat<BlackboardValue>(
                                        $"CANNOT COMPARE int TO {valueToCompareType} BY LESS OR MORE THAN"));

                            if (valueToCompareType == EBlackboardValueType.INT)
                                return GetValue<int>().LessRaw(valueToCompare);

                            return value.LessInFloat(
                                ValueType,
                                valueToCompare,
                                valueToCompareType,
                                logger);

                        case EBlackboardValueType.FLOAT:
                            if (valueToCompareType != EBlackboardValueType.INT
                                && valueToCompareType != EBlackboardValueType.FLOAT)
                                throw new Exception(
                                    logger.TryFormat<BlackboardValue>(
                                        $"CANNOT COMPARE float TO {valueToCompareType} BY LESS OR MORE THAN"));

                            return value.LessInFloat(
                                ValueType,
                                valueToCompare,
                                valueToCompareType,
                                logger);

                        default:
                            throw new Exception(
                                logger.TryFormat<BlackboardValue>(
                                    "INVALID VALUE TYPE"));
                    }

                case EComparisons.MORE_THAN:
                    return !Compare(
                        EComparisons.LESS_THAN,
                        valueToCompareType,
                        valueToCompare);

                case EComparisons.EQUALS_OR_LESS_THAN:
                    return Compare(
                               EComparisons.EQUALS_TO,
                               valueToCompareType,
                               valueToCompare)
                           || Compare(
                               EComparisons.LESS_THAN,
                               valueToCompareType,
                               valueToCompare);

                case EComparisons.EQUALS_OR_MORE_THAN:
                    return Compare(
                               EComparisons.EQUALS_TO,
                               valueToCompareType,
                               valueToCompare)
                           || Compare(
                               EComparisons.MORE_THAN,
                               valueToCompareType,
                               valueToCompare);

                default:
                    throw new Exception(
                        logger.TryFormat<BlackboardValue>(
                            "INVALID COMPARISON TYPE"));
            }
        }

        public bool Compare(
            EComparisons comparisonType,
            EBlackboardValueType valueToCompareType,
            string serializedValueToCompare)
        {
            return Compare(
                comparisonType,
                valueToCompareType,
                serializedValueToCompare.Parse(
                    valueToCompareType,
                    logger));
        }
    }
}