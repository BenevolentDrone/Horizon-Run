using System;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Blackboard
{
    public class BlackboardCarrierReferenceTriggerBehaviour : MonoBehaviour
    {
        [Inject]
        ILoggerResolver loggerResolver;

        [SerializeField]
        private MonoBehaviour blackboardCarrierReferenceBehaviour;
        
        //[SerializeField]
        //private BlackboardBehaviour blackboardBehaviour;


        [SerializeField]
        private BlackboardKeyValuePair[] dataToShareWithSelfOnReferencePresent;
        
        [SerializeField]
        private BlackboardKeyValuePair[] dataToShareWithReferencedBlackboard;

        private ILogger logger;


        private IBlackboardCarrier lastReferencedBlackboardCarrier;

        private void Awake()
        {
            logger = loggerResolver.GetLogger<BlackboardCarrierReferenceTriggerBehaviour>();
        }

        private void Start()
        {
            if (blackboardCarrierReferenceBehaviour == null)
                throw new Exception(
                    logger.TryFormat<BlackboardCarrierReferenceTriggerBehaviour>(
                        "CARRIER BEHAVIOUR IS NULL"));


            var currentBlackboardCarrierReference = blackboardCarrierReferenceBehaviour as IBlackboardCarrierReference;


            lastReferencedBlackboardCarrier = currentBlackboardCarrierReference.ReferencedCarrier;


            currentBlackboardCarrierReference.OnReferenceModified += ModifyMetadata;
            
            ModifyMetadata(currentBlackboardCarrierReference);
        }

        private void OnDestroy()
        {
            var currentBlackboardCarrierReference = blackboardCarrierReferenceBehaviour as IBlackboardCarrierReference;

            if (currentBlackboardCarrierReference != null)
                currentBlackboardCarrierReference.OnReferenceModified -= ModifyMetadata;
        }

        private void ModifyMetadata(IBlackboardCarrierReference blackboardCarrierReference)
        {
            //Remove shared data from last carrier
            if (lastReferencedBlackboardCarrier != null)
            {
                var lastReferencesBlackboardBehaviour = lastReferencedBlackboardCarrier.BlackboardBehaviour;

                if (lastReferencesBlackboardBehaviour != null)
                {
                    foreach (var data in dataToShareWithReferencedBlackboard)
                        lastReferencesBlackboardBehaviour?.Remove(data.Key);
                }
            }
            
            if (blackboardCarrierReference.ReferencedCarrier == null)
            {
                foreach (var data in dataToShareWithSelfOnReferencePresent)
                    blackboardCarrierReference.BlackboardBehaviour?.Remove(data.Key);

                lastReferencedBlackboardCarrier = null;
            }
            else
            {
                foreach (var data in dataToShareWithSelfOnReferencePresent)
                    blackboardCarrierReference.BlackboardBehaviour?.AddOrUpdate(
                        data.Key,
                        new BlackboardValue(
                            data.ValueType,
                            data.Value));
                
                lastReferencedBlackboardCarrier = blackboardCarrierReference.ReferencedCarrier;

                var newReferencesBlackboardBehaviour = lastReferencedBlackboardCarrier.BlackboardBehaviour;

                if (newReferencesBlackboardBehaviour != null)
                {
                    foreach (var data in dataToShareWithReferencedBlackboard)
                        newReferencesBlackboardBehaviour?.AddOrUpdate(
                            data.Key,
                            new BlackboardValue(
                                data.ValueType,
                                data.Value));
                }
            }
        }
    }
}