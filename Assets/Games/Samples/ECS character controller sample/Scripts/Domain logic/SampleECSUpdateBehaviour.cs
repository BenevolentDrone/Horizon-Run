using System;

using HereticalSolutions.Synchronization;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Factories;

using UnityEngine;

using DefaultEcs.System;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleECSUpdateBehaviour : MonoBehaviour
	{
		private ISynchronizationProvider updateTimeManagerAsProvider;

		private ISynchronizationProvider fixedUpdateTimeManagerAsProvider;

		private ISynchronizationProvider lateUpdateTimeManagerAsProvider;


		private ISubscription updateSubscription;

		private ISubscription fixedUpdateSubscription;

		private ISubscription lateUpdateSubscription;


		private ISystem<float> updateSystems;

		private ISystem<float> fixedUpdateSystems;

		private ISystem<float> lateUpdateSystems;


		public void Initialize(
			ISynchronizationProvider updateTimeManagerAsProvider,
			ISynchronizationProvider fixedUpdateTimeManagerAsProvider,
			ISynchronizationProvider lateUpdateTimeManagerAsProvider,
			
			ISystem<float> updateSystems,
			ISystem<float> fixedUpdateSystems,
			ISystem<float> lateUpdateSystems)
		{
			this.updateTimeManagerAsProvider = updateTimeManagerAsProvider;

			this.fixedUpdateTimeManagerAsProvider = fixedUpdateTimeManagerAsProvider;

			this.lateUpdateTimeManagerAsProvider = lateUpdateTimeManagerAsProvider;


			this.updateSystems = updateSystems;

			this.fixedUpdateSystems = fixedUpdateSystems;

			this.lateUpdateSystems = lateUpdateSystems;


			updateSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<float>(TickUpdateSystems);

			fixedUpdateSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<float>(TickFixedUpdateSystems);

			lateUpdateSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<float>(TickLateUpdateSystems);


			updateTimeManagerAsProvider?.Subscribe(
				updateSubscription);

			fixedUpdateTimeManagerAsProvider?.Subscribe(
				fixedUpdateSubscription);

			lateUpdateTimeManagerAsProvider?.Subscribe(
				lateUpdateSubscription);
		}

		void TickFixedUpdateSystems(float timeDelta)
		{
			fixedUpdateSystems?.Update(
				timeDelta);
		}

		void TickUpdateSystems(float timeDelta)
		{
			updateSystems?.Update(
				timeDelta);
		}

		void TickLateUpdateSystems(float timeDelta)
		{
			lateUpdateSystems?.Update(
				timeDelta);
		}

		void OnDisable()
		{
			if (updateSubscription != null
				&& updateTimeManagerAsProvider != null
				&& updateSubscription.Active)
			{
				updateTimeManagerAsProvider.Unsubscribe(
					updateSubscription);
			}

			if (fixedUpdateSubscription != null
				&& fixedUpdateTimeManagerAsProvider != null
				&& fixedUpdateSubscription.Active)
			{
				fixedUpdateTimeManagerAsProvider.Unsubscribe(
					fixedUpdateSubscription);
			}

			if (lateUpdateSubscription != null
				&& lateUpdateTimeManagerAsProvider != null
				&& lateUpdateSubscription.Active)
			{
				lateUpdateTimeManagerAsProvider.Unsubscribe(
					lateUpdateSubscription);
			}

			(updateSubscription as IDisposable)?.Dispose();

			(fixedUpdateSubscription as IDisposable)?.Dispose();

			(lateUpdateSubscription as IDisposable)?.Dispose();
		}
	}
}