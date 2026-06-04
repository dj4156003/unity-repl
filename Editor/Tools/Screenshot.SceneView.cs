
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace UnityReplGs.Tools
{
    [ToolComment("Render the current Scene View camera to a PNG file.", "UnityReplGs.Tools.ScreenshotSceneViewTool.Capture(\"Temp/scene-view.png\", 1024, 768)")]
    public static class ScreenshotSceneViewTool
    {
        public static string Capture(string outputPath = "Temp/unity-repl-gs-scene-view.png", int width = 1024, int height = 768)
        {
            return JsonOut.Try(() =>
            {
                var sceneView = SceneView.lastActiveSceneView;
                if (sceneView == null || sceneView.camera == null)
                    throw new System.InvalidOperationException("No active SceneView camera.");
                CaptureCamera(sceneView.camera, outputPath, width, height);
                return new { path = outputPath, width = width, height = height, exists = File.Exists(outputPath) };
            });
        }

        static void CaptureCamera(Camera camera, string outputPath, int width, int height)
        {
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);
            var previousTarget = camera.targetTexture;
            var previousActive = RenderTexture.active;
            var rt = new RenderTexture(width, height, 24);
            var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            try
            {
                camera.targetTexture = rt;
                camera.Render();
                RenderTexture.active = rt;
                tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                tex.Apply();
                File.WriteAllBytes(outputPath, tex.EncodeToPNG());
            }
            finally
            {
                camera.targetTexture = previousTarget;
                RenderTexture.active = previousActive;
                UnityEngine.Object.DestroyImmediate(tex);
                UnityEngine.Object.DestroyImmediate(rt);
            }
        }
    }
}
