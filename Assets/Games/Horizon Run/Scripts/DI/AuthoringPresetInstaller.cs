using HereticalSolutions.Entities;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.HorizonRun.DI
{
	public class AuthoringPresetInstaller : MonoInstaller
	{
		[SerializeField]
		private EEntityAuthoringPresets authoringPreset;

		public override void InstallBindings()
		{
			Container
				.Bind<EEntityAuthoringPresets>()
				.FromInstance(authoringPreset)
				.AsCached();
		}
	}
}