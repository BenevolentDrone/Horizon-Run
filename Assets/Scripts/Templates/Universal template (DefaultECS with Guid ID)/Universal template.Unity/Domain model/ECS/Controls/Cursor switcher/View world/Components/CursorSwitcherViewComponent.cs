using HereticalSolutions.Entities;

using UImGui.Assets;

namespace HereticalSolutions.Templates.Universal.Unity
{
    [ViewComponent]
    public class CursorSwitcherViewComponent
        : AMonoViewComponent
    {
        public CursorData[] Cursors;
        
        public CursorShapesAsset CursorShapesAsset;

        
        public string PreferredCursor;

        public bool Dirty;
    }
}