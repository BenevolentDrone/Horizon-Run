namespace HereticalSolutions.Templates.Universal.Unity
{
	public interface IDearImGuiManager
	{
		void RegisterHoveredImGuiWindow(object window);

		void UnregisterHoveredImGuiWindow(object window);
	}
}