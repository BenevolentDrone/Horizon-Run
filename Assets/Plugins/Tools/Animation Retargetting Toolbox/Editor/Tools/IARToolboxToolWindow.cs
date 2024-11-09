namespace HereticalSolutions.Tools.AnimationRetargettingToolbox
{
	public interface IARToolboxToolWindow
	{
		void Draw(IARToolboxContext context);

		void DrawHandles(IARToolboxContext context);

		void SceneUpdate(IARToolboxContext context);
	}
}