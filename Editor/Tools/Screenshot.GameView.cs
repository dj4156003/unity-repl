using System.Collections;

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
    [ToolComment("Capture the Game View to a PNG file, with camera fallback when needed.", "UnityReplGs.Tools.ScreenshotGameViewTool.Capture(\"Temp/game-view.png\", 1, 60)")]
    public static class ScreenshotGameViewTool
    {
        public static IEnumerator Capture(string outputPath = "Temp/unity-repl-gs-game-view.png", int superSize = 1, int maxWaitFrames = 60)
        {
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);
            if (File.Exists(outputPath))
                File.Delete(outputPath);
            ScreenCapture.CaptureScreenshot(outputPath, superSize);
            var waited = 0;
            while (!File.Exists(outputPath) && waited++ < maxWaitFrames)
                yield return null;
            var usedFallback = false;
            if (!File.Exists(outputPath) && Camera.main != null)
            {
                CaptureCamera(Camera.main, outputPath, Mathf.Max(64, Screen.width), Mathf.Max(64, Screen.height));
                usedFallback = true;
            }
            yield return JsonOut.Ok(new { path = outputPath, superSize = superSize, exists = File.Exists(outputPath), waitedFrames = waited, fallbackCamera = usedFallback });
        }

        static void CaptureCamera(Camera camera, string outputPath, int width, int height)
        {
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
