using UnityEngine;

using HereticalSolutions.Time;

namespace HereticalSolutions.HorizonRun
{
	public class HorizonRunTimeSynchronizationBehaviour : MonoBehaviour
	{
		private ITickable updateTimeManagerAsTickable;

		private ITickable fixedUpdateTimeManagerAsTickable;

		private ITickable lateUpdateTimeManagerAsTickable;

		public void Initialize(
			ITickable updateTimeManagerAsTickable,
			ITickable fixedUpdateTimeManagerAsTickable,
			ITickable lateUpdateTimeManagerAsTickable)
		{
			this.updateTimeManagerAsTickable = updateTimeManagerAsTickable;

			this.fixedUpdateTimeManagerAsTickable = fixedUpdateTimeManagerAsTickable;

			this.lateUpdateTimeManagerAsTickable = lateUpdateTimeManagerAsTickable;
		}

		void FixedUpdate()
		{
			fixedUpdateTimeManagerAsTickable?.Tick(
				UnityEngine.Time.deltaTime);
		}

		void Update()
		{
			updateTimeManagerAsTickable?.Tick(
				UnityEngine.Time.deltaTime);
		}

		void LateUpdate()
		{
			lateUpdateTimeManagerAsTickable?.Tick(
				UnityEngine.Time.deltaTime);
		}
		
		public void Deinitialize()
		{
			updateTimeManagerAsTickable = null;
			
			fixedUpdateTimeManagerAsTickable = null;
			
			lateUpdateTimeManagerAsTickable = null;
		}
	}
}