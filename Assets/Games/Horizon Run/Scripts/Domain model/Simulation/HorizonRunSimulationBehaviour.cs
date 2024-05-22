using System;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Synchronization;

using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using DefaultEcs.System;

namespace HereticalSolutions.HorizonRun
{
	public class HorizonRunSimulationBehaviour : MonoBehaviour
	{
		private ISynchronizationProvider updateTimeManagerAsProvider;

		private ISynchronizationProvider fixedUpdateTimeManagerAsProvider;

		private ISynchronizationProvider lateUpdateTimeManagerAsProvider;
		
		
		private ISubscription updateSubscription;

		private ISubscription fixedUpdateSubscription;

		private ISubscription lateUpdateSubscription;
		
		
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
			
			ILogger logger = null)
		{
			this.updateTimeManagerAsProvider = updateTimeManagerAsProvider;

			this.fixedUpdateTimeManagerAsProvider = fixedUpdateTimeManagerAsProvider;

			this.lateUpdateTimeManagerAsProvider = lateUpdateTimeManagerAsProvider;
			

			this.eventSystems = eventSystems;

			this.updateSystems = updateSystems;

			this.fixedUpdateSystems = fixedUpdateSystems;

			this.lateUpdateSystems = lateUpdateSystems;

			
			this.logger = logger;
			

			updateSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<float>(TickUpdate);

			fixedUpdateSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<float>(TickFixedUpdate);

			lateUpdateSubscription = DelegatesFactory.BuildSubscriptionSingleArgGeneric<float>(TickLateUpdate);

			
			//StartSimulation();
		}

		public void StartSimulation()
		{
			logger?.Log<HorizonRunSimulationBehaviour>(
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
			logger?.Log<HorizonRunSimulationBehaviour>(
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