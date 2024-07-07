using UnityEngine;

using UnityEditor;

namespace HereticalSolutions.Templates.Universal.Unity.Editor
{
    public class SimulationOverseer : EditorWindow
    {
        private bool initialized = false;

        private SimulationDrawer[] drawers;

        private Vector2 scrollPosition;

        // Window has been selected
        void OnFocus()
        {
            // Remove delegate listener if it has previously
            // been assigned.
            SceneView.duringSceneGui -= this.OnSceneGUI;
            // Add (or re-add) the delegate.
            SceneView.duringSceneGui += this.OnSceneGUI;
        }

        void OnDestroy()
        {
            // When the window is destroyed, remove the delegate
            // so that it will no longer do any drawing.
            SceneView.duringSceneGui -= this.OnSceneGUI;
        }

        void OnSceneGUI(SceneView sceneView)
        {
            foreach (var drawer in drawers)
                drawer.DrawHandles(this);

            /*
            Handles.BeginGUI();
            // Do your drawing here using GUI.
            Handles.EndGUI();
            */
        }

        [MenuItem("Tools/Simulation overseer")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            SimulationOverseer window = (SimulationOverseer)EditorWindow.GetWindow(typeof(SimulationOverseer));
        }

        private void OnEnable()
        {
            if (drawers == null)
                drawers = new SimulationDrawer[]
                {
                    //new SpaceDrawer(),
                    new RegistryEntityDrawer()
                };
        }

        void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorUtilities.WarningBox("Overseer works only in runtime");

                return;
            }

            if (!Initialized())
            {
                EditorUtilities.WarningBox("Not initialized");

                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            foreach (var drawer in drawers)
                drawer.TryDraw();

            EditorGUILayout.EndScrollView();

            if (Input.GetKey(KeyCode.Space) && !EditorApplication.isPaused)
                EditorApplication.isPaused = true;

            //EditorUtilities.WarningBox("Initialized");
        }

        private bool Initialized()
        {
            if (!initialized)
            {
                foreach (var drawer in drawers)
                    drawer.Initialize();

                initialized = true;
            }

            return initialized;
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}