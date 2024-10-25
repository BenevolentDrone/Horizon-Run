using UnityEngine;
using UnityEditor;
using System;

[InitializeOnLoad]
public class EditorAnimationPlayer
{
    public static bool registered = false;

    public static bool active = false;

    public static Action<float> OnSimulate;

    public static Func<bool> OnSimulationFinished;

    public static Func<bool> InterruptSimulation;

    public static float frameDuration;

    public static int animationDurationInFrames;

    public static int currentFrame;

    static EditorAnimationPlayer()
    {
        if (!registered)
        {
            // hook into the editor update
            EditorApplication.update += Update;

            // and the scene view OnGui
            SceneView.onSceneGUIDelegate += OnSceneGUI;

            registered = true;
        }
    }

    public static void Activate(float _frameDuration, int _animationDurationInFrames, Action<float> onSimulate, Func<bool> onSimulationFinished, Func<bool> interruptSimulation = null)
    {
        if (!active)
        {
            active = true;

            currentFrame = -1; //0;

            OnSimulate = onSimulate;

            OnSimulationFinished = onSimulationFinished;

            InterruptSimulation = interruptSimulation;

            frameDuration = _frameDuration;

            animationDurationInFrames = _animationDurationInFrames;
        }
    }

    public static void Update()
    {
        if (active)
        {
            currentFrame++;

            float currentSecond = currentFrame * frameDuration;

            if (currentFrame > animationDurationInFrames 
                || InterruptSimulation != null && InterruptSimulation())
            {
                active = false;

                if (OnSimulationFinished != null)
                    if (!OnSimulationFinished())
                        return;

                OnSimulate = null;

                OnSimulationFinished = null;

                currentFrame = -1;

                frameDuration = 0f;

                animationDurationInFrames = -1;
            }
            else
            {
                if (OnSimulate != null)
                    OnSimulate(currentSecond);
            }
        }
    }

    public static void OnSceneGUI(SceneView sceneView)
    {
        if (active)
        {
            float currentSecond = currentFrame * frameDuration;

            Handles.BeginGUI();

            Color cacheColor = GUI.color;

            GUI.color = Color.red;

            GUILayout.Label("Playing animation", GUI.skin.box, GUILayout.Width(200));

            GUILayout.Label(string.Format("Current second: {0:F2}", currentSecond), GUI.skin.box, GUILayout.Width(200));

            if (GUILayout.Button("STOP!"))
                Debug.Log("Stop");

            Handles.EndGUI();

            GUI.color = cacheColor;
        }
    }

    public static void SetAnimationFrame(Animator animator, int clipHash, float normalizedTime)
    {
        animator.speed = 0f;
        animator.Play(clipHash, 0, normalizedTime);
        animator.Update(0f);
    }
}