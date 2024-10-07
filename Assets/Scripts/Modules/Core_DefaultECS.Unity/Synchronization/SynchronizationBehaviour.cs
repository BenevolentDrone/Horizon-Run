using HereticalSolutions.Delegates;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
    public class SynchronizationBehaviour : MonoBehaviour
    {
        private IPublisherNoArgs updatePing;

        private IPublisherNoArgs fixedUpdatePing;

        private IPublisherNoArgs lateUpdatePing;

        public void Initialize(
            IPublisherNoArgs updatePing,
            IPublisherNoArgs fixedUpdatePing,
            IPublisherNoArgs lateUpdatePing)
        {
            this.updatePing = updatePing;

            this.fixedUpdatePing = fixedUpdatePing;

            this.lateUpdatePing = lateUpdatePing;
        }

        void FixedUpdate()
        {
            fixedUpdatePing?.Publish();
        }

        void Update()
        {
            updatePing?.Publish();
        }

        void LateUpdate()
        {
            lateUpdatePing?.Publish();
        }
    }
}