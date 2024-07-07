using UnityEngine;

using Zenject;

namespace HereticalSolutions.Templates.Universal.Unity.DI
{
    public class UIManagerInstaller : MonoInstaller
    {
        //ImGui
        [SerializeField]
        private DearImGuiManager dearImGuiManager;

        public override void InstallBindings()
		{
            Container
				.Bind<IUIManager>()
				.FromInstance(dearImGuiManager)
				.AsCached();

            Container
                .Bind<IDearImGuiManager>()
                .FromInstance(dearImGuiManager)
                .AsCached();
        }
	}
}