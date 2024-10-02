using HereticalSolutions.Entities;

using Zenject;

using UImGui;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	public abstract class ADearImGuiViewComponent
		: AMonoViewComponent
	{
		[Inject]
		protected IDearImGuiManager dearImGuiManager;

		protected static int staticDearImGuiIDComplement = 0;

		protected int dearImGuiIDComplement = -1;

		protected bool wasClickedLastFrame = false;


		protected void Awake()
		{
			dearImGuiIDComplement = staticDearImGuiIDComplement;

			staticDearImGuiIDComplement++;
		}

		private void OnEnable()
		{
			UImGuiUtility.Layout += OnLayout;
		}

		private void OnDisable()
		{
			UImGuiUtility.Layout -= OnLayout;
		}

		protected void OnLayout(UImGuiBehaviour uImGui)
		{
			bool clicked = OnLayoutInternal(uImGui);

			if (clicked
				&& !wasClickedLastFrame)
			{
				dearImGuiManager.RegisterHoveredImGuiWindow(this);
			}

			if (!clicked
				 && wasClickedLastFrame)
			{
				dearImGuiManager.UnregisterHoveredImGuiWindow(this);
			}

			wasClickedLastFrame = clicked;
		}

		protected abstract bool OnLayoutInternal(UImGuiBehaviour uImGui);
	}
}