namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public interface IDearImGuiManager
	{
		void RegisterHoveredImGuiWindow(object window);

		void UnregisterHoveredImGuiWindow(object window);
	}
}