using HereticalSolutions.Entities;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.DI
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