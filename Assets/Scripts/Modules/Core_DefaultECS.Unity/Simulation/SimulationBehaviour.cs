using System;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Synchronization;

using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public class SimulationBehaviour : MonoBehaviour
	{
		private ISynchronizationProvider updateTimeManagerAsProvider;

		private ISynchronizationProvider fixedUpdateTimeManagerAsProvider;

		private ISynchronizationProvider lateUpdateTimeManagerAsProvider;
		
		
		private INonAllocSubscription updateSubscription;

		private INonAllocSubscription fixedUpdateSubscription;

		private INonAllocSubscription lateUpdateSubscription;
		
		
		private ISystem<float> eventSystems;

		private ISystem<float> updateSystems;

		private ISystem<float> fixedUpdateSystems;

		private ISystem<float> lateUpdateSystems;


		private bool initialized = false;
		
		
		private ILogger logger;


		public void Initialize(
			ISynchronizationProvider updateTimeManagerAsProvider,
			ISynchronizationProvider fixedUpdateTimeManagerAsProvider,
			ISynchronizationProvider lateUpdateTimeManagerAsProvider,

			ISystem<float> eventSystems,
			ISystem<float> updateSystems,
			ISystem<float> fixedUpdateSystems,
			ISystem<float> lateUpdateSystems,
			
			ILogger logger)
		{
			this.updateTimeManagerAsProvider = updateTimeManagerAsProvider;

			this.fixedUpdateTimeManagerAsProvider = fixedUpdateTimeManagerAsProvider;

			this.lateUpdateTimeManagerAsProvider = lateUpdateTimeManagerAsProvider;
			

			this.eventSystems = eventSystems;

			this.updateSystems = updateSystems;

			this.fixedUpdateSystems = fixedUpdateSystems;

			this.lateUpdateSystems = lateUpdateSystems;

			
			this.logger = logger;
			

			updateSubscription = SubscriptionFactory.BuildSubscriptionSingleArgGeneric<float>(TickUpdate);

			fixedUpdateSubscription = SubscriptionFactory.BuildSubscriptionSingleArgGeneric<float>(TickFixedUpdate);

			lateUpdateSubscription = SubscriptionFactory.BuildSubscriptionSingleArgGeneric<float>(TickLateUpdate);
		}

		public void StartSimulation()
		{
			logger?.Log(
				GetType(),
				$"SIMULATION INITIALIZING");
			
			updateTimeManagerAsProvider?.Subscribe(
				updateSubscription);

			fixedUpdateTimeManagerAsProvider?.Subscribe(
				fixedUpdateSubscription);

			lateUpdateTimeManagerAsProvider?.Subscribe(
				lateUpdateSubscription);
			
			initialized = true;
		}

		void TickFixedUpdate(float timeDelta)
		{
			fixedUpdateSystems?.Update(
				timeDelta);
		}

		void TickUpdate(float timeDelta)
		{
			eventSystems?.Update(
				timeDelta);

			updateSystems?.Update(
				timeDelta);

			//Because why not?
			eventSystems?.Update(
				timeDelta);
		}

		void TickLateUpdate(float timeDelta)
		{
			lateUpdateSystems?.Update(
				timeDelta);
		}

		public void StopSimulation()
		{
			logger?.Log(
				GetType(),
				$"SIMULATION DEINITIALIZING");

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

			initialized = false;
		}
		
		void OnDisable()
		{
			StopSimulation();
		}
	}
}