using System.IO;
using UnityEditor;
using UnityEngine;

public class Screenshooter
{
    [MenuItem("Tools/Screenshooter")]
    public static bool CaptureEditorScreenshot() //(string _filename, bool _transparent)
    {
        string _filename = "Screenshot";
        bool _transparent = false;

        SceneView sw = SceneView.lastActiveSceneView;

        if (sw == null)
        {
            Debug.LogError("Unable to capture editor screenshot, no scene view found");
            return false;
        }

        Camera cam = sw.camera;

        if (cam == null)
        {
            Debug.LogError("Unable to capture editor screenshot, no camera attached to current scene view");

            return false;
        }
        
        RenderTexture previousTexture = cam.targetTexture;

        cam.targetTexture = new RenderTexture(1920, 1080, previousTexture.depth);

        cam.Render();

        RenderTexture renderTexture = cam.targetTexture;

        if (renderTexture == null)
        {
            Debug.LogError("Unable to capture editor screenshot, camera has no render texture attached");

            return false;
        }

        int width = 1920; //renderTexture.width;
        int height = 1080; //renderTexture.height;

        var outputTexture = new Texture2D(width, height, _transparent ? TextureFormat.RGBA32 : TextureFormat.RGB24, false);

        RenderTexture.active = renderTexture;

        outputTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);

        byte[] pngData = outputTexture.EncodeToPNG();

        FileStream file = File.Create(_filename);

        if (!file.CanWrite)
        {
            Debug.LogError("Unable to capture editor screenshot, Failed to open file for writing");

            return false;
        }

        file.Write(pngData, 0, pngData.Length);

        file.Close();

        GameObject.DestroyImmediate(outputTexture);

        Debug.Log("Screenshot written to file " + _filename);

        cam.targetTexture = previousTexture;

        return true;
    }
}