using HereticalSolutions.Repositories.Factories;

using UnityEditor;

using UnityEngine;

namespace HereticalSolutions.Tools.AnimationRetargettingToolbox
{
	public class ARToolboxWindow : EditorWindow
	{
		private const string WINDOW_TITLE = "ARToolbox"; //"Animation Retargetting Toolbox";

		private static IARToolboxToolWindow[] toolWindows;

		private IARToolboxContext context;

		private double lastEditorUpdateTime;

		private Vector2 scrollPos;

		#region EditorWindow

		[MenuItem("Tools/Animation/Animation retargetting tool")]
		static void Init()
		{
			StaticLazyInitialization();

			// Get existing open window or if none, make a new one:
			ARToolboxWindow window = (ARToolboxWindow)EditorWindow.GetWindow(
				typeof(ARToolboxWindow),
				false,
				WINDOW_TITLE);
		}

		// Window has been selected
		void OnFocus()
		{
			StaticLazyInitialization();

			LazyInitialization();

			// Remove delegate listener if it has previously
			// been assigned.
			SceneView.duringSceneGui -= this.OnSceneGUI;
			// Add (or re-add) the delegate.
			SceneView.duringSceneGui += this.OnSceneGUI;


			EditorApplication.update -= this.OnSceneUpdate;
			EditorApplication.update += this.OnSceneUpdate;
		}

		void OnGUI()
		{
			if (toolWindows == null)
				return;

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

			foreach (var toolWindow in toolWindows)
			{
				toolWindow.Draw(context);
			}

			EditorGUILayout.EndScrollView();
		}

		// Window has been destroyed
		void OnDestroy()
		{
			// When the window is destroyed, remove the delegate
			// so that it will no longer do any drawing.
			SceneView.duringSceneGui -= this.OnSceneGUI;

			EditorApplication.update -= this.OnSceneUpdate;
		}

		#endregion

		static void StaticLazyInitialization()
		{
			if (toolWindows == null)
			{
				toolWindows = new IARToolboxToolWindow[]
				{
					 new SkeletonTool(),
					 new AnimationTool()
				};
			}
		}

		void LazyInitialization()
		{
			if (context == null)
			{
				context = new ARToolboxRootContext(
					RepositoriesFactory.BuildDictionaryRepository<string, object>());
			}

			if (lastEditorUpdateTime == 0)
			{
				lastEditorUpdateTime = EditorApplication.timeSinceStartup;
			}
		}

		void OnSceneGUI(
			SceneView sceneView)
		{
			if (toolWindows == null)
				return;

			foreach (var toolWindow in toolWindows)
			{
				toolWindow.DrawHandles(context);
			}
		}

		void OnSceneUpdate()
		{
			var currentTime = EditorApplication.timeSinceStartup;

			var deltaTime = currentTime - lastEditorUpdateTime;

			if (lastEditorUpdateTime == 0)
			{
				deltaTime = 0;
			}

			lastEditorUpdateTime = currentTime;

			//UnityEngine.Debug.Log(deltaTime);

			((ARToolboxRootContext)context).DeltaTime = deltaTime;

			if (toolWindows == null)
				return;

			foreach (var toolWindow in toolWindows)
			{
				toolWindow.SceneUpdate(context);
			}
		}
	}
}