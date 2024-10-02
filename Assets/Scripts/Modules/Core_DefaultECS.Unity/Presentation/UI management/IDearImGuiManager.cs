namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	public interface IDearImGuiManager
	{
		void RegisterHoveredImGuiWindow(object window);

		void UnregisterHoveredImGuiWindow(object window);
	}
}