using System;
using System.Collections.Generic;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

namespace HereticalSolutions.Blackboard
{
    public static class BlackboardValueTypeExtensions
    {
        public static object Parse(
            this string serializedValue,
            EBlackboardValueType valueType,
            ILogger logger = null)
        {
            object value = null;
            
            switch (valueType)
            {
                case EBlackboardValueType.FLAG:
                    value = null;
                    break;
                
                case EBlackboardValueType.STRING:
                    value = serializedValue;
                    break;
                
                case EBlackboardValueType.INT:
                    value = Convert.ToInt32(serializedValue);
                    break;
                
                case EBlackboardValueType.BOOL:
                    value = Convert.ToBoolean(serializedValue);
                    break;
                
                case EBlackboardValueType.FLOAT:
                    value = float.Parse(serializedValue);
                    break;
                
                default:
                    throw new Exception(
                        logger.TryFormat(
                            "INVALID VALUE TYPE"));
            }

            return value;
        }

        public static float GetFloat(
            this object value,
            EBlackboardValueType valueType,
            ILogger logger = null)
        {
            if (valueType == EBlackboardValueType.INT)
            {
                int intValue = (int)value;
                
                return (float)(intValue);
            }

            if (valueType == EBlackboardValueType.FLOAT)
                return (float)value;

            throw new Exception(
                logger.TryFormat(
                    $"CANNOT CONVERT VALUE TYPE {valueType} TO float"));
        }
        
        public static bool EqualRaw<TValue>(
            this TValue value,
            object otherValue)
        {
            TValue valueToCompare = (TValue)otherValue;
            
            return EqualityComparer<TValue>.Default.Equals(value, valueToCompare);
        }

        public static bool EqualInFloat(
            this object value,
            EBlackboardValueType valueType,
            object valueToCompare,
            EBlackboardValueType valueToCompareType,
            ILogger logger = null)
        {
            float a = value.GetFloat(
                valueType,
                logger);

            float b = valueToCompare.GetFloat(
                valueToCompareType,
                logger);
                            
            return Mathf.Abs(a - b) < MathHelpers.EPSILON;
        }

        public static bool LessRaw<TValue>(
            this TValue value,
            object otherValue)
        {
            TValue valueToCompare = (TValue)otherValue;
            
            return Comparer<TValue>.Default.Compare(value, valueToCompare) < 0;
        }
        
        public static bool LessInFloat(
            this object value,
            EBlackboardValueType valueType,
            object valueToCompare,
            EBlackboardValueType valueToCompareType,
            ILogger logger = null)
        {
            float a = value.GetFloat(
                valueType,
                logger);

            float b = valueToCompare.GetFloat(
                valueToCompareType,
                logger);

            return a < b;
        }
    }
}