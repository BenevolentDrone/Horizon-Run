namespace HereticalSolutions.HorizonRun
{
	public interface IDearImGuiManager
	{
		void RegisterHoveredImGuiWindow(object window);

		void UnregisterHoveredImGuiWindow(object window);
	}
}