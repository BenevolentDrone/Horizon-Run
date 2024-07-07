using HereticalSolutions.Entities;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Templates.Universal.Unity.DI
{
	public class EntityAuthoringSettingsInstaller : MonoInstaller
	{
		[SerializeField]
		private EntityAuthoringSettingsScriptable entityAuthoringSettings;

		public override void InstallBindings()
		{
			Container
				.Bind<EntityAuthoringSettings>()
				.FromInstance(entityAuthoringSettings.EntityAuthoringSettings)
				.AsCached();
		}
	}
}