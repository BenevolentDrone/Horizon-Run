using UnityEditor;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Editor
{
    public abstract class SimulationDrawer
    {
        protected bool draw;

        protected abstract string Title { get; }

        public virtual void Initialize()
        {
            
        }

        public void TryDraw()
        {
            EditorUtilities.AddPadding(() => { draw = EditorUtilities.LabelBoxWithToggle(Title, draw); });

            if (draw)
            {
                Draw();
            }

            EditorGUILayout.Space();
        }

        protected abstract void Draw();

        public virtual void DrawHandles(SimulationOverseer overseer)
        {}
    }
}