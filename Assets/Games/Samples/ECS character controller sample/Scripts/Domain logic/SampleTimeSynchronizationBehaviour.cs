using UnityEngine;

using HereticalSolutions.Time;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample
{
	public class SampleTimeSynchronizationBehaviour : MonoBehaviour
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
			fixedUpdateTimeManagerAsTickable?.Tick(UnityEngine.Time.deltaTime);
		}

		void Update()
		{
			updateTimeManagerAsTickable?.Tick(UnityEngine.Time.deltaTime);
		}

		void LateUpdate()
		{
			lateUpdateTimeManagerAsTickable?.Tick(UnityEngine.Time.deltaTime);
		}
	}
}